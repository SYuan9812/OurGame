//chase player

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActionChase : FSMAction
{
    [Header("Config")]
    [SerializeField] private float chaseSpeed;

    private EnemyBrain enemyBrain;
    private Animator animator;
    private Vector3 chaseDirection;

    private void Awake()
    {
        enemyBrain = GetComponent<EnemyBrain>();
        animator = GetComponent<Animator>();
    }

    public override void Act()
    {
        ChasePlayer();
    }

    private void ChasePlayer()
    {
        if(enemyBrain == null) return;
        chaseDirection = (enemyBrain.Player.position - transform.position).normalized;
        Vector3 dirToPlayer = enemyBrain.Player.position - transform.position;
        if(dirToPlayer.magnitude >= 1.3f)
        {
            transform.Translate(dirToPlayer.normalized * (chaseSpeed * Time.deltaTime));
            animator.SetFloat("EnemyX", chaseDirection.x);
            animator.SetFloat("EnemyY", chaseDirection.y);
        }
    }
}