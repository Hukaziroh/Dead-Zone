// Enemy.cs (죽음 소리 기능 추가 버전)

using System.Collections;
using UnityEngine;

// ▼▼▼ 1. AudioSource를 사용하기 위해 RequireComponent 추가 ▼▼▼
[RequireComponent(typeof(AudioSource))]
public class Enemy : MonoBehaviour
{
    public Collider2D attackCollider;
    public float speed;
    public float health;
    public float maxHealth;
    public int damage = 10;
    public Rigidbody2D target;

    bool isLive;
    Rigidbody2D rigid;
    Animator anim;
    private bool isAttacking = false;

    // ▼▼▼ 2. 사운드 재생을 위한 변수 추가 ▼▼▼
    public AudioClip deathSound; // 인스펙터에서 지정할 죽음 소리
    private AudioSource audioSource;

    private void Awake()
    {
        rigid = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        // ▼▼▼ 3. AudioSource 컴포넌트 초기화 ▼▼▼
        audioSource = GetComponent<AudioSource>();
    }

    IEnumerator AttackProcess()
    {
        isAttacking = true;
        yield return new WaitForSeconds(0.5f);
        if (attackCollider != null) attackCollider.enabled = true;
        yield return new WaitForSeconds(0.2f);
        if (attackCollider != null) attackCollider.enabled = false;
        isAttacking = false;
    }

    IEnumerator DieSequence()
    {
        // ▼▼▼ 4. 죽음 애니메이션 시작과 동시에 사운드 재생 ▼▼▼
        if (deathSound != null)
        {
            audioSource.PlayOneShot(deathSound);
        }

        anim.SetBool("IsDead", true);
        rigid.simulated = false;
        foreach (Collider2D col in GetComponents<Collider2D>())
        {
            col.enabled = false;
        }
        GameManager.instance.AddKill();
        yield return new WaitForSeconds(3f);
        gameObject.SetActive(false);
    }

    // --- (이하 기존 코드와 동일) ---

    #region 기존 코드
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player") && isLive && !isAttacking)
        {
            rigid.bodyType = RigidbodyType2D.Static;
            anim.SetBool("IsAttack", true);
            StartCoroutine(AttackProcess());
        }
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            rigid.bodyType = RigidbodyType2D.Kinematic;
            anim.SetBool("IsAttack", false);
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            other.GetComponent<Player>().TakeDamage(damage);
        }
    }

    private void OnEnable()
    {
        target = GameManager.instance.player.GetComponent<Rigidbody2D>();
        health = maxHealth;
        isLive = true;
        rigid.simulated = true;
        foreach (Collider2D col in GetComponents<Collider2D>())
        {
            col.enabled = true;
        }
        if (attackCollider != null)
        {
            attackCollider.enabled = false;
        }
    }

    private void FixedUpdate()
    {
        if (!isLive || target == null || anim.GetBool("IsAttack"))
        {
            rigid.linearVelocity = Vector2.zero;
            return;
        }
        Vector2 dirVec = target.position - rigid.position;
        Vector2 nextVec = dirVec.normalized * speed * Time.fixedDeltaTime;
        rigid.MovePosition(rigid.position + nextVec);
        rigid.linearVelocity = Vector2.zero;
        if (dirVec.sqrMagnitude > 0.01f)
        {
            anim.SetFloat("MoveX", dirVec.normalized.x);
            anim.SetFloat("MoveY", dirVec.normalized.y);
        }
    }

    public void TakeDamage(int damage)
    {
        health -= damage;
        if (health <= 0 && isLive)
        {
            isLive = false;
            StartCoroutine(DieSequence());
        }
    }
    #endregion
}