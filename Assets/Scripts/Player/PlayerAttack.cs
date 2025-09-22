// PlayerAttack.cs (수정본)

using UnityEngine;

[RequireComponent(typeof(Animator))]
public class PlayerAttack : MonoBehaviour
{
    Animator anim;
    Camera cam;

    [Header("Attack Settings")]
    public int bulletPoolIndex = 4;
    public Transform playerCenterPoint;

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
    }

    void Update()
    {
        // ▼▼▼ 게임이 일시 정지 상태인지 먼저 확인 ▼▼▼
        // GameManager.instance.isGamePaused 대신, GameManager에 isGamePaused 상태를 반환하는 public 함수를 만드는 것이 좋습니다.
        // 현재 GameManager의 isGamePaused가 private이므로, 아래와 같이 변경합니다.

        // GameManager에 IsGamePaused() public getter 추가를 권장합니다.
        // 예를 들어 GameManager.cs에:
        // public bool IsGamePaused() { return isGamePaused; }
        // 추가 후 여기서는 GameManager.instance.IsGamePaused()로 호출.

        // 하지만 임시로 GameManager.instance.isGamePaused 필드에 직접 접근하거나
        // 아니면 UIManager의 pausePanel.activeSelf를 확인하는 방법도 있습니다.
        // 가장 안전하고 좋은 방법은 GameManager에 public getter를 추가하는 것입니다.
        if (GameManager.instance != null && GameManager.instance.isGamePaused) // 임시로 private 필드에 접근한다고 가정
        {
            return; // 게임이 일시 정지 상태면 공격 로직을 실행하지 않고 바로 Update 종료
        }

        // (더 좋은 방법: GameManager.cs에 다음 함수 추가 후 사용)
        // public bool GetIsGamePaused() { return isGamePaused; }
        // 그리고 이 스크립트에서는:
        // if (GameManager.instance != null && GameManager.instance.GetIsGamePaused())
        // {
        //     return;
        // }


        if (Input.GetMouseButtonDown(0))
        {
            if (!IsInAttackState())
            {
                Vector3 mouseWorldPos = cam.ScreenToWorldPoint(Input.mousePosition);
                mouseWorldPos.z = 0;
                Vector2 dir = (mouseWorldPos - transform.position).normalized;

                anim.SetTrigger("IsAttack");

                Vector2 currentOffset = CalculateMuzzleOffset(dir);
                Vector3 finalMuzzleWorldPosition = (Vector2)playerCenterPoint.position + currentOffset;

                FireBullet(dir, finalMuzzleWorldPosition);
            }
        }
    }

    private Vector2 CalculateMuzzleOffset(Vector2 mouseDir)
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

    void FireBullet(Vector2 direction, Vector3 muzzleWorldPosition)
    {
        GameObject bulletObject = GameManager.instance.pool.Get(bulletPoolIndex);
        bulletObject.transform.SetParent(null);
        bulletObject.transform.position = muzzleWorldPosition;

        Bullet bulletScript = bulletObject.GetComponent<Bullet>();
        if (bulletScript != null)
        {
            bulletScript.Init(direction);
        }
        else
        {
            Debug.LogError("총알에서 Bullet.cs 스크립트를 찾을 수 없습니다!");
        }
    }

    private bool IsInAttackState()
    {
        return anim.GetCurrentAnimatorStateInfo(0).IsTag("Attack");
    }
}