// Boss.cs (수정된 최종 버전 - 공격 로직 명확화)

using System.Collections;
using UnityEngine;

public class Boss : MonoBehaviour
{
    public Collider2D attackCollider; // Inspector에서 공격 판정용 콜라이더 할당 (Is Trigger 체크 필요)
    public float speed;
    public float health;       // 현재 체력
    public float maxHealth = 1000; // 최대 체력 (Inspector에서 조절)
    public int damage = 10;
    public Rigidbody2D target;

    bool isLive;
    Rigidbody2D rigid;
    Animator anim;
    private bool isAttacking = false;

    // 이 변수를 사용하여 플레이어가 현재 충돌 상태인지 추적
    private bool isPlayerColliding = false;

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
            if (target == null)
            {
                Debug.LogError("Boss OnEnable: Player 오브젝트에 Rigidbody2D 컴포넌트가 없습니다! 보스가 플레이어를 추적할 수 없습니다.");
            }
        }
        else
        {
            Debug.LogWarning("Boss OnEnable: GameManager.instance 또는 player가 아직 설정되지 않았습니다. 타겟 할당 실패!");
            target = null;
        }

        health = maxHealth;
        isLive = true;
        rigid.simulated = true;

        // OnEnable 시점에 모든 콜라이더 활성화 (공격 콜라이더는 AttackProcess에서 관리)
        foreach (Collider2D col in GetComponents<Collider2D>())
        {
            col.enabled = true;
        }
        if (attackCollider != null)
        {
            attackCollider.enabled = false; // 공격 콜라이더는 시작 시 비활성화
        }

        gameObject.layer = LayerMask.NameToLayer("SpawningEnemy");
        StartCoroutine(BecomeSolidAfterDelay());

        Debug.Log("<color=blue>Boss OnEnable: GameManager.ShowBossHealthBar 호출 시도</color>");
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

        Debug.Log($"<color=red>Boss 데미지 받음: {damage}, 현재 체력: {health}</color>");
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
        if (attackCollider != null) attackCollider.enabled = false; // 혹시 모를 상황 대비

        yield return new WaitForSeconds(3f);
        gameObject.SetActive(false);
    }

    // ▼▼▼ AttackProcess 코루틴 수정: 공격 판정은 여기서만 발생하도록 ▼▼▼
    IEnumerator AttackProcess()
    {
        isAttacking = true;
        rigid.linearVelocity = Vector2.zero; // 공격 중에는 멈춤
        anim.SetBool("IsAttack", true); // 공격 애니메이션 시작

        yield return new WaitForSeconds(0.5f); // 공격 애니메이션 중간까지 대기

        if (attackCollider != null)
        {
            attackCollider.enabled = true; // 공격 판정 콜라이더 활성화

            // ▼▼▼ 여기에 직접 데미지 주는 로직 추가 (OnTriggerEnter2D 대신) ▼▼▼
            // Player player = GameManager.instance.player; // GameManager에서 플레이어 참조를 가져옴
            // if (player != null && Vector2.Distance(attackCollider.bounds.center, player.transform.position) < attackCollider.bounds.extents.magnitude + 0.5f) // 간접적으로 충돌 검사
            // {
            //    player.TakeDamage(damage);
            //    Debug.Log($"<color=red>Boss 공격! 플레이어에게 {damage} 데미지 줌. 플레이어 현재 HP: {player.currentHealth}</color>");
            // }

            // 또는 Physics2D.OverlapCircleAll 등으로 현재 attackCollider 범위 내에 플레이어가 있는지 직접 검사
            Collider2D[] hitPlayers = Physics2D.OverlapCircleAll(attackCollider.bounds.center, attackCollider.bounds.extents.x, LayerMask.GetMask("Player"));
            foreach (Collider2D hit in hitPlayers)
            {
                if (hit.CompareTag("Player"))
                {
                    Player player = hit.GetComponent<Player>();
                    if (player != null)
                    {
                        player.TakeDamage(damage);
                        Debug.Log($"<color=red>Boss 공격! 플레이어에게 {damage} 데미지 줌. 플레이어 현재 HP: {player.currentHealth}</color>");
                    }
                }
            }
        }

        yield return new WaitForSeconds(0.2f); // 공격 판정 유지 시간

        if (attackCollider != null) attackCollider.enabled = false; // 공격 판정 콜라이더 비활성화

        anim.SetBool("IsAttack", false); // 공격 애니메이션 종료
        isAttacking = false;
    }

    // ▼▼▼ OnCollisionEnter2D 수정: 여기서 직접 데미지를 주지 않습니다. ▼▼▼
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player") && isLive && !isAttacking)
        {
            // 플레이어와 몸통이 닿았을 때 공격 애니메이션 시작
            // rigid.bodyType = RigidbodyType2D.Static; // 공격 중 멈추려면
            isPlayerColliding = true; // 플레이어와 닿아있음을 표시
            StartCoroutine(AttackProcess());
        }
    }

    // ▼▼▼ OnCollisionExit2D 수정: 플레이어와 떨어졌을 때 isPlayerColliding 초기화 ▼▼▼
    private void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            // rigid.bodyType = RigidbodyType2D.Kinematic; // 다시 움직이게
            isPlayerColliding = false; // 플레이어와 떨어졌음을 표시
            // anim.SetBool("IsAttack", false); // 공격 애니메이션은 AttackProcess에서 끄도록
        }
    }

    // ▼▼▼ 이 함수는 이제 사용하지 않습니다. 완전히 삭제하거나 주석 처리합니다. ▼▼▼
    // private void OnTriggerEnter2D(Collider2D other)
    // {
    //     // 이 함수는 보스의 메인 콜라이더가 트리거일 때 호출되므로,
    //     // 공격 판정용 attackCollider의 OnTriggerEnter2D와 분리해야 합니다.
    //     // 여기서는 데미지를 주지 않는 것이 좋습니다.
    // }
}