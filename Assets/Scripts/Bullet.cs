using UnityEngine;

public class Bullet : MonoBehaviour
{
    public float speed = 15f;
    public int damage = 10;
    Rigidbody2D rb;

    // ▼▼▼ "이미 충돌했는지" 기억하는 변수 추가 ▼▼▼
    private bool hasHit;

    // 오브젝트 풀에서 활성화될 때마다 호출되는 함수
    void OnEnable()
    {
        // 총알이 발사될 때마다 hasHit
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
        // 충돌시 아무것도 하지 않고 즉시 함수를 빠져나갑니다. 
        if (hasHit)
        {
            return;
        }


        if (other.CompareTag("Enemy"))
        {
            hasHit = true; // 적과 부딪혔으니 플래그.
            other.GetComponent<Enemy>().TakeDamage(damage);
            gameObject.SetActive(false);
        }
        else if (other.CompareTag("Wall"))
        {
            hasHit = true; // 벽과 부딪혔으니 플래그.
            gameObject.SetActive(false);
        }
    }
}