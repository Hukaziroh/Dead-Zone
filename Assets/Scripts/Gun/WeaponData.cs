// WeaponData.cs

using UnityEngine;

// CreateAssetMenu를 사용하면 유니티 에디터에서 WeaponData 에셋을 쉽게 생성할 수 있습니다.
[CreateAssetMenu(fileName = "WeaponData", menuName = "ScriptableObjects/Weapon Data", order = 1)]
public class WeaponData : ScriptableObject
{
    [Header("Weapon Info")]
    public string weaponName; // 무기 이름 (예: "Rifle")

    [Header("Weapon Stats")]
    public int damage;             // 공격력
    public float attackCooldown;     // 공격 속도 (쿨타임)
    public float bulletLifetime;     // 총알 사거리 (수명)
    public int penetrationCount;     // 관통 횟수 (1이면 비관통)

    [Header("Shotgun Specific")]
    public int pelletCount = 1;      // 한번에 발사되는 총알 수 (샷건용)
    public float spreadAngle = 0f;     // 총알 분산 각도 (샷건용)
}