using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class Player : MonoBehaviour
{
    Animator anim;
    public int maxHealth = 100;
    public int currentHealth;

    // ▼▼▼ 스탯 업그레이드 시스템을 위해 추가된 변수들 ▼▼▼
    // 이 변수들이 이제 플레이어의 실제 스탯을 저장하고, 다른 스크립트에 전달됩니다.
    public int currentAttackDamage = 10;
    public float currentAttackCooldown = 0.5f; // 공격 쿨타임 (낮을수록 빠름)
    public float currentMoveSpeed = 5f;

    // 다른 스크립트 참조
    private PlayerMovement playerMovement;
    private PlayerAttack playerAttack;


    private void Awake()
    {
        anim = GetComponent<Animator>();
        currentHealth = maxHealth;

        // 다른 스크립트 참조 가져오기
        playerMovement = GetComponent<PlayerMovement>();
        playerAttack = GetComponent<PlayerAttack>();

        // 게임 시작 시, PlayerMovement와 PlayerAttack 스크립트에 현재 스탯 값을 적용합니다.
        ApplyCurrentStats();
    }

    // 게임 시작 시 또는 스탯이 변경될 때, 관련 스크립트에 현재 스탯을 적용하는 함수
    void ApplyCurrentStats()
    {
        if (playerMovement != null)
        {
            playerMovement.SetMoveSpeed(currentMoveSpeed);
        }
        else
        {
            Debug.LogWarning("PlayerMovement 스크립트를 찾을 수 없습니다. 이동 속도 적용 실패.");
        }

        if (playerAttack != null)
        {
            playerAttack.SetAttackDamage(currentAttackDamage);
            playerAttack.SetAttackCooldown(currentAttackCooldown);
        }
        else
        {
            Debug.LogWarning("PlayerAttack 스크립트를 찾을 수 없습니다. 공격 스탯 적용 실패.");
        }
    }

    // 좀비가 닿을 때 호출되는 함수 (기존 로직 유지)
    public void TakeDamage(int damage)
    {
        // 이미 죽었다면 데미지를 받지 않음
        if (currentHealth <= 0) return;

        currentHealth -= damage;
        Debug.Log("Player took damage: " + damage + ", Current HP: " + currentHealth);

        // UIManager가 플레이어 체력 UI를 업데이트해야 한다면 여기서 UIManager 함수 호출
        // UIManager.instance.UpdatePlayerHealthUI(currentHealth, maxHealth);

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    private void Die() // 기존 로직 유지
    {
        StartCoroutine(DieSequence());
    }

    IEnumerator DieSequence() // 기존 로직 유지
    {
        Debug.Log("Player died!");

        anim.SetBool("IsDead", true);

        GetComponent<PlayerMovement>().enabled = false;
        GetComponent<Collider2D>().enabled = false;
        GetComponent<PlayerAttack>().enabled = false;

        yield return new WaitForSeconds(3f);

        SceneManager.LoadScene("GmaeOver");
    }

    // ▼▼▼ GameManager에서 호출할 스탯 업그레이드 함수들 추가 ▼▼▼

    // 공격력 스탯을 강화합니다.
    public void UpgradeAttackDamage(int amount)
    {
        currentAttackDamage += amount;
        if (playerAttack != null)
        {
            playerAttack.SetAttackDamage(currentAttackDamage);
        }
        Debug.Log($"공격력 업그레이드: {currentAttackDamage}");
    }

    // 공격 쿨타임(공격 속도) 스탯을 강화합니다. (쿨타임이 줄어듦)
    public void UpgradeAttackCooldown(float amount)
    {
        // 쿨타임을 amount만큼 줄이고, 최소 쿨타임(0.1f) 이하로는 내려가지 않도록 합니다.
        currentAttackCooldown = Mathf.Max(0.1f, currentAttackCooldown - amount);
        if (playerAttack != null)
        {
            playerAttack.SetAttackCooldown(currentAttackCooldown);
        }
        Debug.Log($"공격 속도 업그레이드 (쿨타임): {currentAttackCooldown}");
    }

    // 이동 속도 스탯을 강화합니다.
    public void UpgradeMoveSpeed(float amount)
    {
        currentMoveSpeed += amount;
        if (playerMovement != null)
        {
            playerMovement.SetMoveSpeed(currentMoveSpeed);
        }
        Debug.Log($"이동 속도 업그레이드: {currentMoveSpeed}");
    }
}