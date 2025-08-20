using UnityEngine;

public class Enemy : MonoBehaviour
{
    public float speed;
    public float health;
    public float maxHealth;
    public int damage = 10; // 플레이어에게 주는 데미지
    public Rigidbody2D target;
    public RuntimeAnimatorController[] animCon;
    public Collider2D cameraBoundsCollider;

    bool isLive;
    Collider2D coll;
    Rigidbody2D rigid;
    Animator anim;
    SpriteRenderer spriter;

    private void Awake()
    {
        rigid = GetComponent<Rigidbody2D>();
        spriter = GetComponent<SpriteRenderer>();
        anim = GetComponent<Animator>();
        coll = GetComponent<Collider2D>();
    }

    private void FixedUpdate()
    {
        if (!isLive || target == null) return; // 살아있을 때만 움직이도록

        Vector2 dirVec = target.position - rigid.position;
        Vector2 nextVec = dirVec.normalized * speed * Time.fixedDeltaTime;
        rigid.MovePosition(rigid.position + nextVec);
        rigid.linearVelocity = Vector2.zero; // 물리적 미끄러짐 방지
    }

    private void LateUpdate()
    {
        if (!isLive) return;

        spriter.flipX = target.position.x < rigid.position.x;
    }

    // 오브젝트 풀에서 다시 활성화될 때 호출
    private void OnEnable()
    {
        target = GameManager.instance.player.GetComponent<Rigidbody2D>();
        cameraBoundsCollider = GameManager.instance.Bound; // GameManager의 Bound 직접 참조
        health = maxHealth; // 체력 초기화
        isLive = true;
        coll.enabled = true; // 콜라이더 활성화
        rigid.simulated = true; // 물리 시뮬레이션 활성화
    }

    // 플레이어와 물리적으로 충돌했을 때
    private void OnCollisionStay2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player") && isLive)
        {
            collision.gameObject.GetComponent<Player>().TakeDamage(damage);
        }
    }

    // 총알에 맞았을 때 호출될 함수 (가장 중요)
    public void TakeDamage(int damage)
    {
        health -= damage;

        if (health <= 0 && isLive)
        {
            Die();
        }
    }

    void Die()
    {
        isLive = false;
        coll.enabled = false; // 다른 총알에 또 맞지 않도록 콜라이더 비활성화
        rigid.simulated = false; // 물리 효과 정지

        // 여기에 죽는 애니메이션이나 효과를 넣을 수 있습니다.

        // 오브젝트 풀로 돌아가기 위해 비활성화
        gameObject.SetActive(false);
    }
}