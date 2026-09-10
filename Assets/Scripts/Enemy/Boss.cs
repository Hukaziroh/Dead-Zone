using System.Collections;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class Boss : MonoBehaviour
{
    public float speed;
    public float health;
    public float maxHealth = 1000;
    public int damage = 20;
    public Rigidbody2D target;

    bool isLive;
    Rigidbody2D rigid;
    Animator anim;

    public AudioClip deathSound;
    private AudioSource audioSource;

    // ▼▼▼ GameManager를 담을 변수 선언 ▼▼▼
    private GameManager gameManager;

    private void Awake()
    {
        rigid = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        audioSource = GetComponent<AudioSource>();
        // ▼▼▼ 씬에서 GameManager를 찾아 변수에 할당 ▼▼▼
        gameManager = FindAnyObjectByType<GameManager>();
    }

    private void OnEnable()
    {
        // ▼▼▼ gameManager 변수를 통해 player에 접근 ▼▼▼
        target = gameManager.player?.GetComponent<Rigidbody2D>();
        health = maxHealth;
        isLive = true;
        rigid.simulated = true;
        GetComponent<Collider2D>().enabled = true;

        // ▼▼▼ gameManager 변수를 통해 함수 호출 ▼▼▼
        gameManager.ShowBossHealthBar(this);
    }

    private void FixedUpdate()
    {
        if (!isLive || target == null)
        {
            rigid.linearVelocity = Vector2.zero;
            return;
        }
        Vector2 dirVec = (target.position - rigid.position).normalized;
        Vector2 nextVec = dirVec * speed * Time.fixedDeltaTime;
        rigid.MovePosition(rigid.position + nextVec);
        rigid.linearVelocity = Vector2.zero;

        anim.SetFloat("MoveX", dirVec.x);
        anim.SetFloat("MoveY", dirVec.y);
    }

    void OnCollisionStay2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player") && isLive)
        {
            collision.gameObject.GetComponent<Player>().TakeDamage(damage);
        }
    }

    public void TakeDamage(int damage)
    {
        if (!isLive) return;
        health -= damage;
        // ▼▼▼ gameManager 변수를 통해 함수 호출 ▼▼▼
        gameManager.UpdateBossHealth();

        if (health <= 0)
        {
            StartCoroutine(DieSequence());
        }
    }

    IEnumerator DieSequence()
    {
        isLive = false;
        if (deathSound != null) audioSource.PlayOneShot(deathSound);

        // ▼▼▼ gameManager 변수를 통해 함수 호출 ▼▼▼
        gameManager.BossDied();
        anim.SetTrigger("IsDead");
        rigid.simulated = false;
        GetComponent<Collider2D>().enabled = false;
        yield return new WaitForSeconds(3f);
        gameObject.SetActive(false);
    }
}