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

    // ▼▼▼ GameManager를 담을 변수 선언 ▼▼▼
    private GameManager gameManager;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        // ▼▼▼ 씬에서 GameManager를 찾아 변수에 할당 ▼▼▼
        gameManager = FindAnyObjectByType<GameManager>();
    }

    private void Update()
    {
        // ▼▼▼ gameManager 변수를 사용하여 일시 정지 상태를 확인합니다 ▼▼▼
        if (gameManager != null && gameManager.isGamePaused)
        {
            rb.linearVelocity = Vector2.zero; // 물리적인 멈춤
            anim.SetBool("IsMoving", false); // 멈춰있으니 애니메이션도 끔
            return;
        }

        AnimatorStateInfo stateInfo = anim.GetCurrentAnimatorStateInfo(0);
        isAttacking = stateInfo.IsTag("Attack");

        if (!isAttacking)
        {
            moveInput.x = Input.GetAxisRaw("Horizontal");
            moveInput.y = Input.GetAxisRaw("Vertical");
            moveInput.Normalize();

            bool isMoving = moveInput.sqrMagnitude > moveThreshold * moveThreshold;

            if (isMoving)
            {
                lastMoveDir = moveInput;
            }

            Vector3 mouseWorldPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            mouseWorldPos.z = 0;

            Vector2 mouseDir = (mouseWorldPos - transform.position).normalized;

            anim.SetFloat("MoveX", mouseDir.x);
            anim.SetFloat("MoveY", mouseDir.y);
            anim.SetBool("IsMoving", isMoving);
            anim.speed = isMoving ? moveSpeed / baseSpeed : 1f;
        }
        else
        {
            moveInput = Vector2.zero;
            anim.SetBool("IsMoving", false);
        }
    }

    private void FixedUpdate()
    {
        // ▼▼▼ gameManager 변수를 사용하여 일시 정지 상태를 확인합니다 ▼▼▼
        if (gameManager != null && gameManager.isGamePaused)
        {
            rb.linearVelocity = Vector2.zero;
            return;
        }

        rb.MovePosition(rb.position + moveInput * moveSpeed * Time.fixedDeltaTime);
    }

    public void SetMoveSpeed(float newSpeed)
    {
        moveSpeed = newSpeed;
    }
}