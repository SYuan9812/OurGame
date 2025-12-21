using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAttack : MonoBehaviour
{
    public int ATK = 10;
    public Animator animBrick;
    public Animator animSlash;
    public GameObject BrickAttackRange;
    public Transform BrickTriPos;
    private float attackCooldown = 0.667f;
    private float lastAttackTime;

    void Start()
    {
        lastAttackTime = -attackCooldown;
    }


    void Update()
    {
        Attack();
    }

    public void Attack()
    {
        //Calling attack
        // Check if left mouse button is pressed and attack cooldown is completed
        if (Input.GetMouseButtonDown(0) && Time.time >= lastAttackTime + attackCooldown)
        {
            animBrick.SetTrigger("Attack1");
            animSlash.SetTrigger("Attack1");
            GameObject attackRange = Instantiate(
                BrickAttackRange,
                BrickTriPos.position,
                BrickTriPos.rotation,
                null
            );
            attackRange.GetComponent<PlayerAttackTrigger>().SetDamage(ATK);
            // Update last attack time to current time to start cooldown counting
            lastAttackTime = Time.time;
        }
    }
}
