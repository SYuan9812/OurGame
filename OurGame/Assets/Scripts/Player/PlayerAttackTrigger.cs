using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAttackTrigger : MonoBehaviour
{
    public int damage = 10;
    private HashSet<EnemyBase> damagedEnemies = new HashSet<EnemyBase>(); //Tracking enemies that are damaged

    void Start()
    {
        Destroy(gameObject, 0.3f);
    }

    private void OnTriggerEnter2D(Collider2D collision) //Create attack range when touches enemy
    {
        if (collision == null) return;

        EnemyBase enemyBase = collision.transform.GetComponentInParent<EnemyBase>();

        if (enemyBase != null && !damagedEnemies.Contains(enemyBase))
        {
            enemyBase.EnemyTakeDamage(damage);
            damagedEnemies.Add(enemyBase); //Add enemy to damaged
        }
    }

    public void SetDamage(int Num)
    {
        damage = Num;
    }
}