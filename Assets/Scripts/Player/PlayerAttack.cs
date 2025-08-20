using UnityEngine;

[RequireComponent(typeof(Animator))]
public class PlayerAttack : MonoBehaviour
{
    Animator anim;
    Camera cam;

    [Header("Attack Settings")]
    public int bulletPoolIndex = 4; // PoolManager에 등록된 총알 프리팹의 인덱스
    public Transform bulletSpawnPoint; // 총알이 생성될 위치

    void Awake()
    {
        anim = GetComponent<Animator>();
        cam = Camera.main;
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            if (!IsInAttackState())
            {
                // --- 기존 코드는 그대로 둡니다 ---
                Vector3 mouseWorldPos = cam.ScreenToWorldPoint(Input.mousePosition);
                mouseWorldPos.z = 0;
                Vector2 dir = (mouseWorldPos - transform.position).normalized;

                // ▼▼▼▼▼ 여기에 핵심 코드를 추가합니다 ▼▼▼▼▼
                // 마우스 방향을 기반으로 총알 생성 위치를 실시간으로 업데이트합니다.
                // 0.5f 라는 값은 플레이어 중심에서 얼마나 떨어진 곳에 생성할지 정하는 거리입니다.
                // 이 값을 조절해서 총알이 생성되는 위치를微세하게 바꿀 수 있습니다.
                float spawnDistance = 0.3f; // 이 값을 조절해 보세요.
                bulletSpawnPoint.localPosition = dir * spawnDistance;
                // ▲▲▲▲▲▲▲▲▲▲▲▲▲▲▲▲▲▲▲▲▲▲▲▲▲▲

                // --- 나머지 기존 코드도 그대로 둡니다 ---
                anim.SetFloat("AttackX", dir.x);
                anim.SetFloat("AttackY", dir.y);
                anim.SetTrigger("IsAttack");

                FireBullet(dir);
            }
        }
    }

    // PlayerAttack.cs 의 FireBullet 함수

    void FireBullet(Vector2 direction)
    {
        Debug.Log("--- 1. FireBullet 함수 시작 ---");

        // 1. PoolManager에서 총알을 가져옵니다.
        GameObject bulletObject = GameManager.instance.pool.Get(bulletPoolIndex);
        Debug.Log("2. Pool에서 가져온 총알: " + bulletObject.name);

        // 2. 총알의 위치를 지정된 발사 위치로 설정합니다.
        bulletObject.transform.SetParent(null);
        Debug.Log("3. 지정된 발사 위치(SpawnPoint): " + bulletSpawnPoint.position);
        bulletObject.transform.position = bulletSpawnPoint.position;
        Debug.Log("4. 총알의 실제 위치 설정 후: " + bulletObject.transform.position);


        // 3. Bullet 스크립트의 Init 함수를 호출하여 발사!
        Bullet bulletScript = bulletObject.GetComponent<Bullet>();
        if (bulletScript != null)
        {
            Debug.Log("5. 총알에게 전달할 발사 방향: " + direction);
            bulletScript.Init(direction);
        }
        else
        {
            Debug.LogError("총알에서 Bullet.cs 스크립트를 찾을 수 없습니다!");
        }
        Debug.Log("--- 6. FireBullet 함수 종료 ---");
    }

    private bool IsInAttackState()
    {
        return anim.GetCurrentAnimatorStateInfo(0).IsTag("Attack");
    }
}