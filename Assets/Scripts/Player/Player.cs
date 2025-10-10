using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class Player : MonoBehaviour
{
    Animator anim;
    public int maxHealth = 100;
    public int currentHealth;

    private PlayerMovement playerMovement;
    private PlayerAttack playerAttack;
    private GameManager gameManager;

    public void Initialize(GameManager gm)
    {
        gameManager = gm;
    }

    private void Awake()
    {
        anim = GetComponent<Animator>();
        currentHealth = maxHealth;
        playerMovement = GetComponent<PlayerMovement>();
        playerAttack = GetComponent<PlayerAttack>();
    }

    public void EquipWeaponByID(string weaponID)
    {
        if (string.IsNullOrEmpty(weaponID))
        {
            weaponID = "Pistol";
        }

        WeaponData selectedWeapon = Resources.Load<WeaponData>($"WeaponData/{weaponID}");

        if (selectedWeapon != null && playerAttack != null)
        {
            playerAttack.EquipWeapon(Instantiate(selectedWeapon));
        }
        else
        {
            Debug.LogError($"[Player] '{weaponID}' 무기 데이터를 찾을 수 없습니다!");
        }
    }

    public void TakeDamage(int damage)
    {
        if (currentHealth <= 0) return;
        currentHealth -= damage;
        if (currentHealth <= 0) Die();
    }

    private void Die()
    {
        StartCoroutine(DieSequence());
    }

    IEnumerator DieSequence()
    {
        anim.SetBool("IsDead", true);
        playerMovement.enabled = false;
        playerAttack.enabled = false;
        GetComponent<Collider2D>().enabled = false;
        yield return new WaitForSeconds(3f);
        SceneManager.LoadScene("GmaeOver");
    }

    public void UpgradeAttackDamage(int amount)
    {
        if (playerAttack != null) playerAttack.UpgradeDamage(amount);
    }

    public void UpgradeAttackCooldown(float amount)
    {
        if (playerAttack != null) playerAttack.UpgradeCooldown(amount);
    }

    public void UpgradeMoveSpeed(float amount)
    {
        if (playerMovement != null) playerMovement.moveSpeed += amount;
    }
}