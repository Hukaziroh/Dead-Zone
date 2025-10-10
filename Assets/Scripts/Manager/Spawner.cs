using UnityEngine;

public class Spawner : MonoBehaviour
{
    public Transform[] spawnPoint;
    public SpawnData[] spawnData;
    public LayerMask tilemapLayer;
    public int bossPoolIndex;
    public Transform bossSpawnPoint;

    float timer;
    int maxSpawnAttempts = 10;
    private GameManager gameManager;

    // GameManager가 호출하여 초기화
    public void Initialize(GameManager gm)
    {
        gameManager = gm;
    }

    private void Awake()
    {
        spawnPoint = GetComponentsInChildren<Transform>();
    }

    void Update()
    {
        if (gameManager == null || gameManager.isGamePaused) return;

        timer += Time.deltaTime;
        int level = gameManager.level;

        if (level >= spawnData.Length)
        {
            level = spawnData.Length - 1;
        }

        if (timer > spawnData[level].spawnTime)
        {
            timer = 0f;
            Spawn(level);
        }
    }

    void Spawn(int level)
    {
        Transform randomPoint = FindValidSpawnPoint();
        if (randomPoint == null) return;

        SpawnData currentSpawnData = spawnData[level];
        int[] enemyTypes = currentSpawnData.spriteTypes;
        int randomEnemyType = enemyTypes[Random.Range(0, enemyTypes.Length)];

        GameObject enemyObject = gameManager.pool.Get(randomEnemyType);
        enemyObject.transform.position = randomPoint.position;
    }
    // ▼▼▼ 사용자님의 기존 스폰 위치 검증 로직 (그대로 유지) ▼▼▼
    Transform FindValidSpawnPoint()
    {
        for (int i = 0; i < maxSpawnAttempts; i++)
        {
            Transform randomPoint = spawnPoint[Random.Range(1, spawnPoint.Length)];

            Debug.Log("스폰 시도 위치: " + randomPoint.position + " (시도 #" + (i + 1) + ")");

            Collider2D hit = Physics2D.OverlapPoint(randomPoint.position, tilemapLayer);

            if (hit != null)
            {
                Debug.Log("  -> 감지된 오브젝트: " + hit.gameObject.name + ", 태그: " + hit.tag + ", 레이어: " + LayerMask.LayerToName(hit.gameObject.layer));

                if (hit.CompareTag("Wall") || hit.CompareTag("Water"))
                {
                    Debug.LogWarning("  -> " + hit.gameObject.name + " (" + hit.tag + ") 이(가) 스폰 불가 지역입니다. 다음 위치를 시도합니다.");
                    continue;
                }
            }
            else
            {
                Debug.Log("  -> 스폰 지점에서 아무 콜라이더도 감지되지 않았습니다. (스폰 가능)");
            }

            Debug.Log("<color=green>  -> 유효한 스폰 위치를 찾았습니다: " + randomPoint.position + "</color>");
            return randomPoint;
        }

        Debug.LogError("<color=red>모든 스폰 시도(10번)가 실패했습니다. 유효한 스폰 위치를 찾지 못했습니다.</color>");
        return null;
    }

    public void StopSpawning()
    {
        this.enabled = false;
    }

    public void SpawnBoss()
    {
        GameObject boss = gameManager.pool.Get(bossPoolIndex);
        boss.transform.position = bossSpawnPoint.position;
    }
}

[System.Serializable]
public class SpawnData
{
    public int[] spriteTypes;
    public float spawnTime;
}