using UnityEngine;

public class Hitbox : MonoBehaviour
{
    private BossFSM bossFSM;
    private PolygonCollider2D hitboxCollider;
    public float chargeDamage;
    private bool hasDealtDamage;

    void Awake()
    {
        bossFSM = transform.parent.GetComponent<BossFSM>();
        hasDealtDamage = false;
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (hasDealtDamage) return;

        if (other.CompareTag("Player"))
        {
            IDamageable damageable = other.GetComponent<IDamageable>();
            if (damageable != null)
            {
                damageable.TakeDamage(chargeDamage);
                hasDealtDamage = true;
                bossFSM.TransitionState(StateType.Chase);
            }
        }
    }

    public void CopyColliderFromBoss(PolygonCollider2D sourceCollider)
    {
        if (sourceCollider == null) return;

        if (hitboxCollider == null)
        {
            hitboxCollider = gameObject.AddComponent<PolygonCollider2D>();
        }

        hitboxCollider.points = sourceCollider.points;
        hitboxCollider.offset = sourceCollider.offset;
        hitboxCollider.isTrigger = true;
    }

    public void ResetDamageFlag()
    {
        hasDealtDamage = false;
    }
}