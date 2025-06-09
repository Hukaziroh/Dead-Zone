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
            moveInput.x = Input.GetAxisRaw("Horizontal");
            moveInput.y = Input.GetAxisRaw("Vertical");
            moveInput.Normalize();

            bool isMoving = moveInput.sqrMagnitude > moveThreshold * moveThreshold;

            if (isMoving)
            {
                lastMoveDir = moveInput;
            }

            Vector2 displayDir = isMoving ? moveInput : lastMoveDir;

            anim.SetFloat("MoveX", displayDir.x);
            anim.SetFloat("MoveY", displayDir.y);
            anim.SetBool("IsMoving", isMoving);

            anim.speed = isMoving ? moveSpeed / baseSpeed : 1f;
        }
        else
        {
            moveInput = Vector2.zero;
            anim.SetBool("IsMoving", false);
        }

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
