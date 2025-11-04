using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(Collider2D))]
public class Projectile : MonoBehaviour
{
    public int damage = 20;

    private float speed;
    private Vector2 moveDirection;
    private Rigidbody2D target;
    private Rigidbody2D rb;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        rb.bodyType = RigidbodyType2D.Kinematic;
        rb.gravityScale = 0;

        Collider2D col = GetComponent<Collider2D>();
        col.isTrigger = true;
    }

    // 1단계 (유도)
    public void Init(Rigidbody2D playerTarget, float bulletSpeed)
    {
        this.target = playerTarget;
        this.speed = bulletSpeed;
        Destroy(gameObject, 5f);
    }

    // 2단계 (원형)
    public void Init(Vector2 direction, float bulletSpeed)
    {
        this.target = null;
        this.moveDirection = direction.normalized;
        this.speed = bulletSpeed;
        Destroy(gameObject, 5f);
    }

    void Update()
    {
        if (target != null)
        {
            moveDirection = (target.position - (Vector2)transform.position).normalized;
        }
    }

    private void FixedUpdate()
    {
        if (moveDirection != Vector2.zero)
        {
            rb.MovePosition(rb.position + moveDirection * speed * Time.fixedDeltaTime);
        }
    }

    // ▼▼▼ 여기에 디버깅 코드를 추가했습니다 ▼▼▼
    void OnTriggerEnter2D(Collider2D collision)
    {
        // 1. 무엇과 부딪혔는지 항상 로그를 출력합니다.
        Debug.Log($"[Projectile] 충돌 감지! 대상: {collision.gameObject.name}, 태그: {collision.gameObject.tag}");

        // 2. 플레이어 태그와 충돌했는지 확인합니다.
        if (collision.CompareTag("Player"))
        {
            Debug.Log("<color=green>[Projectile] 'Player' 태그와 정상적으로 충돌했습니다.</color>");

            Player player = collision.GetComponent<Player>();
            if (player != null)
            {
                // 3. Player 컴포넌트를 찾아서 데미지를 입힙니다.
                Debug.Log($"[Projectile] Player 컴포넌트 발견! {damage}의 데미지를 입힙니다.");
                player.TakeDamage(damage);
            }
            else
            {
                // 4. 태그는 맞지만 Player 컴포넌트가 없는 경우
                Debug.LogWarning("[Projectile] 'Player' 태그는 맞지만, Player.cs 스크립트를 찾지 못했습니다!");
            }

            Destroy(gameObject);
        }
        else if (collision.CompareTag("Wall"))
        {
            // 5. 벽과 충돌한 경우
            Debug.Log("[Projectile] 'Wall' 태그와 충돌하여 파괴됩니다.");
            Destroy(gameObject);
        }
        else
        {
            // 6. 플레이어나 벽이 아닌 다른 것과 충돌한 경우 (예: 다른 적, 아이템 등)
            Debug.Log($"[Projectile] {collision.gameObject.name}과 충돌했지만, 'Player' 또는 'Wall' 태그가 아니라서 무시합니다.");
        }
    }
}