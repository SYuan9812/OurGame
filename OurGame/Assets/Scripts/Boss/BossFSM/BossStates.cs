using UnityEngine;

public class IdleState : BossState
{
    private BossFSM manager;
    private Parameter parameter;
    private float timer;

    public IdleState(BossFSM manager)
    {
        this.manager = manager;
        this.parameter = manager.parameter;
    }

    public void OnEnter() => timer = 0;
    public void OnUpdate()
    {
        timer += Time.deltaTime;
        if (parameter.target != null)
            manager.TransitionState(StateType.Chase);
    }
    public void OnExit() { }
}

public class ChaseState : BossState
{
    private BossFSM manager;
    private Parameter parameter;
    private float chaseTimer;

    public ChaseState(BossFSM manager)
    {
        this.manager = manager;
        this.parameter = manager.parameter;
    }

    public void OnEnter() => chaseTimer = 0;

    public void OnUpdate()
    {
        if (parameter.target == null)
        {
            manager.TransitionState(StateType.Idle);
            return;
        }

        Vector2 dirToPlayer = (parameter.target.position - parameter.attackPoint.position).normalized;
        manager.GetBossBase().SetMoveDirection(dirToPlayer);


        manager.FlipTo(parameter.target);


        if (Physics2D.OverlapCircle(parameter.attackPoint.position, parameter.attackArea, parameter.targetLayer) &&
            !manager.IsMeleeCoolingDown())
        {
            manager.TransitionState(StateType.Melee);
            return;
        }

        chaseTimer += Time.deltaTime;
        if (chaseTimer >= parameter.chaseDuration)
            manager.TransitionState(StateType.Wander);
    }

    public void OnExit() { }
}

public class WanderState : BossState
{
    private BossFSM manager;
    private Parameter parameter;
    private float wanderTimer;
    private Vector2 wanderDir;

    public WanderState(BossFSM manager)
    {
        this.manager = manager;
        this.parameter = manager.parameter;
    }

    public void OnEnter()
    {
        wanderTimer = 0;
        wanderDir = new Vector2(UnityEngine.Random.Range(-1f, 1f), UnityEngine.Random.Range(-1f, 1f)).normalized;
        manager.GetBossBase().SetMoveDirection(wanderDir);
    }
    public void OnUpdate()
    {
        wanderTimer += Time.deltaTime;
        if (wanderTimer >= parameter.wanderDuration)
            manager.TransitionState(StateType.Range);
    }
    public void OnExit() { }
}

public class MeleeState : BossState
{
    private BossFSM manager;
    private Parameter parameter;
    private bool hasAttacked;
    private bool damageDealt;

    public MeleeState(BossFSM manager)
    {
        this.manager = manager;
        this.parameter = manager.parameter;
    }

    public void OnEnter()
    {
        hasAttacked = false;
        damageDealt = false;
        manager.GetBossBase().MeleeAttack();

        DealMeleeDamage();
    }

    public void OnUpdate()
    {
        AnimatorStateInfo stateInfo = parameter.anim.GetCurrentAnimatorStateInfo(0);

        if (stateInfo.normalizedTime >= 0.3f && stateInfo.normalizedTime < 0.8f && !damageDealt)
        {
            DealMeleeDamage();
            damageDealt = true;
        }

        if (!hasAttacked && stateInfo.normalizedTime >= 1f)
        {
            hasAttacked = true;
            manager.TransitionState(StateType.Chase);
        }
    }

    private void DealMeleeDamage()
    {
        Collider2D[] hitTargets = Physics2D.OverlapCircleAll(
            parameter.attackPoint.position,
            parameter.attackArea,
            parameter.targetLayer
        );

        if (hitTargets.Length == 0)
        {
            return;
        }

        foreach (Collider2D target in hitTargets)
        {
            IDamageable damageable = target.GetComponent<IDamageable>();
            if (damageable != null)
            {
                damageable.TakeDamage(parameter.meleeDamage);
            }
        }
    }

    public void OnExit() { }
}

public class RangeState : BossState
{
    private BossFSM manager;
    private Parameter parameter;
    private bool hasFired;

    public RangeState(BossFSM manager)
    {
        this.manager = manager;
        this.parameter = manager.parameter;
    }

    public void OnEnter()
    {
        hasFired = false;
        manager.GetBossBase().RangedAttack();
    }
    public void OnUpdate()
    {
        if (!hasFired && parameter.anim.GetCurrentAnimatorStateInfo(0).normalizedTime >= 1)
        {
            hasFired = true;
            manager.TransitionState(StateType.Chase);
        }
    }
    public void OnExit() { }
}