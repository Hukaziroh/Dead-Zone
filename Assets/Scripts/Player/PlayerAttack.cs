using UnityEngine;

[RequireComponent(typeof(Animator), typeof(AudioSource))]
public class PlayerAttack : MonoBehaviour
{
    Animator anim;
    Camera cam;
    private AudioSource audioSource;
    //public AudioClip fireSound;
    public int bulletPoolIndex = 4;
    public Transform playerCenterPoint;

    [SerializeField] private WeaponData currentWeapon;
    private float lastAttackTime;
    private GameManager gameManager;

    [Header("Muzzle Offsets (Relative to Player Center)")]
    public Vector2 offsetUp = new Vector2(0.0f, 0.5f);
    public Vector2 offsetDown = new Vector2(0.0f, -0.5f);
    public Vector2 offsetLeft = new Vector2(-0.5f, 0.1f);
    public Vector2 offsetRight = new Vector2(0.5f, 0.1f);
    public Vector2 offsetUpRight = new Vector2(0.35f, 0.35f);
    public Vector2 offsetUpLeft = new Vector2(-0.35f, 0.35f);
    public Vector2 offsetDownRight = new Vector2(0.35f, -0.35f);
    public Vector2 offsetDownLeft = new Vector2(-0.35f, -0.35f);

    // 8방향 오프셋 배열 캐싱용
    private Vector2[] muzzleOffsets;

    void Awake()
    {
        anim = GetComponent<Animator>();
        audioSource = GetComponent<AudioSource>();
        cam = Camera.main;
        playerCenterPoint = this.transform;
        gameManager = FindAnyObjectByType<GameManager>();

        // 배열 초기화 (0: 오른쪽부터 반시계 방향으로 45도씩 증가)
        muzzleOffsets = new Vector2[] {
            offsetRight, offsetUpRight, offsetUp, offsetUpLeft,
            offsetLeft, offsetDownLeft, offsetDown, offsetDownRight
        };
    }

    public void EquipWeapon(WeaponData weaponData)
    {
        currentWeapon = weaponData;
        lastAttackTime = -currentWeapon.attackCooldown;
        Debug.Log($"[PlayerAttack] '{currentWeapon.weaponName}' 장착! (데미지: {currentWeapon.damage}, 쿨타임: {currentWeapon.attackCooldown})");
    }

    void Update()
    {
        if (gameManager != null && gameManager.isGamePaused) return;
        if (currentWeapon == null) return;

        if (Input.GetMouseButton(0))
        {
            if (!IsInAttackState() && Time.time >= lastAttackTime + currentWeapon.attackCooldown)
            {
                Vector3 mouseWorldPos = cam.ScreenToWorldPoint(Input.mousePosition);
                Vector2 dir = ((Vector2)mouseWorldPos - (Vector2)transform.position).normalized;

                anim.SetTrigger("IsAttack");
                lastAttackTime = Time.time;

                Vector2 currentOffset = CalculateMuzzleOffset(dir);
                Vector3 finalMuzzleWorldPosition = (Vector2)playerCenterPoint.position + currentOffset;

                FireBullet(dir, finalMuzzleWorldPosition);
            }
        }
    }

    void FireBullet(Vector2 direction, Vector3 muzzleWorldPosition)
    {
        if (currentWeapon.gunshotSound != null)
        {
            audioSource.PlayOneShot(currentWeapon.gunshotSound);
        }

        // 펠릿(산탄) 개수만큼 탄환 동시 발사
        for (int i = 0; i < currentWeapon.pelletCount; i++)
        {
            GameObject bulletObject = gameManager.pool.Get(bulletPoolIndex);
            bulletObject.transform.position = muzzleWorldPosition;

            // 지정된 방사각(Spread Angle) 내에서 랜덤 Z축 회전값 생성
            Quaternion randomRotation = Quaternion.Euler(0, 0, Random.Range(-currentWeapon.spreadAngle / 2f, currentWeapon.spreadAngle / 2f));
            
            // 기준 방향에 회전값을 곱해 새로운 발사 방향 도출
            Vector2 fireDirection = randomRotation * direction;

            Bullet bulletScript = bulletObject.GetComponent<Bullet>();
            if (bulletScript != null)
            {
                bulletScript.damage = currentWeapon.damage;
                bulletScript.Init(fireDirection, currentWeapon.penetrationCount, currentWeapon.bulletLifetime);
            }
        }
    }

    private Vector2 CalculateMuzzleOffset(Vector2 mouseDir)
    {
        float angle = Vector2.SignedAngle(Vector2.right, mouseDir);
        if (angle < 0) angle += 360f;

        // 22.5도를 더해 기준선을 맞추고 45도로 나누어 0~7 사이의 인덱스 도출
        int index = Mathf.FloorToInt((angle + 22.5f) / 45f) % 8;
        
        return muzzleOffsets[index];
    }

    private bool IsInAttackState()
    {
        return anim.GetCurrentAnimatorStateInfo(0).IsTag("Attack");
    }

    public void UpgradeDamage(int amount) { if (currentWeapon) currentWeapon.damage += amount; }
    public void UpgradeCooldown(float amount) { if (currentWeapon) currentWeapon.attackCooldown = Mathf.Max(0.1f, currentWeapon.attackCooldown - amount); }
}