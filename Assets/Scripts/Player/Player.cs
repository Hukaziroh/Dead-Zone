using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections; // 코루틴을 위해 추가!

public class Player : MonoBehaviour
{
    Animator anim;
    public int maxHealth = 100;
    public int currentHealth;

    private void Awake()
    {
        anim = GetComponent<Animator>();
        currentHealth = maxHealth;
    }

    // 좀비가 닿을 때 호출되는 함수
    public void TakeDamage(int damage)
    {
        // 이미 죽었다면 데미지를 받지 않음
        if (currentHealth <= 0) return;

        currentHealth -= damage;
        Debug.Log("Player took damage: " + damage + ", Current HP: " + currentHealth);

        if (currentHealth <= 0)
        {
            Die();
        }
    }

  

    private void Die()
    {
        // 죽음 연출 코루틴을 시작
        StartCoroutine(DieSequence());
    }

    IEnumerator DieSequence()
    {
        Debug.Log("Player died!");

        // 1. IsDead 파라미터를 true로 바꿔서 죽음 애니메이션 재생
        anim.SetBool("IsDead", true);

        // 2. 플레이어의 모든 움직임과 충돌을 멈춤 (선택사항이지만 권장)
        // PlayerMovement 스크립트를 비활성화
        GetComponent<PlayerMovement>().enabled = false;
        // 물리적 충돌을 멈춤
        GetComponent<Collider2D>().enabled = false;

        // 3. 애니메이션이 재생될 시간 동안 대기 
        yield return new WaitForSeconds(3f);

        // 4. 게임오버 씬 로드
        SceneManager.LoadScene("GmaeOver"); 
    }
    
}