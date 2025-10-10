using UnityEngine;

[RequireComponent(typeof(Animator), typeof(AudioSource))]
public class PlayerAttack : MonoBehaviour
{
    Animator anim;
    Camera cam;
    private AudioSource audioSource;
    public AudioClip fireSound;
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

    void Awake()
    {
        anim = GetComponent<Animator>();
        audioSource = GetComponent<AudioSource>();
        cam = Camera.main;
        playerCenterPoint = this.transform;
        gameManager = FindAnyObjectByType<GameManager>();
    }

    public void EquipWeapon(WeaponData weaponData)
    {
        currentWeapon = weaponData;
        lastAttackTime = -currentWeapon.attackCooldown;
    }

    void Update()
    {
        // ▼▼▼ 이 부분을 수정합니다! GameManager.instance -> gameManager ▼▼▼

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
        if (fireSound != null) audioSource.PlayOneShot(fireSound);

        for (int i = 0; i < currentWeapon.pelletCount; i++)
        {
            GameObject bulletObject = gameManager.pool.Get(bulletPoolIndex);
            bulletObject.transform.position = muzzleWorldPosition;

            Quaternion randomRotation = Quaternion.Euler(0, 0, Random.Range(-currentWeapon.spreadAngle / 2, currentWeapon.spreadAngle / 2));
            Vector2 fireDirection = randomRotation * direction;

            Bullet bulletScript = bulletObject.GetComponent<Bullet>();
            if (bulletScript != null)
            {
                bulletScript.damage = currentWeapon.damage;
                bulletScript.lifetime = currentWeapon.bulletLifetime;
                bulletScript.Init(fireDirection, currentWeapon.penetrationCount);
            }
        }
    }

    private Vector2 CalculateMuzzleOffset(Vector2 mouseDir)
    {
        float angle = Vector2.SignedAngle(Vector2.right, mouseDir);
        if (angle < 0) angle += 360;

        if (angle >= 337.5f || angle < 22.5f) return offsetRight;
        if (angle >= 22.5f && angle < 67.5f) return offsetUpRight;
        if (angle >= 67.5f && angle < 112.5f) return offsetUp;
        if (angle >= 112.5f && angle < 157.5f) return offsetUpLeft;
        if (angle >= 157.5f && angle < 202.5f) return offsetLeft;
        if (angle >= 202.5f && angle < 247.5f) return offsetDownLeft;
        if (angle >= 247.5f && angle < 292.5f) return offsetDown;
        return offsetDownRight;
    }
    private bool IsInAttackState()
    {
        return anim.GetCurrentAnimatorStateInfo(0).IsTag("Attack");
    }

    public void UpgradeDamage(int amount) { if (currentWeapon) currentWeapon.damage += amount; }
    public void UpgradeCooldown(float amount) { if (currentWeapon) currentWeapon.attackCooldown = Mathf.Max(0.1f, currentWeapon.attackCooldown - amount); }
}