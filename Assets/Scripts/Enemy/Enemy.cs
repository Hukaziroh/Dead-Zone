using System.Collections;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class Enemy : MonoBehaviour
{
    [Header("Components")]
    public Collider2D attackCollider; // 공격 판정에 사용할 별도의 콜라이더

    [Header("Stats")]
    public float speed;
    public float health;
    public float maxHealth;
    public int damage = 10;
    public Rigidbody2D target;

    [Header("Sound")]
    public AudioClip deathSound;

    // 내부 변수들
    bool isLive;
    Rigidbody2D rigid;
    Animator anim;
    private bool isAttacking = false;
    private AudioSource audioSource;
    private GameManager gameManager;

    private void Awake()
    {
        rigid = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        audioSource = GetComponent<AudioSource>();
        gameManager = FindAnyObjectByType<GameManager>();
    }

    private void OnEnable()
    {
        if (gameManager != null && gameManager.player != null)
        {
            target = gameManager.player.GetComponent<Rigidbody2D>();
        }

        health = maxHealth;
        isLive = true;
        isAttacking = false;
        rigid.simulated = true;
        //rigid.bodyType = RigidbodyType2D.Kinematic; // 시작 시 Kinematic으로 설정

        // 모든 콜라이더 활성화
        foreach (Collider2D col in GetComponents<Collider2D>())
        {
            col.enabled = true;
        }

        // 공격 콜라이더는 비활성화로 시작
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

    // ▼▼▼ 사용자님의 원래 공격 로직 (그대로 유지) ▼▼▼
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player") && isLive && !isAttacking)
        {
            rigid.bodyType = RigidbodyType2D.Static; // 플레이어와 부딪히면 멈춤
            anim.SetBool("IsAttack", true);
            StartCoroutine(AttackProcess());
        }
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            rigid.bodyType = RigidbodyType2D.Kinematic; // 플레이어가 떨어지면 다시 움직임
            anim.SetBool("IsAttack", false);
        }
    }

    // 공격 콜라이더(Trigger)가 플레이어에게 닿았을 때만 데미지를 줌
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            other.GetComponent<Player>().TakeDamage(damage);
        }
    }

    // 공격 애니메이션 중간에 콜라이더를 켜고 끄는 코루틴
    IEnumerator AttackProcess()
    {
        isAttacking = true;
        yield return new WaitForSeconds(0.5f); // 0.5초 대기 (공격 선딜레이)
        if (attackCollider != null) attackCollider.enabled = true;
        yield return new WaitForSeconds(0.2f); // 0.2초 동안만 공격 판정 활성화
        if (attackCollider != null) attackCollider.enabled = false;
        isAttacking = false;
    }

    public void TakeDamage(int damage)
    {
        if (!isLive) return;
        health -= damage;
        if (health <= 0)
        {
            isLive = false;
            StartCoroutine(DieSequence());
        }
    }

    IEnumerator DieSequence()
    {
        if (deathSound != null) audioSource.PlayOneShot(deathSound);

        anim.SetTrigger("IsDead");
        rigid.simulated = false;
        foreach (Collider2D col in GetComponents<Collider2D>())
        {
            col.enabled = false;
        }

        if (gameManager != null)
        {
            gameManager.AddKill();
        }

        yield return new WaitForSeconds(3f);
        gameObject.SetActive(false);
    }
}