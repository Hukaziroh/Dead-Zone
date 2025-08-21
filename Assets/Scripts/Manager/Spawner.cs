using UnityEngine;

public class Spawner : MonoBehaviour
{
    public Transform[] spawnPoint;
    public SpawnData[] spawnData;
    public LayerMask layerMask; // 타일맵 레이어를 감지하기 위한 변수

    float timer;
    //int maxSpawnAttempts = 10; // 유효한 스폰 지점을 찾기 위한 최대 시도 횟수

    private void Awake()
    {
        spawnPoint = GetComponentsInChildren<Transform>();
    }

    void Update()
    {
        timer += Time.deltaTime;

        if (timer > spawnData[0].spawnTime)
        {
            timer = 0f;
            Spawn();
        }
    }

    // Spawner.cs 스크립트의 Spawn 함수

    void Spawn()
    {
        // 스폰 위치를 딱 하나만 정해서 테스트합니다.
        Transform randomPoint = spawnPoint[Random.Range(1, spawnPoint.Length)];

        Debug.Log("스폰 시도 위치: " + randomPoint.position); // 스폰을 시도하는 좌표 출력

        // 해당 위치에 있는 '모든' 콜라이더를 감지합니다.
        Collider2D[] hits = Physics2D.OverlapPointAll(randomPoint.position);

        bool canSpawn = true; // 스폰 가능 여부를 판단하는 변수

        // 감지된 것이 있는지 확인
        if (hits.Length > 0)
        {
            // 감지된 모든 콜라이더의 정보를 출력합니다.
            foreach (Collider2D hit in hits)
            {
                Debug.Log("스폰 지점에서 감지된 오브젝트: " + hit.gameObject.name + ", 태그: " + hit.tag);

                // 감지된 것들 중 하나라도 "Water" 태그를 가지고 있다면,
                if (hit.CompareTag("Water") || hit.CompareTag("Wall"))
                {
                    canSpawn = false; // 스폰 불가능으로 표시
                    Debug.LogWarning("물 위라서 스폰을 취소합니다!");
                    break; // 더 이상 검사할 필요 없으므로 반복 중단
                }
            }
        }
        else
        {
            Debug.Log("스폰 지점에서 아무 콜라이더도 감지되지 않았습니다. (땅으로 간주)");
        }

        // 최종적으로 스폰이 가능하다면
        if (canSpawn)
        {
            Debug.Log("최종 스폰 결정!");
            GameObject enemy = GameManager.instance.pool.Get(Random.Range(0, 4));
            enemy.transform.position = randomPoint.position;
        }
    }

    [System.Serializable]
    public class SpawnData
    {
        public int spriteType;
        public float spawnTime;
        public int health;
        public float speed;
    }
}