using UnityEngine;
// using TMPro; // UIManager가 처리하므로 불필요
// using UnityEngine.UI; // UIManager가 처리하므로 불필요

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
    private int[] killsToNextWave = { 4, 4, 4, 4, 1 };
    private Boss currentBoss;
    public int bossWave = 5;

    [Header("Pause & Game State")]
    public bool isGamePaused = false;
   
    // ▼▼▼ isGamePaused 상태를 외부에 알려주는 public 함수 ▼▼▼
    public bool GetIsGamePaused()
    {
        return isGamePaused;
    }
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
        // instance = this; // 중복이므로 위의 if 블록 안에서만 처리
    }

    void Start()
    {
        level = 0;
        // ▼▼▼ UIManager를 통해 UI 업데이트 호출 ▼▼▼
        if (UIManager.instance != null)
        {
            UIManager.instance.UpdateGameHUD(wave, killsThisWave, killsToNextWave, bossWave);
        }
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (isGamePaused)
            {
                ResumeGame();
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
        if (wave >= bossWave) return;

        killsThisWave++;
        if (killsThisWave >= killsToNextWave[wave - 1])
        {
            NextWave();
        }
        // ▼▼▼ UIManager를 통해 UI 업데이트 호출 ▼▼▼
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
            spawner.StopSpawning();
            spawner.SpawnBoss();
        }

        // ▼▼▼ UIManager를 통해 UI 업데이트 호출 ▼▼▼
        if (UIManager.instance != null)
        {
            UIManager.instance.UpdateGameHUD(wave, killsThisWave, killsToNextWave, bossWave);
        }
        Debug.Log("WAVE " + wave + " START!");
    }

    // Boss.cs의 OnEnable에서 호출
    public void ShowBossHealthBar(Boss boss)
    {
        currentBoss = boss; // 보스 객체는 GameManager가 로직상 참조해야 함
        // ▼▼▼ UIManager를 통해 보스 체력바 활성화 및 업데이트 호출 ▼▼▼
        if (UIManager.instance != null)
        {
            UIManager.instance.ShowBossHealthBar(boss);
        }
    }

    public void UpdateBossHealth()
    {
        // ▼▼▼ UIManager를 통해 보스 체력바 업데이트 호출 ▼▼▼
        if (UIManager.instance != null)
        {
            UIManager.instance.UpdateBossHealthBar();
        }
        // GameManager는 더 이상 UI에 직접 접근하지 않고 로직만 담당
    }

    // Boss.cs의 Die에서 호출
    public void BossDied()
    {
        // ▼▼▼ UIManager를 통해 보스 체력바 비활성화 호출 ▼▼▼
        if (UIManager.instance != null)
        {
            UIManager.instance.HideBossHealthBar();
        }
        Debug.Log("BOSS KILLED! GAME CLEAR!");
    }

    public void PlayerDied()
    {
        PauseGame();
    }

    public int level;
    

 
}