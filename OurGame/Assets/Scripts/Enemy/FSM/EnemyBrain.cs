using UnityEngine;

public class EnemyBrain : MonoBehaviour
{
    [SerializeField] public string initState;
    [SerializeField] private FSMState[] states;

    [Header("Core Settings")]
    [SerializeField] private Transform player;
    [SerializeField] private string playerTag = "Player";

    public FSMState CurrentState { get; set; }
    public Transform Player { get; set; }
    public EnemyDirection CurrentDirection;

    public bool forceStateLock = false; // Lock to prevent state changes

    private void Start()
    {
        if (Player == null)
        {
            GameObject playerObj = GameObject.FindWithTag(playerTag);
            if (playerObj != null)
            {
                Player = playerObj.transform;
            }
            else
            {
                Debug.LogError($"¡¾EnemyBrain¡¿{gameObject.name} Player not found! Check player tag is set to 'Player'", this);
            }
        }
        ChangeState(initState);
    }

    private void Update()
    {
        if (forceStateLock)
        {
            CurrentState?.UpdateState(enemyBrain: this);
            return; // Skip state change logic when locked
        }

        CurrentState?.UpdateState(enemyBrain: this);

        // Update facing direction towards player
        if (Player != null)
        {
            Vector2 dirToPlayer = (Player.position - transform.position).normalized;
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

    public void ChangeState(string newStateID, bool force = false)
    {
        // If state is locked and not forced, do nothing
        if (forceStateLock && !force) return;

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
    }
}
