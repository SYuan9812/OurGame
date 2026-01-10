using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAttackTrigger : MonoBehaviour
{
    public int damage = 10;
    private HashSet<EnemyBase> damagedEnemies = new HashSet<EnemyBase>(); //Tracking enemies that are damaged
    private HashSet<BossBase> damagedBosses = new HashSet<BossBase>();
    // Reference to player transform for knockback direction calculation
    private Transform playerTransform;

    void Start()
    {
        Destroy(gameObject, 0.3f);
        playerTransform = GameObject.FindGameObjectWithTag("Player")?.transform; // Find player transform
    }

    private void OnTriggerEnter2D(Collider2D collision) //Create attack range when touches enemy
    {
        if (collision == null) return;

        EnemyBase enemyBase = collision.transform.GetComponentInParent<EnemyBase>();
        BossBase bossBase = collision.transform.GetComponentInParent<BossBase>();

        if (enemyBase != null && !damagedEnemies.Contains(enemyBase))
        {
            // Calculate knockback direction (away from player)
            Vector2 knockbackDirection = CalculateKnockbackDirection(enemyBase.transform);
            enemyBase.EnemyTakeDamage(damage, knockbackDirection);
            damagedEnemies.Add(enemyBase); //Add enemy to damaged
        }

        if (bossBase != null && !damagedBosses.Contains(bossBase))
        {
            bossBase.BossTakeDamage(damage);
            damagedBosses.Add(bossBase);
        }
    }

    // Calculate direction from player to enemy (reverse for knockback away from player)
    private Vector2 CalculateKnockbackDirection(Transform enemyTransform)
    {
        if (playerTransform == null)
        {
            // Default knockback direction (right) if player not found
            return Vector2.right;
        }

        Vector2 direction = (enemyTransform.position - playerTransform.position).normalized;
        return direction;
    }

    public void SetDamage(int Num)
    {
        damage = Num;
    }
}