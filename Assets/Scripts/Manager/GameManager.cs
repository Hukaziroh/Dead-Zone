using UnityEngine;
using TMPro; // TextMeshPro를 사용하기 위해 필수!
using UnityEngine.UI; // Slider를 사용하기 위해 추가!

public class GameManager : MonoBehaviour
{
    public static GameManager instance;
    public Spawner spawner; // Spawner 참조 추가

    [Header("# Game Control")]
    public float gameTime;
    public float maxGameTime = 2 * 10f;

    [Header("# Player Info")]
    public Player player;

    [Header("# Game Object")]
    public PoolManager pool;
    public Collider2D Bound;

    [Header("# UI Elements")]
    public TextMeshProUGUI waveText;
    public TextMeshProUGUI killCountText;
    public Slider bossHealthBar; // 보스 체력바 Slider 참조 추가

    [Header("# Wave System")]
    private int wave = 1;
    private int killsThisWave = 0;
    // 각 웨이브를 클리어하기 위해 필요한 킬 수 (0번=1웨이브, 1번=2웨이브...)
    private int[] killsToNextWave = { 4, 4, 4, 4,1 };
    // ▼▼▼ 보스 관리를 위한 변수 추가 ▼▼▼
    private Boss currentBoss;
    public int bossWave = 5; // 보스가 등장할 웨이브
    void Awake()
    {
        instance = this;
    }

    void Start()
    {
        // level 변수를 웨이브와 동기화 (Spawner가 사용)
        level = 0;
        UpdateUI();
    }

    void Update()
    {
        gameTime += Time.deltaTime;
        if (gameTime > maxGameTime)
        {
            gameTime = maxGameTime;
        }
    }

    // Enemy.cs에서 이 함수를 호출할 예정
    public void AddKill()
    {
        if (wave >= bossWave) return; // 보스 웨이브 이후에는 킬 카운트 멈춤

        killsThisWave++;
        if (killsThisWave >= killsToNextWave[wave - 1])
        {
            NextWave();
        }
        UpdateUI();
    }


    void NextWave()
    {
        wave++;
        killsThisWave = 0;

        // level 변수를 웨이브와 동기화 (Spawner가 더 강한 몬스터를 뽑도록)
        level = wave - 1;
        // ▼▼▼ 웨이브가 보스 웨이브인지 확인하는 로직 추가 ▼▼▼
        if (wave == bossWave)
        {
            // 보스 웨이브 시작!
            spawner.StopSpawning(); // 일반 몬스터 스폰 중지
            spawner.SpawnBoss();    // 보스 스폰
        }

        UpdateUI();
        Debug.Log("WAVE " + wave + " START!");
    }
    // Boss.cs의 OnEnable에서 호출
    public void ShowBossHealthBar(Boss boss)
    {
        currentBoss = boss;
        bossHealthBar.gameObject.SetActive(true);
        UpdateBossHealth();
    }

    public void UpdateBossHealth()
    {
        if (currentBoss != null && bossHealthBar != null)
        {
            // ▼▼▼ currentBoss.currentHealth 대신 currentBoss.health 사용 ▼▼▼
            float healthRatio = currentBoss.health / currentBoss.maxHealth;

            Debug.Log($"<color=green>Boss Health Bar Update: </color> Current HP: {currentBoss.health}, Max HP: {currentBoss.maxHealth}, Ratio: {healthRatio}");

            bossHealthBar.value = healthRatio;
        }
        else
        {
            if (currentBoss == null)
                Debug.LogError("<color=red>GameManager: currentBoss가 null입니다. 보스 체력바 업데이트 불가.</color>");
            if (bossHealthBar == null)
                Debug.LogError("<color=red>GameManager: bossHealthBar UI 슬라이더가 연결되지 않았습니다. 보스 체력바 업데이트 불가.</color>");
        }
    }

    // Boss.cs의 Die에서 호출
    public void BossDied()
    {
        bossHealthBar.gameObject.SetActive(false);
        // 여기에 게임 클리어 로직을 넣을 수 있습니다.
        Debug.Log("BOSS KILLED! GAME CLEAR!");
        // 예: Time.timeScale = 0; // 게임 정지
    }
    void UpdateUI()
    {
        waveText.text = "Wave: " + wave;
        if (wave >= bossWave)
        {
            killCountText.text = "BOSS WAVE";
        }
        else
        {
            killCountText.text = "Kills: " + killsThisWave + " / " + killsToNextWave[wave - 1];
        }
    }

    // 기존의 레벨, 경험치 관련 변수 및 함수는 그대로 유지하거나 필요에 맞게 수정
    public int level;
    public int exp;
    public int[] nextExp = { 10, 30, 60, 100, 150, 210, 280, 360, 450, 600 };

    public void GetExp()
    {
        exp++;
        if (exp >= nextExp[Mathf.Min(level, nextExp.Length - 1)])
        {
            level++;
            exp = 0;
        }
    }
}