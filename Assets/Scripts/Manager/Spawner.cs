using UnityEngine;

public class Spawner : MonoBehaviour
{
    public Transform[] spawnPoint;
    public SpawnData[] spawnData;
    public LayerMask tilemapLayer;
    public int bossPoolIndex;       // PoolManager에 등록된 보스 프리팹의 인덱스
    public Transform bossSpawnPoint; // 보스가 스폰될 고정 위치

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

    }

    Transform FindValidSpawnPoint()
    {
        for (int i = 0; i < maxSpawnAttempts; i++)
        {
            Transform randomPoint = spawnPoint[Random.Range(1, spawnPoint.Length)];

            // 디버그용: 스폰 시도 위치를 먼저 출력
            Debug.Log("스폰 시도 위치: " + randomPoint.position + " (시도 #" + (i + 1) + ")");

            // 지정된 tilemapLayer만 대상으로 OverlapPoint를 시도합니다.
            Collider2D hit = Physics2D.OverlapPoint(randomPoint.position, tilemapLayer);

            if (hit != null)
            {
                // 감지된 오브젝트의 이름과 태그, 레이어를 상세히 출력
                Debug.Log("  -> 감지된 오브젝트: " + hit.gameObject.name + ", 태그: " + hit.tag + ", 레이어: " + LayerMask.LayerToName(hit.gameObject.layer));

                // 태그가 "Obstacle"이거나 "Water"인지 확인
                if (hit.CompareTag("Wall") || hit.CompareTag("Water"))
                {
                    Debug.LogWarning("  -> " + hit.gameObject.name + " (" + hit.tag + ") 이(가) 스폰 불가 지역입니다. 다음 위치를 시도합니다.");
                    continue; // 스폰 불가 지역이므로 다음 시도로 넘어갑니다.
                }
            }
            else
            {
                Debug.Log("  -> 스폰 지점에서 아무 콜라이더도 감지되지 않았습니다. (스폰 가능)");
            }

            // 여기까지 왔다면 유효한 스폰 위치이므로 반환
            Debug.Log("<color=green>  -> 유효한 스폰 위치를 찾았습니다: " + randomPoint.position + "</color>");
            return randomPoint;
        }

        // 모든 시도가 실패했을 경우
        Debug.LogError("<color=red>모든 스폰 시도(10번)가 실패했습니다. 유효한 스폰 위치를 찾지 못했습니다.</color>");
        return null;
    }

    // GameManager가 호출할 함수: 일반 몬스터 스폰 중지
    public void StopSpawning()
    {
        // Spawner 스크립트 자체를 비활성화하여 Update 함수를 멈춥니다.
        this.enabled = false;
    }

    // GameManager가 호출할 함수: 보스 스폰
    public void SpawnBoss()
    {
        GameObject boss = GameManager.instance.pool.Get(bossPoolIndex);
        boss.transform.position = bossSpawnPoint.position;
    }
}

// 스포너의 역할이 간단해졌으므로, SpawnData도 간단하게 변경합니다.
[System.Serializable]
public class SpawnData
{
    public int[] spriteTypes; // 이 웨이브에 등장할 몬스터 종류 목록
    public float spawnTime;   // 이 웨이브의 스폰 주기
}