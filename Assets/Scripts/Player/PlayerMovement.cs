using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(Animator))]
public class PlayerMovement : MonoBehaviour
{
    public float moveSpeed = 5f;
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
    }

    private void Update()
    {
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
            moveInput = Vector2.zero;
            anim.SetBool("IsMoving", false);
        }

        // 공격 입력 처리
        if (Input.GetMouseButtonDown(0) && !isAttacking)
        {
            anim.SetTrigger("IsAttack");
        }
    }

    private void FixedUpdate()
    {
        rb.MovePosition(rb.position + moveInput * moveSpeed * Time.fixedDeltaTime);
    }

    private bool IsAttacking()
    {
        return anim.GetCurrentAnimatorStateInfo(0).IsTag("Attack");
    }
}
