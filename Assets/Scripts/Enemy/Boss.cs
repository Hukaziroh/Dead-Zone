using System.Collections;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class Boss : MonoBehaviour
{
    // ▼▼▼ Inspector 값 참고 ▼▼▼
    public float speed = 1.5f;
    public float health;
    public float maxHealth = 3000;
    public int damage = 100;
    public Rigidbody2D target;

    // ▼▼▼ 탄막 패턴 변수 ▼▼▼
    public GameObject projectilePrefab;
    public float guidedAttackInterval = 1.5f;     // 1단계 공격 주기
    public float fullCircleBullets = 12;
    public float fullCircleAttackInterval = 1.0f; // 2단계 공격 주기
    public float projectileSpawnDistance = 1f;

    // ▼▼▼ 탄막 속도 변수 ▼▼▼
    private float currentProjectileSpeed;
    private const float PHASE_TWO_SPEED = 4f;     // 2단계 속도
    private const float PHASE_ONE_SPEED = 2f;     // 1단계 속도

    bool isLive;
    Rigidbody2D rigid;
    Animator anim;

    private IEnumerator currentAttackRoutine;
    private bool isPhaseTwoActive = false;

    public AudioClip deathSound;
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
        health = maxHealth;
        isLive = true;
        rigid.simulated = true;
        GetComponent<Collider2D>().enabled = true;

        gameManager.ShowBossHealthBar(this);

        // 초기화
        isPhaseTwoActive = false;
        currentProjectileSpeed = PHASE_ONE_SPEED; // 1단계 속도 2f로 설정

        // 플레이어 찾기 및 공격 시작 코루틴 실행
        StartCoroutine(FindPlayerAndStartAttack());
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
            collision.gameObject.GetComponent<Player>()?.TakeDamage(damage);
        }
    }

    public void TakeDamage(int damage)
    {
        if (!isLive) return;
        health -= damage;
        gameManager.UpdateBossHealth();

        // ▼▼▼ 체력이 50% 이하일 때 2단계로 전환 ▼▼▼
        if (health <= maxHealth * 0.5f && !isPhaseTwoActive)
        {
            isPhaseTwoActive = true;
            currentProjectileSpeed = PHASE_TWO_SPEED; // 탄막 속도 4f로 변경

            // 기존 1단계 루틴 중지
            if (currentAttackRoutine != null)
            {
                StopCoroutine(currentAttackRoutine);
            }

            // 2단계(360도 탄막) 루틴 시작
            currentAttackRoutine = FullCircleBulletRoutine();
            StartCoroutine(currentAttackRoutine);
        }

        if (health <= 0)
        {
            StartCoroutine(DieSequence());
        }
    }

    // ▼▼▼ 플레이어를 찾고 공격 루틴을 시작하는 코루틴 (초기화) ▼▼▼
    IEnumerator FindPlayerAndStartAttack()
    {
        while (target == null)
        {
            target = gameManager.player?.GetComponent<Rigidbody2D>();
            if (target == null)
            {
                yield return new WaitForSeconds(0.1f);
            }
        }

        if (projectilePrefab != null)
        {
            // 1단계 유도 공격 루틴 시작
            currentAttackRoutine = GuidedBulletRoutine();
            StartCoroutine(currentAttackRoutine);
        }
        else
        {
            Debug.LogError("Boss: Projectile Prefab is not assigned in the Inspector!");
        }
    }

    // ▼▼▼ 1단계 패턴: 플레이어 유도 탄막 발사 루틴 (50% 이전) ▼▼▼
    IEnumerator GuidedBulletRoutine()
    {
        while (isLive && !isPhaseTwoActive) // 2단계 활성화 전까지만 실행
        {
            yield return new WaitForSeconds(guidedAttackInterval); // 공격 주기 대기
            if (isLive && target != null)
            {
                anim.SetBool("IsAttack", true); // 1단계 기본 공격 모션 시작

                // 탄막 즉시 발사 (Rigidbody2D와 속도 전달)
                Vector3 spawnPosition = transform.position + (Vector3)(target.position - rigid.position).normalized * projectileSpawnDistance;
                GameObject bullet = Instantiate(projectilePrefab, spawnPosition, Quaternion.identity);
                bullet.GetComponent<Projectile>()?.Init(target, currentProjectileSpeed);

                yield return new WaitForSeconds(0.4f); // 모션 재생 시간 대기
                anim.SetBool("IsAttack", false); // 공격 모션 종료
            }
        }
    }

    // ▼▼▼ 2단계 패턴: 360도 전방위 탄막 발사 루틴 (50% 이후) ▼▼▼
    IEnumerator FullCircleBulletRoutine()
    {
        while (isLive && isPhaseTwoActive) // 2단계 활성화 상태에서만 실행
        {
            yield return new WaitForSeconds(fullCircleAttackInterval); // 공격 주기 대기

            if (isLive)
            {
                anim.SetBool("IsPhase2Attack", true); // 2단계 공격 모션 시작

                // 탄막 즉시 발사 (360도)
                float angleStep = 360f / fullCircleBullets;
                Vector3 spawnPosition = transform.position;

                for (int i = 0; i < fullCircleBullets; i++)
                {
                    float angle = i * angleStep;

                    float bulletDirX = Mathf.Cos(angle * Mathf.Deg2Rad);
                    float bulletDirY = Mathf.Sin(angle * Mathf.Deg2Rad);

                    Vector2 bulletDirection = new Vector2(bulletDirX, bulletDirY);

                    GameObject bullet = Instantiate(projectilePrefab, spawnPosition + (Vector3)bulletDirection * projectileSpawnDistance, Quaternion.identity);
                    // Vector2와 속도 전달
                    bullet.GetComponent<Projectile>()?.Init(bulletDirection, currentProjectileSpeed);
                }

                yield return new WaitForSeconds(0.4f); // 모션 재생 시간 대기
                anim.SetBool("IsPhase2Attack", false); // 2단계 모션 종료
            }
        }
    }

    IEnumerator DieSequence()
    {
        isLive = false;

        if (currentAttackRoutine != null)
        {
            StopCoroutine(currentAttackRoutine);
        }
        StopCoroutine(FindPlayerAndStartAttack());

        if (deathSound != null) audioSource.PlayOneShot(deathSound);

        gameManager.BossDied();
        anim.SetTrigger("IsDead");
        rigid.simulated = false;
        GetComponent<Collider2D>().enabled = false;
        yield return new WaitForSeconds(3f);
        gameObject.SetActive(false);
    }
}