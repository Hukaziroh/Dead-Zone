using System.Collections;
using UnityEngine;

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

    private void Awake()
    {
        rigid = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
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

    // ▼▼▼ 충돌 함수에서 직접 코루틴을 호출하도록 변경 ▼▼▼

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player") && isLive && !isAttacking)
        {
            rigid.bodyType = RigidbodyType2D.Static;
            anim.SetBool("IsAttack", true);

            // 여기서 직접 코루틴을 시작합니다.
            StartCoroutine(AttackProcess());
        }
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            rigid.bodyType = RigidbodyType2D.Kinematic; // 또는 Dynamic
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

    // --- 나머지 함수들은 그대로 유지 ---
    #region 기존 함수들
    private void OnEnable()
    {
        // --- 기존의 초기화 코드는 그대로 둡니다 ---
        target = GameManager.instance.player.GetComponent<Rigidbody2D>();
        health = maxHealth;
        isLive = true;
        rigid.simulated = true;

        // ▼▼▼▼▼ 핵심 해결 코드 ▼▼▼▼▼
        // 이 오브젝트에 붙어있는 모든 콜라이더를 찾아서 다시 활성화(enable)시킵니다.
        foreach (Collider2D col in GetComponents<Collider2D>())
        {
            col.enabled = true;
        }
        // ▲▲▲▲▲▲▲▲▲▲▲▲▲▲▲▲▲▲▲▲▲▲

        // 공격 콜라이더는 평소에 꺼져 있어야 하므로,
        // 모든 콜라이더를 켠 후에 공격 콜라이더만 다시 한번 확실하게 꺼줍니다.
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

    IEnumerator DieSequence()
    {
        anim.SetBool("IsDead", true);
        rigid.simulated = false;
        foreach (Collider2D col in GetComponents<Collider2D>())
        {
            col.enabled = false;
        }
        yield return new WaitForSeconds(3f);
        gameObject.SetActive(false);
    }
    #endregion
}