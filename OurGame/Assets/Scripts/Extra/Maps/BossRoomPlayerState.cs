using UnityEngine;

public class BossRoomPlayerState : MonoBehaviour
{
    public static BossRoomPlayerState Instance;

    [Header("Boss Room State")]
    public bool isPlayerInBossRoom = false;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }
}