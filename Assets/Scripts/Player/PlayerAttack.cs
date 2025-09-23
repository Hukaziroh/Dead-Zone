// PlayerAttack.cs (수정된 최종 버전)

using UnityEngine;

[RequireComponent(typeof(Animator))]
public class PlayerAttack : MonoBehaviour
{
    Animator anim;
    Camera cam;

    [Header("Attack Settings")]
    public int bulletPoolIndex = 4;
    public Transform playerCenterPoint;

    // ▼▼▼ Player.cs에서 값을 받아올 변수들 (초기값은 기본값) ▼▼▼
    // 이 변수들이 실제 공격 로직에 사용됩니다.
    public int currentAttackDamage = 10;           // 공격력
    public float currentAttackCooldown = 0.5f;     // 공격 쿨타임 (낮을수록 빠름)
    private float lastAttackTime; // 마지막으로 공격한 시간


    [Header("Muzzle Offsets (Relative to Player Center)")]
    public Vector2 offsetUp = new Vector2(0.0f, 0.5f);
    public Vector2 offsetDown = new Vector2(0.0f, -0.5f);
    public Vector2 offsetLeft = new Vector2(-0.5f, 0.1f);
    public Vector2 offsetRight = new Vector2(0.5f, 0.1f);
    public Vector2 offsetUpRight = new Vector2(0.35f, 0.35f);
    public Vector2 offsetUpLeft = new Vector2(-0.35f, 0.35f);
    public Vector2 offsetDownRight = new Vector2(0.35f, -0.35f);
    public Vector2 offsetDownLeft = new Vector2(-0.35f, -0.35f);

    void Awake()
    {
        anim = GetComponent<Animator>();
        cam = Camera.main;
        playerCenterPoint = this.transform;

        // Player.cs에서 이 스크립트의 SetAttackDamage/SetAttackCooldown을 호출하여 초기 값을 설정할 것입니다.
        // 따라서 여기서 직접 초기화는 필요 없거나 Player.cs가 호출하기 전에 사용될 경우 기본값을 설정합니다.
        lastAttackTime = -currentAttackCooldown; // 게임 시작 시 바로 공격 가능하도록
    }

    void Update()
    {
        // ▼▼▼ GameManager의 일시 정지 상태 체크 (isGamePaused는 GameManager에 public bool GetIsGamePaused() 함수로 접근) ▼▼▼
        if (GameManager.instance != null && GameManager.instance.GetIsGamePaused())
        {
            return; // 게임이 일시 정지 상태면 공격 로직을 실행하지 않고 바로 Update 종료
        }

        if (Input.GetMouseButtonDown(0))
        {
            // 공격 애니메이션 중이 아니고, 쿨타임이 지났을 때만 공격
            if (!IsInAttackState() && Time.time >= lastAttackTime + currentAttackCooldown)
            {
                Vector3 mouseWorldPos = cam.ScreenToWorldPoint(Input.mousePosition);
                mouseWorldPos.z = 0;
                Vector2 dir = (mouseWorldPos - transform.position).normalized;

                anim.SetTrigger("IsAttack");
                lastAttackTime = Time.time; // 공격 시작 시간 기록

                Vector2 currentOffset = CalculateMuzzleOffset(dir);
                Vector3 finalMuzzleWorldPosition = (Vector2)playerCenterPoint.position + currentOffset;

                FireBullet(dir, finalMuzzleWorldPosition);
            }
        }
    }

    private Vector2 CalculateMuzzleOffset(Vector2 mouseDir) // 기존 로직 유지
    {
        float angle = Vector2.SignedAngle(Vector2.right, mouseDir);
        if (angle < 0) angle += 360;

        if (angle >= 337.5f || angle < 22.5f) return offsetRight;
        else if (angle >= 22.5f && angle < 67.5f) return offsetUpRight;
        else if (angle >= 67.5f && angle < 112.5f) return offsetUp;
        else if (angle >= 112.5f && angle < 157.5f) return offsetUpLeft;
        else if (angle >= 157.5f && angle < 202.5f) return offsetLeft;
        else if (angle >= 202.5f && angle < 247.5f) return offsetDownLeft;
        else if (angle >= 247.5f && angle < 292.5f) return offsetDown;
        else if (angle >= 292.5f && angle < 337.5f) return offsetDownRight;

        return offsetRight;
    }

    void FireBullet(Vector2 direction, Vector3 muzzleWorldPosition) // 기존 로직 유지
    {
        GameObject bulletObject = GameManager.instance.pool.Get(bulletPoolIndex);
        bulletObject.transform.SetParent(null);
        bulletObject.transform.position = muzzleWorldPosition;

        Bullet bulletScript = bulletObject.GetComponent<Bullet>();
        if (bulletScript != null)
        {
            bulletScript.Init(direction);
            bulletScript.damage = currentAttackDamage; // ▼▼▼ 총알에 현재 공격력 전달 ▼▼▼
        }
        else
        {
            Debug.LogError("총알에서 Bullet.cs 스크립트를 찾을 수 없습니다!");
        }
    }

    private bool IsInAttackState() // 기존 로직 유지
    {
        return anim.GetCurrentAnimatorStateInfo(0).IsTag("Attack");
    }

    // ▼▼▼ Player.cs에서 호출하여 공격력을 설정하는 함수 추가 ▼▼▼
    public void SetAttackDamage(int newDamage)
    {
        currentAttackDamage = newDamage;
    }

    // ▼▼▼ Player.cs에서 호출하여 공격 쿨타임을 설정하는 함수 추가 ▼▼▼
    public void SetAttackCooldown(float newCooldown)
    {
        currentAttackCooldown = newCooldown;
    }
}