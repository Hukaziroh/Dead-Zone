using UnityEngine;

public class Projectile : MonoBehaviour
{
    private float speed;
    private Vector2 moveDirection;

    // 1단계 공격용: 유도 탄막 공격 (Rigidbody2D와 속도를 받도록 수정)
    public void Init(Rigidbody2D playerTarget, float bulletSpeed)
    {
        if (playerTarget != null)
        {
            moveDirection = (playerTarget.position - (Vector2)transform.position).normalized;
        }
        else
        {
            moveDirection = Vector2.down;
        }
        speed = bulletSpeed; // 속도 설정
        Destroy(gameObject, 5f);
    }

    // 2단계 공격용: 360도 고정 방향 탄막 (Vector2와 속도를 받음)
    public void Init(Vector2 direction, float bulletSpeed)
    {
        moveDirection = direction.normalized;
        speed = bulletSpeed; // 속도 설정
        Destroy(gameObject, 5f);
    }

    private void FixedUpdate()
    {
        if (moveDirection != Vector2.zero)
        {
            transform.Translate(moveDirection * speed * Time.fixedDeltaTime, Space.World);
        }
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            collision.GetComponent<Player>()?.TakeDamage(10);
            Destroy(gameObject);
        }
    }
}