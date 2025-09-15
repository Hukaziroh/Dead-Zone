// Bullet.cs (수정된 OnTriggerEnter2D 함수)

using UnityEngine;

public class Bullet : MonoBehaviour
{
    public float speed = 15f;
    public int damage = 10;
    Rigidbody2D rb;

    private bool hasHit;

    void OnEnable()
    {
        hasHit = false;
    }

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    public void Init(Vector2 dir)
    {
        rb.linearVelocity = dir.normalized * speed;
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (hasHit)
        {
            return;
        }

        // ▼▼▼ 여기에 Boss 태그 처리 로직 추가 ▼▼▼
        if (other.CompareTag("Boss"))
        {
            Boss boss = other.GetComponent<Boss>(); // Boss 컴포넌트 가져오기
            if (boss != null) // Boss 컴포넌트가 있는지 확인
            {
                hasHit = true; // 보스와 부딪혔으니 플래그 설정
                boss.TakeDamage(damage);
                gameObject.SetActive(false); // 총알 비활성화
            }
            else
            {
                Debug.LogWarning("총알이 'Boss' 태그 오브젝트와 충돌했지만 Boss 스크립트를 찾을 수 없습니다: " + other.name);
                // 스크립트가 없어도 총알은 사라지게 하려면 여기서 gameObject.SetActive(false);
                // 하지만 현재 목표는 데미지이므로 스크립트 없는 경우 총알은 유지
            }
        }
        else if (other.CompareTag("Enemy"))
        {
            Enemy enemy = other.GetComponent<Enemy>(); // Enemy 컴포넌트 가져오기
            if (enemy != null) // Enemy 컴포넌트가 있는지 확인
            {
                hasHit = true; // 적과 부딪혔으니 플래그 설정
                enemy.TakeDamage(damage);
                gameObject.SetActive(false);
            }
            else
            {
                Debug.LogWarning("총알이 'Enemy' 태그 오브젝트와 충돌했지만 Enemy 스크립트를 찾을 수 없습니다: " + other.name);
            }
        }
        else if (other.CompareTag("Wall"))
        {
            hasHit = true; // 벽과 부딪혔으니 플래그 설정
            gameObject.SetActive(false);
        }
        // ▼▼▼ 이 외의 다른 오브젝트와 충돌 시 총알을 어떻게 할지 결정 ▼▼▼
        // 현재는 아무것도 안 하면 총알이 계속 날아갑니다.
        // 예를 들어, else { hasHit = true; gameObject.SetActive(false); } 를 추가하여 모든 충돌에 총알이 사라지게 할 수 있습니다.
    }
}