using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class WarningCircle : MonoBehaviour
{
    public GameObject explodePrefab;
    public float warningDuration = 1.5f;   
    public float explodeRadius = 1.5f;    
    public float explodeDamage = 10f;     
    public LayerMask targetLayer;          

    void Start()
    {
        StartCoroutine(DestroyAfterTime());
    }

    IEnumerator DestroyAfterTime()
    {
        yield return new WaitForSeconds(warningDuration);

        if (explodePrefab != null)
        {
            GameObject explode = Instantiate(
                explodePrefab,
                transform.position,
                Quaternion.identity
            );

            SpriteRenderer sr = explode.GetComponent<SpriteRenderer>();
            if (sr != null)
            {
                sr.sortingOrder = 10;
                sr.color = new Color(1f, 1f, 1f, 1f);
            }

            Collider2D[] hitTargets = Physics2D.OverlapCircleAll(
                transform.position,
                explodeRadius,
                targetLayer
            );

            HashSet<IDamageable> damagedTargets = new HashSet<IDamageable>();

            foreach (Collider2D target in hitTargets)
            {
                IDamageable damageable = target.GetComponent<IDamageable>();
                if (damageable != null && !damagedTargets.Contains(damageable))
                {
                    damagedTargets.Add(damageable);
                    damageable.TakeDamage(explodeDamage);
                }
            }
            Destroy(explode, 3f);
        }
        Destroy(gameObject);
    }
}
