using System;
using System.Collections.Generic;
using UnityEngine;

public enum StateType
{
    Idle, Chase, Wander, Melee, Range
}

[Serializable]
public class Parameter
{
    public int health;
    public float moveSpeed;
    public float chaseSpeed;
    public float idleTime;
    public float chaseDuration;
    public float wanderDuration;
    public float attackArea;

    public float meleeCooldown = 2f;
    public float meleeDamage = 6f;

    public Transform target;
    public LayerMask targetLayer;
    public Transform attackPoint;
    public Animator anim;


    public float rangeWarningDuration = 1.5f;
    public float rangeExplodeRadius = 2f;     
    public float rangeExplodeDamage = 10f;   
    public GameObject rangeWarningPrefab;
    public GameObject rangeExplodePrefab; 
}

public interface BossState
{
    void OnEnter();
    void OnUpdate();
    void OnExit();
}

public class BossFSM : MonoBehaviour
{
    public Parameter parameter;
    private BossBase bossBase;

    private BossState currentState;
    private Dictionary<StateType, BossState> states = new Dictionary<StateType, BossState>();

    private float lastMeleeTime;
    void Start()
    {
        bossBase = GetComponent<BossBase>();

        if (parameter == null) parameter = new Parameter();
        parameter.anim = GetComponent<Animator>();

        states.Add(StateType.Idle, new IdleState(this));
        states.Add(StateType.Chase, new ChaseState(this));
        states.Add(StateType.Wander, new WanderState(this));
        states.Add(StateType.Melee, new MeleeState(this));
        states.Add(StateType.Range, new RangeState(this));

        TransitionState(StateType.Idle);
    }

    void Update()
    {
        currentState?.OnUpdate();
    }

    public void TransitionState(StateType type)
    {
        currentState?.OnExit();
        currentState = states[type];
        currentState?.OnEnter();

        if (type == StateType.Melee)
        {
            lastMeleeTime = Time.time;
        }
    }


    public bool IsMeleeCoolingDown()
    {
        return Time.time - lastMeleeTime < parameter.meleeCooldown;
    }


    public void FlipTo(Transform target)
    {
        if (target == null) return;


        float dirX = target.position.x - transform.position.x;


        if (dirX > 0)
        {
            transform.localScale = new Vector3(
                -Mathf.Abs(transform.localScale.x),
                transform.localScale.y,
                transform.localScale.z
            );
        }

        else
        {
            transform.localScale = new Vector3(
                Mathf.Abs(transform.localScale.x),
                transform.localScale.y,
                transform.localScale.z
            );
        }


        float attackOffsetX = -1.75f;
        float attackOffsetY = -0.7f;
        parameter.attackPoint.localPosition = new Vector3(attackOffsetX, attackOffsetY, 0);
    }

    private void OnDrawGizmos()
    {
        if (parameter.attackPoint != null)
            Gizmos.DrawWireSphere(parameter.attackPoint.position, parameter.attackArea);
    }


    public BossBase GetBossBase() => bossBase;
}