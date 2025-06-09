using UnityEngine;

public class Enemy : MonoBehaviour
{

    public float speed;
    public float health;
    public float maxHealth;

    public Rigidbody2D target;
    public RuntimeAnimatorController[] animCon;

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
        wait = new WaitForFixedUpdate();
        coll = GetComponent<Collider2D>();
    }
    void Start()
    {

    }

    private void FixedUpdate()
    {
        Vector2 dirVec = target.position - rigid.position;
        Vector2 nextVec = dirVec.normalized * speed * Time.fixedDeltaTime;

        // �ִϸ��̼� ���� ����
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

        // �̵� ó��
        rigid.MovePosition(rigid.position + nextVec);
        rigid.linearVelocity = Vector2.zero;
    }


}

