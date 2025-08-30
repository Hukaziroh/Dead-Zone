using UnityEngine;

public class Spawner : MonoBehaviour
{
    public Transform[] spawnPoint;
    public SpawnData[] spawnData;
    public LayerMask tilemapLayer;

    float timer;
    int maxSpawnAttempts = 10;

    private void Awake()
    {
        spawnPoint = GetComponentsInChildren<Transform>();
    }

    void Update()
    {
        timer += Time.deltaTime;
        int level = GameManager.instance.level;

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
        if (randomPoint == null)
        {
            return;
        }

        SpawnData currentSpawnData = spawnData[level];

        int[] enemyTypes = currentSpawnData.spriteTypes;
        int randomIndex = Random.Range(0, enemyTypes.Length);
        int randomEnemyType = enemyTypes[randomIndex];

        GameObject enemyObject = GameManager.instance.pool.Get(randomEnemyType);
        enemyObject.transform.position = randomPoint.position;

        // ▼▼▼ Init 함수 호출 부분을 삭제합니다. ▼▼▼
        // 이제 능력치는 각 프리팹이 스스로 가지고 있습니다.
    }

    Transform FindValidSpawnPoint()
    {
        for (int i = 0; i < maxSpawnAttempts; i++)
        {
            Transform randomPoint = spawnPoint[Random.Range(1, spawnPoint.Length)];
            Collider2D hit = Physics2D.OverlapPoint(randomPoint.position, tilemapLayer);

            if (hit == null || (!hit.CompareTag("Obstacle") && !hit.CompareTag("Water")))
            {
                return randomPoint;
            }
        }
        return null;
    }
}

// 스포너의 역할이 간단해졌으므로, SpawnData도 간단하게 변경합니다.
[System.Serializable]
public class SpawnData
{
    public int[] spriteTypes; // 이 웨이브에 등장할 몬스터 종류 목록
    public float spawnTime;   // 이 웨이브의 스폰 주기
}