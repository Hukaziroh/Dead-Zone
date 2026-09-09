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

    // GameManager를 호출하여 초기화
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
        if (randomPoint == null) return; // 안전 구역 탐색 실패 시 스폰 취소 (오버헤드 방지)

        SpawnData currentSpawnData = spawnData[level];
        int randomEnemyType = currentSpawnData.spriteTypes[Random.Range(0, currentSpawnData.spriteTypes.Length)];

        // 오브젝트 풀링을 사용해 메모리 최적화
        GameObject enemyObject = gameManager.pool.Get(randomEnemyType);
        enemyObject.transform.position = randomPoint.position;
    }
    
    Transform FindValidSpawnPoint()
    {
        // 무한 루프(프리징) 방지를 위한 최대 10회 탐색 제한
        for (int i = 0; i < maxSpawnAttempts; i++)
        {
            Transform randomPoint = spawnPoint[Random.Range(1, spawnPoint.Length)];

            // 해당 좌표의 타일맵 물리 충돌체 단일 검사
            Collider2D hit = Physics2D.OverlapPoint(randomPoint.position, tilemapLayer);

            if (hit != null)
            {
                // 이동 불가 지역(벽, 물)인 경우 기각하고 다음 위치 탐색
                if (hit.CompareTag("Wall") || hit.CompareTag("Water"))
                {
                    continue;
                }
            }

            return randomPoint; // 검증된 안전한 좌표 반환
        }

        return null; // 10회 모두 실패 시 null 반환
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