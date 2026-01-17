using System.Collections.Generic;
using UnityEngine;

public class PlayerAttackTrigger : MonoBehaviour
{
    public int damage = 10;
    private float attackDuration = 0.3f;
    private HashSet<EnemyBase> damagedEnemies = new HashSet<EnemyBase>();
    private HashSet<BossBase> damagedBosses = new HashSet<BossBase>();
    private Transform playerTransform;

    public void Init(WeaponData currentWeaponData)
    {
        this.damage = currentWeaponData.attackDamage;
        this.attackDuration = currentWeaponData.attackDuration;
        Destroy(gameObject, attackDuration);
    }

    void Start()
    {
        playerTransform = GameObject.FindGameObjectWithTag("Player")?.transform;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision == null) return;

        EnemyBase enemyBase = collision.transform.GetComponentInParent<EnemyBase>();
        if (enemyBase != null && !damagedEnemies.Contains(enemyBase))
        {
            Vector2 knockbackDirection = CalculateKnockbackDirection(enemyBase.transform);
            enemyBase.EnemyTakeDamage(damage, knockbackDirection);
            damagedEnemies.Add(enemyBase);
        }

        BossBase bossBase = collision.transform.GetComponent<BossBase>();
        if (bossBase != null && !damagedBosses.Contains(bossBase))
        {
            bossBase.BossTakeDamage(damage);
            damagedBosses.Add(bossBase);
        }
    }

    private Vector2 CalculateKnockbackDirection(Transform enemyTransform)
    {
        if (playerTransform == null) return Vector2.right;

        return (enemyTransform.position - playerTransform.position).normalized;
    }

    public void SetDamage(int Num)
    {
        damage = Num;
    }
}