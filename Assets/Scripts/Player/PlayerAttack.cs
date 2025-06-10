using UnityEngine;

[RequireComponent(typeof(Animator))]
public class PlayerAttack : MonoBehaviour
{
    Animator anim;
    Camera cam;

    void Awake()
    {
        anim = GetComponent<Animator>();
        cam = Camera.main;
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            if (!IsInAttackState())
            {
                Vector3 mouseWorldPos = cam.ScreenToWorldPoint(Input.mousePosition);
                mouseWorldPos.z = 0;

                Vector2 dir = (mouseWorldPos - transform.position).normalized;

                Debug.Log(dir);
                anim.SetFloat("AttackX", dir.x);
                anim.SetFloat("AttackY", dir.y);
                anim.SetTrigger("IsAttack");
            }
        }
    }

    private bool IsInAttackState()
    {
        return anim.GetCurrentAnimatorStateInfo(0).IsTag("Attack");
    }
}
