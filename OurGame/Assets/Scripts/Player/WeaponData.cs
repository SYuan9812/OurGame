using UnityEngine;

[CreateAssetMenu(fileName = "New Weapon Data", menuName = "Game/Weapon Data")]
public class WeaponData : ScriptableObject
{
    [Header("Basis Stats")]
    public string weaponName;
    public int attackDamage = 10;
    public float attackCooldown = 0.667f;
    public float attackDuration = 0.3f;

    [Header("Prefabs and Anim")]
    public GameObject attackRangePrefab;
    public AnimatorOverrideController weaponAnimatorOverride;
    public string attackTriggerName = "Attack1";

    [Header("Visuals")]
    public GameObject weaponModelPrefab;
    public Transform weaponHoldPoint;

    [Header("Flashlight Bullet")]
    public GameObject lightBallPrefab;
    public float lightBallSpeed = 5f;
    public float lightBallLifetime = 2f; 
}
