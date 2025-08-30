using UnityEngine;
using TMPro; // TextMeshPro를 사용하기 위해 필수!

public class GameManager : MonoBehaviour
{
    public static GameManager instance;

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

    [Header("# Wave System")]
    private int wave = 1;
    private int killsThisWave = 0;
    // 각 웨이브를 클리어하기 위해 필요한 킬 수 (0번=1웨이브, 1번=2웨이브...)
    private int[] killsToNextWave = { 30, 50, 70, 100 };

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
        // 현재 웨이브가 마지막 웨이브(5) 이상이면 킬 수만 올리고 종료
        if (wave > killsToNextWave.Length)
        {
            killsThisWave++;
            UpdateUI();
            return;
        }

        killsThisWave++;

        // 현재 웨이브의 목표 킬 수를 달성했는지 확인
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

        Debug.Log("WAVE " + wave + " START!");
    }

    void UpdateUI()
    {
        waveText.text = "Wave: " + wave;

        if (wave > killsToNextWave.Length)
        {
            // 마지막 웨이브 이후
            killCountText.text = "Kills: " + killsThisWave;
        }
        else
        {
            // 현재 웨이브 진행 중
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