using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(Animator))]
public class PlayerMovement : MonoBehaviour
{
    public float moveSpeed = 5f; // Player.cs에서 이 값을 설정합니다. (초기값은 기본값)
    public float moveThreshold = 0.05f;
    public float baseSpeed = 5f;

    private bool isAttacking = false;

    Rigidbody2D rb;
    Animator anim;
    Vector2 moveInput;
    Vector2 lastMoveDir;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        // moveSpeed의 초기값은 Player.cs의 Awake에서 SetMoveSpeed를 통해 설정될 것입니다.
    }

    private void Update()
    {
        // ▼▼▼ 게임 일시 정지 상태일 경우 이동 로직을 실행하지 않음 ▼▼▼
        if (GameManager.instance != null && GameManager.instance.GetIsGamePaused())
        {
            rb.linearVelocity = Vector2.zero; // 물리적인 멈춤
            anim.SetBool("IsMoving", false); // 멈춰있으니 애니메이션도 끔
            return;
        }

        AnimatorStateInfo stateInfo = anim.GetCurrentAnimatorStateInfo(0);
        isAttacking = stateInfo.IsTag("Attack");

        if (!isAttacking)
        {
            // 이동 입력 처리
            moveInput.x = Input.GetAxisRaw("Horizontal");
            moveInput.y = Input.GetAxisRaw("Vertical");
            moveInput.Normalize();

            bool isMoving = moveInput.sqrMagnitude > moveThreshold * moveThreshold;

            if (isMoving)
            {
                lastMoveDir = moveInput;
            }

            // 마우스 월드 좌표 계산
            Vector3 mouseWorldPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            mouseWorldPos.z = 0;

            // 플레이어 -> 마우스 방향 벡터 (Normalize)
            Vector2 mouseDir = (mouseWorldPos - transform.position).normalized;

            // 애니메이터 파라미터에 마우스 방향 넣기 (시점용)
            anim.SetFloat("MoveX", mouseDir.x);
            anim.SetFloat("MoveY", mouseDir.y);

            // 이동 중 여부 설정
            anim.SetBool("IsMoving", isMoving);

            // 애니메이션 재생 속도 조절 (이동 속도 기반)
            anim.speed = isMoving ? moveSpeed / baseSpeed : 1f;
        }
        else
        {
            moveInput = Vector2.zero; // 공격 중에는 이동 입력 무시
            anim.SetBool("IsMoving", false); // 공격 중에는 이동 애니메이션 끔
        }
    }

    private void FixedUpdate()
    {
        // ▼▼▼ 게임 일시 정지 상태일 경우 이동 로직을 실행하지 않음 ▼▼▼
        if (GameManager.instance != null && GameManager.instance.GetIsGamePaused())
        {
            rb.linearVelocity = Vector2.zero;
            return;
        }

        // 실제 이동 처리
        rb.MovePosition(rb.position + moveInput * moveSpeed * Time.fixedDeltaTime);
    }

    private bool IsAttacking() // 이 함수는 현재 Update에서 isAttacking 변수로 대체되어 사용되지 않습니다.
    {
        return anim.GetCurrentAnimatorStateInfo(0).IsTag("Attack");
    }

    // ▼▼▼ Player.cs에서 호출하여 이동 속도를 설정하는 함수 추가 ▼▼▼
    public void SetMoveSpeed(float newSpeed)
    {
        moveSpeed = newSpeed;
    }
}