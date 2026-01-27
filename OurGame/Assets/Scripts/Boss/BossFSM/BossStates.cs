using UnityEngine;
using System.Collections;
using System.Collections.Generic;

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
        wanderDir = new Vector2(Random.Range(-1f, 1f), Random.Range(-1f, 1f)).normalized;
        manager.GetBossBase().SetMoveDirection(wanderDir);
    }

    public void OnUpdate()
    {
        wanderTimer += Time.deltaTime;
        if (wanderTimer >= parameter.wanderDuration)
        {
            manager.TransitionState(Random.value < parameter.wanderToRangeChance ? StateType.Range : StateType.Charge);
        }
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

        HashSet<IDamageable> damagedTargets = new HashSet<IDamageable>();

        foreach (Collider2D target in hitTargets)
        {
            IDamageable damageable = target.GetComponent<IDamageable>();
            if (damageable != null && !damagedTargets.Contains(damageable))
            {
                damagedTargets.Add(damageable);
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
    private Vector3 targetPos;
    private float stateTimer;

    public RangeState(BossFSM manager)
    {
        this.manager = manager;
        this.parameter = manager.parameter;
    }

    public void OnEnter()
    {
        manager.GetBossBase().RangedAttack();

        if (parameter.target != null)
        {
            targetPos = parameter.target.position;
        }

        stateTimer = 0f;
        SpawnWarningCircle();
    }

    public void OnUpdate()
    {
        stateTimer += Time.deltaTime;

        if (stateTimer >= parameter.rangeWarningDuration + 1f)
        {
            manager.TransitionState(StateType.Chase);
        }
    }

    private void SpawnWarningCircle()
    {
        if (parameter.rangeWarningPrefab == null)
        {
            manager.TransitionState(StateType.Chase);
            return;
        }

        GameObject warning = UnityEngine.Object.Instantiate(
            parameter.rangeWarningPrefab,
            targetPos,
            Quaternion.identity
        );

        WarningCircle warningScript = warning.GetComponent<WarningCircle>();
        if (warningScript != null)
        {
            warningScript.explodePrefab = parameter.rangeExplodePrefab;
            warningScript.warningDuration = parameter.rangeWarningDuration;
            warningScript.explodeRadius = 1.5f;
            warningScript.explodeDamage = parameter.rangeExplodeDamage;
            warningScript.targetLayer = parameter.targetLayer;
        }
    }

    public void OnExit()
    {
        UnityEngine.Object.Destroy(GameObject.FindGameObjectWithTag("WarningCircle"));
    }
}




public class ChargeState : BossState
{
    private BossFSM manager;
    private Parameter parameter;
    private Vector2 chargeDirection;
    private bool isCharging;

    public ChargeState(BossFSM manager)
    {
        this.manager = manager;
        this.parameter = manager.parameter;
    }

    public void OnEnter()
    {
        isCharging = false;
        chargeDirection = parameter.target != null
            ? (parameter.target.position - manager.transform.position).normalized
            : Vector2.right;

        manager.GetBossBase().SetMoveDirection(Vector2.zero);
        manager.StartCoroutine(CopyColliderAfterFrame());
    }

    private IEnumerator CopyColliderAfterFrame()
    {
        yield return null;

        PolygonCollider2D bossCollider = manager.GetComponent<PolygonCollider2D>();
        Hitbox hitbox = manager.GetComponentInChildren<Hitbox>();

        if (bossCollider != null && hitbox != null)
        {
            hitbox.ResetDamageFlag();
            hitbox.CopyColliderFromBoss(bossCollider);
            hitbox.chargeDamage = (int)parameter.chargeDamage;
        }
    }

    public void OnUpdate()
    {
        if (!isCharging)
        {
            isCharging = true;
            manager.GetBossBase().SetMoveDirection(chargeDirection);
        }
        if (manager.GetBossBase().IsAtBoundary())
        {
            manager.GetBossBase().SetMoveSpeed(0);
            manager.TransitionState(StateType.Chase);
        }
    }

    public void OnExit() { }
}