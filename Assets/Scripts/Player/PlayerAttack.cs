// PlayerAttack.cs (총소리 기능 추가 버전)

using UnityEngine;

// ▼▼▼ 1. AudioSource를 사용하기 위해 RequireComponent 추가 ▼▼▼
[RequireComponent(typeof(Animator), typeof(AudioSource))]
public class PlayerAttack : MonoBehaviour
{
    Animator anim;
    Camera cam;
    // ▼▼▼ 2. 사운드 재생을 위한 변수 추가 ▼▼▼
    private AudioSource audioSource;
    public AudioClip fireSound; // 인스펙터에서 지정할 총소리 파일

    [Header("Attack Settings")]
    public int bulletPoolIndex = 4;
    public Transform playerCenterPoint;

    public int currentAttackDamage = 10;
    public float currentAttackCooldown = 0.5f;
    private float lastAttackTime;

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
        // ▼▼▼ 3. AudioSource 컴포넌트 초기화 ▼▼▼
        audioSource = GetComponent<AudioSource>();
        cam = Camera.main;
        playerCenterPoint = this.transform;

        lastAttackTime = -currentAttackCooldown;
    }

    void Update()
    {
        if (GameManager.instance != null && GameManager.instance.GetIsGamePaused())
        {
            return;
        }

        if (Input.GetMouseButtonDown(0))
        {
            if (!IsInAttackState() && Time.time >= lastAttackTime + currentAttackCooldown)
            {
                Vector3 mouseWorldPos = cam.ScreenToWorldPoint(Input.mousePosition);
                mouseWorldPos.z = 0;
                Vector2 dir = (mouseWorldPos - transform.position).normalized;

                anim.SetTrigger("IsAttack");
                lastAttackTime = Time.time;

                Vector2 currentOffset = CalculateMuzzleOffset(dir);
                Vector3 finalMuzzleWorldPosition = (Vector2)playerCenterPoint.position + currentOffset;

                FireBullet(dir, finalMuzzleWorldPosition);
            }
        }
    }

    private Vector2 CalculateMuzzleOffset(Vector2 mouseDir)
    {
        float angle = Vector2.SignedAngle(Vector2.right, mouseDir);
        if (angle < 0) angle += 360;

        if (angle >= 337.5f || angle < 22.5f) return offsetRight;
        else if (angle >= 22.5f && angle < 67.5f) return offsetUpRight;
        else if (angle >= 67.5f && angle < 112.5f) return offsetUp;
        else if (angle >= 112.5f && angle < 157.5f) return offsetUpLeft;
        else if (angle >= 157.5f && angle < 202.5f) return offsetLeft;
        else if (angle >= 202.5f && angle < 247.5f) return offsetDownLeft;
        else if (angle >= 247.5f && angle < 292.5f) return offsetDown;
        else if (angle >= 292.5f && angle < 337.5f) return offsetDownRight;

        return offsetRight;
    }

    void FireBullet(Vector2 direction, Vector3 muzzleWorldPosition)
    {
        // ▼▼▼ 4. 총알을 발사하기 직전에 사운드 재생 ▼▼▼
        if (fireSound != null)
        {
            audioSource.PlayOneShot(fireSound);
        }

        GameObject bulletObject = GameManager.instance.pool.Get(bulletPoolIndex);
        bulletObject.transform.SetParent(null);
        bulletObject.transform.position = muzzleWorldPosition;

        Bullet bulletScript = bulletObject.GetComponent<Bullet>();
        if (bulletScript != null)
        {
            bulletScript.Init(direction);
            bulletScript.damage = currentAttackDamage;
        }
        else
        {
            Debug.LogError("총알에서 Bullet.cs 스크립트를 찾을 수 없습니다!");
        }
    }

    private bool IsInAttackState()
    {
        return anim.GetCurrentAnimatorStateInfo(0).IsTag("Attack");
    }

    public void SetAttackDamage(int newDamage)
    {
        currentAttackDamage = newDamage;
    }

    public void SetAttackCooldown(float newCooldown)
    {
        currentAttackCooldown = newCooldown;
    }
}