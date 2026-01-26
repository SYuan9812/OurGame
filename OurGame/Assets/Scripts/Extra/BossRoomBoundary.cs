using UnityEngine;

public class BossRoomWallController : MonoBehaviour
{
    [Header("Settings")]
    [Tooltip("Walls")]
    public GameObject walls;
    [Tooltip("Boss")]
    public BossBase boss;

    private void Awake()
    {
        if (walls != null)
        {
            walls.SetActive(false);
        }

        if (boss == null)
        {
            boss = FindObjectOfType<BossBase>();
        }
    }

    private void Update()
    {
        if (BossRoomPlayerState.Instance != null && boss != null)
        {
            if (BossRoomPlayerState.Instance.isPlayerInBossRoom && !boss.IsDead())
            {
                ShowWalls();
            }
            else
            {
                HideWalls();
            }
        }
    }

    private void ShowWalls()
    {
        if (walls != null && !walls.activeSelf)
        {
            walls.SetActive(true);
        }
    }

    private void HideWalls()
    {
        if (walls != null && walls.activeSelf)
        {
            walls.SetActive(false);
        }
    }
}