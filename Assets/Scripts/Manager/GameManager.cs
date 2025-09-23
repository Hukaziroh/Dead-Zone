using UnityEngine;
// using TMPro; // UIManager가 처리하므로 불필요 (기존 코드 주석 유지)
// using UnityEngine.UI; // UIManager가 처리하므로 불필요 (기존 코드 주석 유지)

public class GameManager : MonoBehaviour
{
    public static GameManager instance;
    public Spawner spawner;

    [Header("# Player Info")]
    public Player player;

    [Header("# Game Object")]
    public PoolManager pool; // 기존 코드의 PoolManager 사용
    public Collider2D Bound;

    [Header("# Wave System")]
    private int wave = 1;
    private int killsThisWave = 0;
    private int[] killsToNextWave = { 4, 4, 4, 4, 1 }; // 기존 코드의 killsToNextWave 유지
    private Boss currentBoss;
    public int bossWave = 5;

    [Header("Pause & Game State")]
    public bool isGamePaused = false;

    // ▼▼▼ isGamePaused 상태를 외부에 알려주는 public 함수 ▼▼▼ (기존 코드 유지)
    public bool GetIsGamePaused()
    {
        return isGamePaused;
    }

    // ▼▼▼ 스탯 업그레이드 관련 변수 추가 ▼▼▼
    [Header("# Stat Upgrade")]
    public int attackDamageUpgradeAmount = 10;       // 공격력 강화량
    public float attackCooldownDecreaseAmount = 0.2f; // 공격 속도 강화량 (쿨타임 감소량)
    public float moveSpeedUpgradeAmount = 0.5f;       // 이동 속도 강화량

    public string[] upgradeOptions = new string[3]; // UIManager에 넘겨줄 업그레이드 옵션 텍스트


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
        // instance = this; // 기존 코드 주석대로 중복이므로 제거
    }

    void Start()
    {
        level = 0;
        // UIManager가 GameScene 로드 시 초기화를 담당하게 되므로, 여기 Start에서는 HUD 업데이트만 호출
        if (UIManager.instance != null)
        {
            UIManager.instance.UpdateGameHUD(wave, killsThisWave, killsToNextWave, bossWave);
        }
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            // UpgradePanel이 활성화 중일 때는 Escape 키로 닫지 않음
            if (UIManager.instance != null && UIManager.instance.IsUpgradePanelActive())
            {
                return;
            }

            if (isGamePaused)
            {
                // PausePanel이 열려있을 때만 Resume
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

    public void Restart() // 기존 Restart 함수 이름 유지
    {
        ResumeGame();
        // 씬 재로드 로직 추가 (필요 시)
        // UnityEngine.SceneManagement.SceneManager.LoadScene(UnityEngine.SceneManagement.SceneManager.GetActiveScene().name);
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

        if (wave >= bossWave) return; // 보스 웨이브 이후에는 킬 카운트 멈춤

        killsThisWave++;
        if (killsThisWave >= killsToNextWave[Mathf.Min(wave - 1, killsToNextWave.Length - 1)]) // 배열 범위 체크
        {
            NextWave();
        }
        // UIManager를 통해 UI 업데이트 호출
        if (UIManager.instance != null)
        {
            UIManager.instance.UpdateGameHUD(wave, killsThisWave, killsToNextWave, bossWave);
        }
    }

    void NextWave()
    {
        wave++;
        killsThisWave = 0;

        level = wave - 1; // 몬스터 스폰 레벨 조절용

        // 보스 웨이브가 아닐 때만 스탯 업그레이드 UI 표시
        if (wave == bossWave)
        {
            if (spawner != null) spawner.StopSpawning();
            if (spawner != null) spawner.SpawnBoss();
        }
        else // 일반 웨이브 진행 시 스탯 업그레이드 UI 표시
        {
            ShowUpgradeUI(); // ▼▼▼ 스탯 업그레이드 UI 표시 ▼▼▼
        }

        // UIManager를 통해 UI 업데이트 호출
        if (UIManager.instance != null)
        {
            UIManager.instance.UpdateGameHUD(wave, killsThisWave, killsToNextWave, bossWave);
        }
        Debug.Log("WAVE " + wave + " START!");
    }

    // Boss.cs의 OnEnable에서 호출 (기존 코드 유지)
    public void ShowBossHealthBar(Boss boss)
    {
        currentBoss = boss;
        // UIManager를 통해 보스 체력바 활성화 및 업데이트 호출
        if (UIManager.instance != null)
        {
            UIManager.instance.ShowBossHealthBar(boss);
        }
    }

    public void UpdateBossHealth() // 기존 코드 유지
    {
        // UIManager를 통해 보스 체력바 업데이트 호출
        if (UIManager.instance != null)
        {
            UIManager.instance.UpdateBossHealthBar();
        }
    }

    // Boss.cs의 Die에서 호출 (기존 코드 유지)
    public void BossDied()
    {
        // UIManager를 통해 보스 체력바 비활성화 호출
        if (UIManager.instance != null)
        {
            UIManager.instance.HideBossHealthBar();
        }
        Debug.Log("BOSS KILLED! GAME CLEAR!");
    }

    public void PlayerDied() // 기존 코드 유지
    {
        PauseGame(); // 플레이어 사망 시 게임 일시 정지
    }

    public int level; // 몬스터 스폰 난이도 조절용 (기존 코드 유지)


    // ▼▼▼ 스탯 업그레이드 UI를 띄우는 함수 추가 ▼▼▼
    void ShowUpgradeUI()
    {
        PauseGame(); // 스탯 선택 중에는 게임 일시 정지

        // 업그레이드 옵션 텍스트 준비
        upgradeOptions[0] = $"공격력 증가 (+{attackDamageUpgradeAmount})";
        upgradeOptions[1] = $"공격 속도 증가 (쿨타임 -{attackCooldownDecreaseAmount:F1}s)"; // 소수점 한 자리 표시
        upgradeOptions[2] = $"이동 속도 증가 (+{moveSpeedUpgradeAmount})";

        if (UIManager.instance != null)
        {
            UIManager.instance.ShowUpgradePanel(upgradeOptions); // UIManager에 업그레이드 패널 표시 요청
        }
    }

    // ▼▼▼ UIManager의 버튼 클릭 시 호출될 함수들 (플레이어 스탯 강화) 추가 ▼▼▼
    public void SelectUpgradeOption(int optionIndex)
    {
        if (player == null)
        {
            Debug.LogError("Player 스크립트가 GameManager에 할당되지 않았습니다!");
            return;
        }

        switch (optionIndex)
        {
            case 0: // 공격력
                player.UpgradeAttackDamage(attackDamageUpgradeAmount);
                break;
            case 1: // 공격 속도 (쿨타임 감소)
                player.UpgradeAttackCooldown(attackCooldownDecreaseAmount);
                break;
            case 2: // 이동 속도
                player.UpgradeMoveSpeed(moveSpeedUpgradeAmount);
                break;
            default:
                Debug.LogError("잘못된 업그레이드 옵션 인덱스: " + optionIndex);
                break;
        }

        // 업그레이드 선택 후 게임 재개 및 UI 숨김
        if (UIManager.instance != null)
        {
            UIManager.instance.HideUpgradePanel();
        }
        ResumeGame(); // 스탯 선택 완료 후 게임 재개
    }
}