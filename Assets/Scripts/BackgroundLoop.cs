using UnityEngine;
using UniRx;
using UniRx.Triggers;

// 왼쪽 끝으로 이동한 배경을 오른쪽 끝으로 재배치하는 스크립트
public class BackgroundLoop : MonoBehaviour
{
    private float width; // 배경의 가로 길이

    private void Awake()
    {
        // BoxCollider2D 컴포넌트의 Size 필드의 X 값을 가로 길이로 사용
        BoxCollider2D backgroundCollider = GetComponent<BoxCollider2D>();
        width = backgroundCollider.size.x;
    }

    private void Start()
    {
        this.UpdateAsObservable()
        .Where(_ => transform.position.x <= -width)
        .Subscribe(_ =>
        {
            Vector2 offset = new Vector2(width * 2f, 0);
            transform.position = (Vector2)transform.position + offset;
        });
    }
}