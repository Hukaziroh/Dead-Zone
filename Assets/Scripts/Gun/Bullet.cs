using UnityEngine;
using System.Collections;

public class Bullet : MonoBehaviour
{
    public float speed = 15f;
    public int damage = 10;
    public float lifetime = 3f;

    private Rigidbody2D rb;
    private int currentPenetration;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    void OnEnable()
    {
        StopAllCoroutines(); // 재사용 시 이전 코루틴 정지
        StartCoroutine(DisableAfterDelay(lifetime));
    }

    public void Init(Vector2 dir, int penCount)
    {
        rb.linearVelocity = dir.normalized * speed;
        currentPenetration = penCount;
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