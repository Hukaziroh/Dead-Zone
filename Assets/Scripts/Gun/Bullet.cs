// Bullet.cs (최종 수정본)
using UnityEngine;
using System.Collections;

public class Bullet : MonoBehaviour
{
    public float speed = 15f;
    public int damage = 10;

    // 이 변수는 이제 Init에서 설정되므로 public일 필요가 없습니다.
    private float lifetime;

    private Rigidbody2D rb;
    private int currentPenetration;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    void OnEnable()
    {
        // OnEnable에서는 이전에 실행되던 코루틴을 확실히 멈추기만 합니다.
        StopAllCoroutines();
    }

    // ▼▼▼ Init 함수를 아래와 같이 수정합니다 ▼▼▼
    public void Init(Vector2 dir, int penCount, float life)
    {
        rb.linearVelocity = dir.normalized * speed;
        currentPenetration = penCount;
        lifetime = life; // 사거리 정보 설정

        // 모든 정보가 설정된 후에 비행 시작(자동 비활성화 코루틴 시작)
        StartCoroutine(DisableAfterDelay(lifetime));
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Boss") || other.CompareTag("Enemy"))
        {
            if (other.CompareTag("Boss"))
                other.GetComponent<Boss>()?.TakeDamage(damage);
            else if (other.CompareTag("Enemy"))
                other.GetComponent<Enemy>()?.TakeDamage(damage);

            currentPenetration--;

            if (currentPenetration <= 0)
            {
                gameObject.SetActive(false);
            }
        }
        else if (other.CompareTag("Wall"))
        {
            gameObject.SetActive(false);
        }
    }

    IEnumerator DisableAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        if (gameObject.activeSelf)
        {
            gameObject.SetActive(false);
        }
    }
}