using Unity.VisualScripting;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    public float speed;
    public float health;
    public float maxHealth;

    public Rigidbody2D target;
    public RuntimeAnimatorController[] animCon;

    // 추가: 카메라 바운드 콜라이더 참조 (씬에서 할당)
    public Collider2D cameraBoundsCollider;

    bool isLive;
    Collider2D coll;
    Rigidbody2D rigid;
    Animator anim;
    SpriteRenderer spriter;
    WaitForFixedUpdate wait;

    private void Awake()
    {
        rigid = GetComponent<Rigidbody2D>();
        spriter = GetComponent<SpriteRenderer>();
        anim = GetComponent<Animator>();
        coll = GetComponent<Collider2D>();
        wait = new WaitForFixedUpdate();
    }

    private void FixedUpdate()
    {
        Vector2 dirVec = target.position - rigid.position;
        Vector2 nextVec = dirVec.normalized * speed * Time.fixedDeltaTime;
        Vector2 targetPos = rigid.position + nextVec;

        if (cameraBoundsCollider != null)
        {
            if (cameraBoundsCollider.OverlapPoint(targetPos))
            {
                rigid.MovePosition(targetPos);
            }
            
        }
        else
        {
            rigid.MovePosition(targetPos);
        }

        if (dirVec.sqrMagnitude > 0.01f)
        {
            anim.SetFloat("MoveX", dirVec.normalized.x);
            anim.SetFloat("MoveY", dirVec.normalized.y);
            anim.SetBool("IsMoving", true);
        }
        else
        {
            anim.SetBool("IsMoving", false);
        }

        rigid.linearVelocity = Vector2.zero;
    }

    private void OnEnable()
    {
        target = GameManager.instance.player.GetComponent<Rigidbody2D>();
        cameraBoundsCollider = GameManager.instance.Bound.GetComponent<Collider2D>();
    }
}
