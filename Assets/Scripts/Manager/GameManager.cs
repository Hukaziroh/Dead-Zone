using UnityEngine;
using UnityEngine.SceneManagement; // 씬 관리를 위해 필수!
using System.Collections;
public class GameManager : MonoBehaviour
{
    public static GameManager instance;
    public Spawner spawner;

    [Header("# Player Info")]
    public Player player;

    [Header("# Game Object")]
    public PoolManager pool;
    public Collider2D Bound;

    [Header("# Wave System")]
    private int wave = 1;
    private int killsThisWave = 0;
    private int[] killsToNextWave = { 1, 1, 1, 1, 1 };
    private Boss currentBoss;
    public int bossWave = 5;

    [Header("Pause & Game State")]
    public bool isGamePaused = false;

    [Header("# Stat Upgrade")]
    public int attackDamageUpgradeAmount = 10;
    public float attackCooldownDecreaseAmount = 0.2f;
    public float moveSpeedUpgradeAmount = 0.5f;

    public string[] upgradeOptions = new string[3];

    public int level; // Spawner가 사용할 레벨 변수


    void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void Start()
    {
        level = 0;
        if (UIManager.instance != null)
        {
            UIManager.instance.UpdateGameHUD(wave, killsThisWave, killsToNextWave, bossWave);
        }
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (UIManager.instance != null && UIManager.instance.IsUpgradePanelActive())
            {
                return;
            }

            if (isGamePaused)
            {
                if (UIManager.instance != null && UIManager.instance.IsPausePanelActive())
                {
                    ResumeGame();
                }
            }
            else
            {
                PauseGame();
            }
        }
    }

    public void Restart()
    {
        ResumeGame();
    }

    public void PauseGame()
    {
        isGamePaused = true;
        Time.timeScale = 0f;
        if (UIManager.instance != null)
        {
            UIManager.instance.ShowPausePanel();
        }
        Debug.Log("Game Paused!");
    }

    public void ResumeGame()
    {
        isGamePaused = false;
        Time.timeScale = 1f;
        if (UIManager.instance != null)
        {
            UIManager.instance.HidePausePanel();
        }
        Debug.Log("Game Resumed!");
    }

    public void AddKill()
    {
        Debug.Log("GameManager.AddKill() 호출됨! 현재 웨이브: " + wave + ", 현재 킬: " + killsThisWave);

        if (wave >= bossWave) return;

        killsThisWave++;
        if (killsThisWave >= killsToNextWave[Mathf.Min(wave - 1, killsToNextWave.Length - 1)])
        {
            NextWave();
        }
        if (UIManager.instance != null)
        {
            UIManager.instance.UpdateGameHUD(wave, killsThisWave, killsToNextWave, bossWave);
        }
    }

    void NextWave()
    {
        wave++;
        killsThisWave = 0;

        level = wave - 1;

        if (wave == bossWave)
        {
            if (spawner != null) spawner.StopSpawning();
            if (spawner != null) spawner.SpawnBoss();
        }

        ShowUpgradeUI();

        if (UIManager.instance != null)
        {
            UIManager.instance.UpdateGameHUD(wave, killsThisWave, killsToNextWave, bossWave);
        }
        Debug.Log("WAVE " + wave + " START!");
    }

    public void ShowBossHealthBar(Boss boss)
    {
        currentBoss = boss;
        if (UIManager.instance != null)
        {
            UIManager.instance.ShowBossHealthBar(boss);
        }
    }

    public void UpdateBossHealth()
    {
        if (UIManager.instance != null)
        {
            UIManager.instance.UpdateBossHealthBar();
        }
    }

    // Boss.cs의 Die에서 호출 (수정됨)
    public void BossDied()
    {
        if (UIManager.instance != null)
        {
            UIManager.instance.HideBossHealthBar();
        }
        Debug.Log("BOSS KILLED! GAME CLEAR!");

        // ▼▼▼ 여기에 보스 사망 후 씬 전환 코루틴 시작 ▼▼▼
        StartCoroutine(GoToClearSceneAfterDelay(3f));
    }

    // ▼▼▼ 보스 사망 후 씬 전환을 위한 코루틴 추가 ▼▼▼
    IEnumerator GoToClearSceneAfterDelay(float delay)
    {
        // 3초 동안 대기
        yield return new WaitForSeconds(delay);

        // 혹시 모를 상황을 대비해 게임 시간 정상화
        Time.timeScale = 1f;

        // "Clear" 씬으로 이동 (씬 이름이 다르다면 Inspector에서 수정하거나 여기서 직접 변경)
        SceneManager.LoadScene("Clear");
    }

    public void PlayerDied()
    {
        PauseGame();
    }

    public bool GetIsGamePaused()
    {
        return isGamePaused;
    }

    void ShowUpgradeUI()
    {
        PauseGame();

        upgradeOptions[0] = $"공격력 증가 (+{attackDamageUpgradeAmount})";
        upgradeOptions[1] = $"공격 속도 증가 (쿨타임 -{attackCooldownDecreaseAmount:F1}s)";
        upgradeOptions[2] = $"이동 속도 증가 (+{moveSpeedUpgradeAmount})";

        if (UIManager.instance != null)
        {
            UIManager.instance.ShowUpgradePanel(upgradeOptions);
        }
    }

    public void SelectUpgradeOption(int optionIndex)
    {
        if (player == null)
        {
            Debug.LogError("Player 스크립트가 GameManager에 할당되지 않았습니다!");
            return;
        }

        switch (optionIndex)
        {
            case 0:
                player.UpgradeAttackDamage(attackDamageUpgradeAmount);
                break;
            case 1:
                player.UpgradeAttackCooldown(attackCooldownDecreaseAmount);
                break;
            case 2:
                player.UpgradeMoveSpeed(moveSpeedUpgradeAmount);
                break;
            default:
                Debug.LogError("잘못된 업그레이드 옵션 인덱스: " + optionIndex);
                break;
        }

        if (UIManager.instance != null)
        {
            UIManager.instance.HideUpgradePanel();
        }
        ResumeGame();
    }
}