// Bullet.cs 스크립트

using UnityEngine;

public class Bullet : MonoBehaviour
{
    public float speed = 10f;
    public int damage = 10;
    Rigidbody2D rb;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        // Rigidbody를 제대로 찾았는지 확인
        if (rb == null)
        {
            Debug.LogError("Bullet에서 Rigidbody 2D를 찾을 수 없습니다!");
        }
    }

    // 총알 발사 시 호출될 함수
    public void Init(Vector2 dir)
    {
        Debug.Log("7. Bullet.Init 함수 시작됨. 전달받은 방향: " + dir);
        Debug.Log("8. 현재 총알의 속도(Speed) 변수 값: " + speed);
        Debug.Log("9. Rigidbody 타입: " + rb.bodyType);

        if (speed > 0)
        {
            rb.linearVelocity = dir.normalized * speed;
            Debug.Log("10. 최종 속도(velocity) 설정 완료: " + rb.linearVelocity);
        }
        else
        {
            Debug.LogWarning("총알의 Speed 값이 0 이라서 움직일 수 없습니다!");
        }
    }

    // 트리거 충돌 시 호출
    void OnTriggerEnter2D(Collider2D other)
    {
        // 부딪힌 대상의 태그가 "Enemy"일 경우
        if (other.CompareTag("Enemy"))
        {
            other.GetComponent<Enemy>().TakeDamage(damage);
            gameObject.SetActive(false); // 총알 비활성화
        }
        // 부딪힌 대상의 태그가 "Wall"일 경우
        else if (other.CompareTag("Wall"))
        {
            gameObject.SetActive(false); // 총알 비활성화
        }
    }
}