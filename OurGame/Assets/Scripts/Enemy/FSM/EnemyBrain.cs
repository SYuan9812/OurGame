
using UnityEngine;

public class EnemyBrain : MonoBehaviour
{
    [SerializeField] private string initState;
    [SerializeField] private FSMState[] states;

    public FSMState CurrentState { get; set; }
    public Transform Player { get; set; }
    public EnemyDirection CurrentDirection;

    private void Start()
    {
        if (Player == null)
        {
            GameObject playerObj = GameObject.FindWithTag("Player");
            if (playerObj != null)
            {
                Player = playerObj.transform; // 赋值Transform
            }
            else
            {
                Debug.LogError($"【EnemyBrain】{gameObject.name} 未找到玩家！请检查玩家Tag是否为Player", this);
            }
        }
        ChangeState(initState);
    }

    private void Update()
    {
        CurrentState?.UpdateState(enemyBrain: this);

        // 新增：每帧朝向玩家，更新CurrentDirection
        if (Player != null)
        {
            // 计算敌人到玩家的方向
            Vector2 dirToPlayer = (Player.position - transform.position).normalized;
            // 优先上下，再左右（避免斜向移动时朝向混乱）
            if (Mathf.Abs(dirToPlayer.y) > Mathf.Abs(dirToPlayer.x))
            {
                CurrentDirection = dirToPlayer.y > 0 ? EnemyDirection.Up : EnemyDirection.Down;
            }
            else
            {
                CurrentDirection = dirToPlayer.x > 0 ? EnemyDirection.Right : EnemyDirection.Left;
            }
        }
    }

    public void ChangeState(string newStateID)
    {
        FSMState newState = GetState(newStateID);
        if (newState == null) return;
        CurrentState = newState;
    }

    private FSMState GetState(string newStateID)
    {
        for (int i = 0; i < states.Length; i++)
        {
            if (states[i].ID == newStateID)
            {
                return states[i];
            }
        }

        return null;
    }

    public void UpdateDirection(Vector2 moveDir)
    {
        if (moveDir.y > 0)
            CurrentDirection = EnemyDirection.Up;
        else if (moveDir.y < 0)
            CurrentDirection = EnemyDirection.Down;
        else if (moveDir.x < 0)
            CurrentDirection = EnemyDirection.Left;
        else if (moveDir.x > 0)
            CurrentDirection = EnemyDirection.Right;

        // 原有动画切换逻辑（比如SetBool/SetTrigger）保留不变
    }
}
