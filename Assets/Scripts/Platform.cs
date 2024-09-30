using UnityEngine;
using UniRx;
using UniRx.Triggers;

// 발판으로서 필요한 동작을 담은 스크립트
public class Platform : MonoBehaviour
{
    public GameObject[] obstacles; // 장애물 오브젝트들
    private bool stepped = false; // 플레이어 캐릭터가 밟았었는가

    private void Start()
    {
        this.OnEnableAsObservable()
        .Subscribe(_ =>
        {
            stepped = false;

            for (int i = 0; i < obstacles.Length; i++)
            {
                // 현재 순번의 장애물을 1/3의 확률로 활성화
                if (Random.Range(0, 3) == 0)
                {
                    obstacles[i].SetActive(true);
                }
                else
                {
                    obstacles[i].SetActive(false);
                }
            }
        });

        this.OnCollisionEnter2DAsObservable()
        .Where(col => col.collider.tag == "Player" && stepped == false)
        .Subscribe(_ =>
        {
            stepped = true;
            GameManager.instance.AddScore(1);
        });
    }
}