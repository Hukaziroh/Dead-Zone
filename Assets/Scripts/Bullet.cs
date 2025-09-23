// Bullet.cs (3초 후 자동 비활성화 기능 추가)

using UnityEngine;
using System.Collections; // 코루틴을 사용하기 위해 추가!

public class Bullet : MonoBehaviour
{
    public float speed = 15f;
    public int damage = 10;
    Rigidbody2D rb;

    private bool hasHit;
    public float lifetime = 3f; // 총알의 수명 (3초)

    void OnEnable()
    {
        hasHit = false;
        // ▼▼▼ 오브젝트 풀에서 활성화될 때마다 코루틴 시작 ▼▼▼
        StartCoroutine(DisableAfterDelay(lifetime));
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

        if (other.CompareTag("Boss"))
        {
            Boss boss = other.GetComponent<Boss>();
            if (boss != null)
            {
                hasHit = true;
                boss.TakeDamage(damage);
                gameObject.SetActive(false);
            }
            else
            {
                Debug.LogWarning("총알이 'Boss' 태그 오브젝트와 충돌했지만 Boss 스크립트를 찾을 수 없습니다: " + other.name);
            }
        }
        else if (other.CompareTag("Enemy"))
        {
            Enemy enemy = other.GetComponent<Enemy>();
            if (enemy != null)
            {
                hasHit = true;
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
            hasHit = true;
            gameObject.SetActive(false);
        }
    
    }

    // ▼▼▼ 3초 후 총알을 비활성화하는 코루틴 추가 ▼▼▼
    IEnumerator DisableAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);

        // 이미 다른 충돌로 비활성화되지 않았다면 비활성화
        if (gameObject.activeSelf)
        {
            gameObject.SetActive(false);
        }
    }
}