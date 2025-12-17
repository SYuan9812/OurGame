//Damage Player
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActionAttack : FSMAction
{
    [Header("Config")]
    [SerializeField] private float damage;
    [SerializeField] private float timeBtwAttacks;
    private EnemyBrain enemyBrain;
    private float timer;

    private void Awake()
    {
        enemyBrain = GetComponent<EnemyBrain>();
    }


    public override void Act()
    {
        AttackPlayer();
    }

    private void AttackPlayer()
    {
        if (enemyBrain.Player == null) return;
        timer -= Time.deltaTime;
        if(timer <= 0f)
        {
            IDamageable player = enemyBrain.Player.GetComponent<IDamageable>();
            PlayerHealth health = enemyBrain.Player.GetComponent<PlayerHealth>();
            if(health.isDead == true) return;
            player.TakeDamage(damage);
            timer = timeBtwAttacks;
        }
    }
}
//Add something in Player Health class
//stats.Health = 0f in the TakeDamage method