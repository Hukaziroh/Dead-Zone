// Enemy.cs

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

    IEnumerator DieSequence()
    {
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
    #endregion
}