// Boss.cs (디버그 메시지 제거 버전)

using System.Collections;
using UnityEngine;

public class Boss : MonoBehaviour
{
    public Collider2D attackCollider;
    public float speed;
    public float health;
    public float maxHealth = 1000;
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

    private void OnEnable()
    {
        if (GameManager.instance != null && GameManager.instance.player != null)
        {
            target = GameManager.instance.player.GetComponent<Rigidbody2D>();
        }
        else
        {
            target = null;
        }

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

        gameObject.layer = LayerMask.NameToLayer("SpawningEnemy");
        StartCoroutine(BecomeSolidAfterDelay());

        GameManager.instance.ShowBossHealthBar(this);
    }

    IEnumerator BecomeSolidAfterDelay()
    {
        yield return new WaitForSeconds(2f);
        gameObject.layer = LayerMask.NameToLayer("Enemy");
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
        if (!isLive) return;

        health -= damage;
        GameManager.instance.UpdateBossHealth();

        if (health <= 0)
        {
            StartCoroutine(DieSequence());
        }
    }

    IEnumerator DieSequence()
    {
        isLive = false;
        GameManager.instance.BossDied();
        GameManager.instance.AddKill();

        anim.SetBool("IsDead", true);
        rigid.simulated = false;
        foreach (Collider2D col in GetComponents<Collider2D>())
        {
            col.enabled = false;
        }
        if (attackCollider != null) attackCollider.enabled = false;

        yield return new WaitForSeconds(3f);
        gameObject.SetActive(false);
    }

    IEnumerator AttackProcess()
    {
        isAttacking = true;
        rigid.linearVelocity = Vector2.zero;
        anim.SetBool("IsAttack", true);

        yield return new WaitForSeconds(0.5f);

        if (attackCollider != null)
        {
            attackCollider.enabled = true;

            Collider2D[] hitPlayers = Physics2D.OverlapCircleAll(attackCollider.bounds.center, attackCollider.bounds.extents.x, LayerMask.GetMask("Player"));
            foreach (Collider2D hit in hitPlayers)
            {
                if (hit.CompareTag("Player"))
                {
                    Player player = hit.GetComponent<Player>();
                    if (player != null)
                    {
                        player.TakeDamage(damage);
                    }
                }
            }
        }

        yield return new WaitForSeconds(0.2f);

        if (attackCollider != null) attackCollider.enabled = false;

        anim.SetBool("IsAttack", false);
        isAttacking = false;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player") && isLive && !isAttacking)
        {
            StartCoroutine(AttackProcess());
        }
    }

   
}