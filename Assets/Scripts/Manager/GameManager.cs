// GameManager.cs (최종 수정본)
using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class GameManager : MonoBehaviour
{
    public static string selectedWeaponID;

    [Header("Core Components")]
    public Player player;
    public PoolManager pool;
    public Spawner spawner;
    public UIManager uiManager;

    [Header("Game State")]
    public int wave = 1;
    public int killsThisWave = 0;
    public int level = 0;
    public bool isGamePaused = false;

    [Header("Wave Settings")]
    public int[] killsToNextWave = { 10, 20, 30, 40, 50 };
    public int bossWave = 5;
    private Boss currentBoss;

    [Header("Stat Upgrade")]
    public int attackDamageUpgradeAmount = 5;
    public float attackCooldownDecreaseAmount = 0.05f;
    public float moveSpeedUpgradeAmount = 0.5f;

    void Awake()
    {
        player = FindAnyObjectByType<Player>();
        pool = FindAnyObjectByType<PoolManager>();
        spawner = FindAnyObjectByType<Spawner>();
        uiManager = FindAnyObjectByType<UIManager>();

        isGamePaused = false;
        Time.timeScale = 1f;
    }

    void Start()
    {
        if (player != null)
        {
            player.Initialize(this);
            player.EquipWeaponByID(selectedWeaponID);
        }

        if (uiManager != null)
        {
            uiManager.Initialize(this);
            uiManager.UpdateGameHUD(wave, killsThisWave, killsToNextWave, bossWave);
        }

        if (spawner != null)
        {
            spawner.Initialize(this);
        }
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            // 업그레이드 창이나 옵션 창이 열려 있을 때는 ESC로 닫지 않음 (버튼으로만 닫도록)
            if (uiManager != null && (uiManager.IsUpgradePanelActive() || uiManager.IsOptionPanelActive())) return;

            if (isGamePaused) ResumeGame();
            else PauseGame();
        }
    }

    // 퍼즈 메뉴를 위한 일시정지 함수
    public void PauseGame()
    {
        isGamePaused = true;
        Time.timeScale = 0f;
        if (uiManager != null) uiManager.ShowPausePanel();
    }

    // 게임 재개 함수
    public void ResumeGame()
    {
        isGamePaused = false;
        Time.timeScale = 1f;
        if (uiManager != null) uiManager.HidePausePanel();
    }

    public void Restart()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void AddKill()
    {
        if (wave >= bossWave) return;
        killsThisWave++;
        if (killsThisWave >= killsToNextWave[Mathf.Min(wave - 1, killsToNextWave.Length - 1)])
        {
            NextWave();
        }
        if (uiManager != null) uiManager.UpdateGameHUD(wave, killsThisWave, killsToNextWave, bossWave);
    }

    void NextWave()
    {
        wave++;
        killsThisWave = 0;
        level = wave - 1;

        // 보스 웨이브든 아니든, 일단 업그레이드 UI를 보여주는 역할만 합니다.
        ShowUpgradeUI();

        if (uiManager != null) uiManager.UpdateGameHUD(wave, killsThisWave, killsToNextWave, bossWave);
    }

    // ▼▼▼ 이 함수를 수정합니다 ▼▼▼
    void ShowUpgradeUI()
    {
        // PauseGame() 대신, 시간만 멈추도록 직접 제어합니다.
        isGamePaused = true;
        Time.timeScale = 0f;

        string[] options = { $"Attack Up", $"Attack Cool Down", $"Speed Up" };
        if (uiManager != null) uiManager.ShowUpgradePanel(options);
    }

    public void SelectUpgradeOption(int optionIndex)
    {
        if (player == null) return;

        // 1. 선택한 능력치를 업그레이드합니다.
        switch (optionIndex)
        {
            case 0: player.UpgradeAttackDamage(attackDamageUpgradeAmount); break;
            case 1: player.UpgradeAttackCooldown(attackCooldownDecreaseAmount); break;
            case 2: player.UpgradeMoveSpeed(moveSpeedUpgradeAmount); break;
        }

        // 2. 업그레이드 UI를 닫습니다.
        if (uiManager != null) uiManager.HideUpgradePanel();

        // 3. 업그레이드가 끝난 후, 보스 웨이브인지 확인합니다.
        if (wave == bossWave)
        {
            // 보스 웨이브가 맞다면, 여기서 보스를 소환합니다.
            if (spawner != null)
            {
                spawner.StopSpawning();
                spawner.SpawnBoss();
            }
            // 보스전 시작을 위해 게임을 재개합니다.
            isGamePaused = false;
            Time.timeScale = 1f;
        }
        else
        {
            // 일반 웨이브라면 평소처럼 게임을 재개합니다.
            ResumeGame();
        }
    }

    // --- (이하 Boss 관련 함수들은 그대로 유지) ---
    public void ShowBossHealthBar(Boss boss)
    {
        currentBoss = boss;
        if (uiManager != null) uiManager.ShowBossHealthBar(boss);
    }
    public void UpdateBossHealth()
    {
        if (uiManager != null) uiManager.UpdateBossHealthBar();
    }
    public void BossDied()
    {
        if (uiManager != null) uiManager.HideBossHealthBar();
        StartCoroutine(GoToClearSceneAfterDelay(3f));
    }
    IEnumerator GoToClearSceneAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        Time.timeScale = 1f;
        SceneManager.LoadScene("Clear");
    }
}