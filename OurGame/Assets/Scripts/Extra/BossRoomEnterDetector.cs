using UnityEngine;

[RequireComponent(typeof(Collider2D), typeof(Rigidbody2D))]
public class BossRoomEnterDetector : MonoBehaviour
{
    private Collider2D triggerCollider;

    private void Awake()
    {
        triggerCollider = GetComponent<Collider2D>();
        if (!triggerCollider.isTrigger)
        {
            triggerCollider.isTrigger = true;
        }

        Rigidbody2D rb = GetComponent<Rigidbody2D>();
        rb.gravityScale = 0;
        rb.isKinematic = true;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player") && BossRoomPlayerState.Instance != null)
        {
            BossRoomPlayerState.Instance.isPlayerInBossRoom = true;
        }
    }
}