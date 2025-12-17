//Attack state

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum EnemyDirection { Up, Down, Left, Right }

public class DecisionAttackRange : FSMDecision
{
    [Header("Config")]
    [SerializeField] private LayerMask playerMask;
    [SerializeField] private Transform upColliderObj;
    [SerializeField] private Transform downColliderObj;
    [SerializeField] private Transform leftColliderObj;
    [SerializeField] private Transform rightColliderObj;

    private PolygonCollider2D currentAttackCollider;
    private EnemyBrain enemy;

    private void Awake()
    {
        enemy = GetComponent<EnemyBrain>();

        // 检测核心组件
        if (enemy == null)
            Debug.LogError($"【攻击判定】{gameObject.name} 缺失 EnemyBrain 组件！", this);

        // 检测子物体是否未赋值
        CheckUnassignedColliderObjs();
    }

    // 删除错误的Update方法！朝向更新不在判定脚本中处理

    public override bool Decide()
    {

        UpdateCurrentColliderByDirection();
        bool isInRange = PlayerInAttackRange();

        // 仅在判定为True时打印（避免刷屏）

        return isInRange;
    }

    private void UpdateCurrentColliderByDirection()
    {
        // 核心修改：直接从EnemyBrain获取朝向，无需Animator
        EnemyDirection currentDir = enemy.CurrentDirection;

        // 空安全访问碰撞箱
        switch (currentDir)
        {
            case EnemyDirection.Up:
                currentAttackCollider = upColliderObj?.GetComponent<PolygonCollider2D>();
                break;
            case EnemyDirection.Down:
                currentAttackCollider = downColliderObj?.GetComponent<PolygonCollider2D>();
                break;
            case EnemyDirection.Left:
                currentAttackCollider = leftColliderObj?.GetComponent<PolygonCollider2D>();
                break;
            case EnemyDirection.Right:
                currentAttackCollider = rightColliderObj?.GetComponent<PolygonCollider2D>();
                break;
            default:
                currentAttackCollider = downColliderObj?.GetComponent<PolygonCollider2D>();
                break;
        }

        if (currentAttackCollider == null)
            Debug.LogWarning($"【攻击判定】{currentDir} 方向碰撞箱缺失/未赋值！", this);
    }

    private bool PlayerInAttackRange()
    {
        // 全链路空值校验（保留）
        if (currentAttackCollider == null)
        {
            Debug.LogWarning($"【攻击判定】当前朝向{enemy.CurrentDirection}的碰撞箱为空！", this);
            return false;
        }

        Collider2D playerCollider = enemy.Player.GetComponent<Collider2D>();
        if (playerCollider == null)
        {
            Debug.LogError($"【攻击判定】玩家无Collider2D组件！", this);
            return false;
        }

        // 替换为OverlapCollider检测（适配Trigger=true）
        ContactFilter2D filter = new ContactFilter2D();
        filter.SetLayerMask(playerMask); // 只检测玩家层
        List<Collider2D> hitColliders = new List<Collider2D>();

        // 检测碰撞箱范围内的玩家
        currentAttackCollider.OverlapCollider(filter, hitColliders);
        foreach (var coll in hitColliders)
        {
            if (coll.transform == enemy.Player)
            {
                return true;
            }
        }
        return false;
    }

    // 辅助方法：检测未赋值的子物体
    private void CheckUnassignedColliderObjs()
    {
        List<string> unassigned = new List<string>();
        if (upColliderObj == null) unassigned.Add("upColliderObj");
        if (downColliderObj == null) unassigned.Add("downColliderObj");
        if (leftColliderObj == null) unassigned.Add("leftColliderObj");
        if (rightColliderObj == null) unassigned.Add("rightColliderObj");

        if (unassigned.Count > 0)
            Debug.LogError($"【攻击判定】请在Inspector赋值以下字段：{string.Join(", ", unassigned)}", this);
    }

    // Gizmos绘制当前碰撞箱
    private void OnDrawGizmosSelected()
    {
        if (currentAttackCollider != null)
        {
            Gizmos.color = Color.red;
            Vector2[] points = currentAttackCollider.points;
            Transform collTrans = currentAttackCollider.transform;
            for (int i = 0; i < points.Length; i++)
            {
                int nextIndex = (i + 1) % points.Length;
                Vector3 worldPointA = collTrans.TransformPoint(points[i]);
                Vector3 worldPointB = collTrans.TransformPoint(points[nextIndex]);
                Gizmos.DrawLine(worldPointA, worldPointB);
            }
        }
    }
}