using UnityEngine;

[RequireComponent(typeof(Animator))]
public class PlayerAttack : MonoBehaviour
{
    Animator anim;

    void Awake()
    {
        anim = GetComponent<Animator>();
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            // 애니메이터가 Attack 중이 아니면 실행
            if (!IsInAttackState())
            {
                anim.SetTrigger("IsAttacking");
            }
        }
    }

    private bool IsInAttackState()
    {
        AnimatorStateInfo stateInfo = anim.GetCurrentAnimatorStateInfo(0);
        return stateInfo.IsName("Attack"); // Attack Blend Tree 상태 이름 정확히 확인
    }
}
