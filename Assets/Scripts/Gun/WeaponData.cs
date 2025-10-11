// WeaponData.cs

using UnityEngine;

[CreateAssetMenu(fileName = "WeaponData", menuName = "ScriptableObjects/Weapon Data", order = 1)]
public class WeaponData : ScriptableObject
{
    [Header("Weapon Info")]
    public string weaponName;
    // ▼▼▼ 여기에 사운드 클립 변수를 추가합니다 ▼▼▼
    public AudioClip gunshotSound; // 발사 사운드

    [Header("Weapon Stats")]
    public int damage;
    public float attackCooldown;
    public float bulletLifetime;
    public int penetrationCount;

    [Header("Shotgun Specific")]
    public int pelletCount = 1;
    public float spreadAngle = 0f;
}