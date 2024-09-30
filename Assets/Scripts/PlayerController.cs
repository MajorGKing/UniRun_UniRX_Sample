using UnityEngine;
using UniRx;
using UniRx.Triggers;

// PlayerController는 플레이어 캐릭터로서 Player 게임 오브젝트를 제어한다.
public class PlayerController : MonoBehaviour {
    public AudioClip deathClip; // 사망시 재생할 오디오 클립
    public float jumpForce = 700f; // 점프 힘

    private int jumpCount = 0; // 누적 점프 횟수
    private bool isGrounded = false; // 바닥에 닿았는지 나타냄
    private bool isDead = false; // 사망 상태

    private Rigidbody2D playerRigidbody; // 사용할 리지드바디 컴포넌트
    private Animator animator; // 사용할 애니메이터 컴포넌트
    private AudioSource playerAudio; // 사용할 오디오 소스 컴포넌트

    public int maxJump = 2;

    private void Start() {
        // 게임 오브젝트로부터 사용할 컴포넌트들을 가져와 변수에 할당
        playerRigidbody = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        playerAudio = GetComponent<AudioSource>();

        var jump = this.UpdateAsObservable()
                .Where(_ => isDead == false && Input.GetMouseButtonDown(0));

        var jumpEnd = this.UpdateAsObservable()
                    .Where(_ => isDead == false && Input.GetMouseButtonUp(0));

        jump.Where(_ => maxJump > jumpCount)
        .Subscribe(_ =>
        {
            // 점프 횟수 증가
            jumpCount++;
            // 점프 직전에 속도를 순간적으로 제로(0, 0)로 변경
            playerRigidbody.velocity = Vector2.zero;
            // 리지드바디에 위쪽으로 힘을 주기
            playerRigidbody.AddForce(new Vector2(0, jumpForce));
            // 오디오 소스 재생
            playerAudio.Play();
        });

        jumpEnd.Where(_ => playerRigidbody.velocity.y > 0)
        .Subscribe(_ => playerRigidbody.velocity = playerRigidbody.velocity * 0.5f);

        this.OnTriggerEnter2DAsObservable()
        .Where(other =>  other.tag == "Dead" && !isDead)
        .Subscribe(_ => Die());

        this.OnCollisionEnter2DAsObservable()
        .Where(collision => collision.contacts[0].normal.y > 0.7f)
        .Subscribe(_ =>
        {
            isGrounded = true;
            jumpCount = 0;
            animator.SetBool("Grounded", isGrounded);
        });

        this.OnCollisionExit2DAsObservable()
        .Subscribe(_ => 
        {
            isGrounded = false;
            animator.SetBool("Grounded", isGrounded);
        });

        
    }

    // private void Update() {
    //     if (isDead)
    //     {
    //         // 사망시 처리를 더 이상 진행하지 않고 종료
    //         return;
    //     }

        // // 마우스 왼쪽 버튼을 눌렀으며 && 최대 점프 횟수(2)에 도달하지 않았다면
        // if (Input.GetMouseButtonDown(0) && jumpCount < 2)
        // {
        //     // // 점프 횟수 증가
        //     // jumpCount++;
        //     // // 점프 직전에 속도를 순간적으로 제로(0, 0)로 변경
        //     // playerRigidbody.velocity = Vector2.zero;
        //     // // 리지드바디에 위쪽으로 힘을 주기
        //     // playerRigidbody.AddForce(new Vector2(0, jumpForce));
        //     // // 오디오 소스 재생
        //     // playerAudio.Play();
        // }
        // else if (Input.GetMouseButtonUp(0) && playerRigidbody.velocity.y > 0)
        // {
        //     // 마우스 왼쪽 버튼에서 손을 떼는 순간 && 속도의 y 값이 양수라면 (위로 상승 중)
        //     // 현재 속도를 절반으로 변경
        //     playerRigidbody.velocity = playerRigidbody.velocity * 0.5f;
        // }

        // // 애니메이터의 Grounded 파라미터를 isGrounded 값으로 갱신
        // animator.SetBool("Grounded", isGrounded);
    //}

    private void Die() {
        // 애니메이터의 Die 트리거 파라미터를 셋
        animator.SetTrigger("Die");

        // 오디오 소스에 할당된 오디오 클립을 deathClip으로 변경
        playerAudio.clip = deathClip;
        // 사망 효과음 재생
        playerAudio.Play();

        // 속도를 제로(0, 0)로 변경
        playerRigidbody.velocity = Vector2.zero;
        // 사망 상태를 true로 변경
        isDead = true;

        // 게임 매니저의 게임 오버 처리 실행
        GameManager.instance.OnPlayerDead();
    }
}