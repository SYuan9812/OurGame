using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyBase : MonoBehaviour
{
    public Animator anim;

    // Knockback settings for enemy
    [Header("Knockback Settings")]
    public float knockbackForce = 5f;
    public float knockbackDuration = 0.2f;


    [Header("Health Settings")]
    [SerializeField] private int maxHealth = 100; // Max health of enemy
    [SerializeField] private int currentHealth;   // Current health of enemy
    [SerializeField] private GameObject damageTextPrefab; // Damage text prefab reference
    [SerializeField] private Transform damageTextSpawnPoint; // Spawn position for damage text

    private Rigidbody2D rb;
    private bool isKnockedBack = false;
    private bool isDead = false;
    private EnemyLoot enemyLoot;
    private LevelManager levelManager;

    void Start()
    {
        // Get Rigidbody2D component for physics-based knockback
        rb = GetComponent<Rigidbody2D>();

        currentHealth = maxHealth;

        enemyLoot = GetComponent<EnemyLoot>();
    }


    void Update()
    {
        if (isDead) return;
    }

    public void EnemyTakeDamage(int damage, Vector2 knockbackDirection)
    {
        if (isDead || isKnockedBack || rb == null) return;
        if (currentHealth <= 0) return; // Prevent damage if enemy is already dead

        currentHealth -= damage;
        currentHealth = Mathf.Max(currentHealth, 0); // Ensure health doesn't go below 0

        SpawnDamageText(damage);

        CancelInvoke(nameof(GetHitAnimEnd));
        anim.SetBool("IsHit", true);
        Invoke(nameof(GetHitAnimEnd), 0.6f);

        // Start knockback
        StartCoroutine(KnockbackCoroutine(knockbackDirection));

        if (currentHealth <= 0)
        {
            EnemyDie();
        }
    }

    private void SpawnDamageText(int damage)
    {
        // Return if prefab or spawn point is not assigned
        if (damageTextPrefab == null || damageTextSpawnPoint == null)
        {
            return;
        }

        // Instantiate damage text prefab at spawn position
        GameObject damageTextObj = Instantiate(damageTextPrefab, damageTextSpawnPoint.position, Quaternion.identity);
        // Set damage value to text component
        damageTextObj.GetComponent<DamageText>().SetDamageText(damage);
    }

    private void EnemyDie()
    {
        isDead = true;

        StopAllCoroutines();

        if (rb != null)
        {
            rb.velocity = Vector2.zero; // Stop movement immediately
            rb.isKinematic = true;
        }
        anim.SetTrigger("Dead");

        Collider2D[] allColliders = GetComponentsInChildren<Collider2D>();
        foreach (Collider2D col in allColliders)
        {
            col.enabled = false;
        }

        DropExp();

        LevelManager.Instance.IncreaseProgress();

        // Destroy enemy after death animation
        Destroy(gameObject, 0.417f);
    }

    private IEnumerator KnockbackCoroutine(Vector2 direction)
    {
        isKnockedBack = true;
        float elapsedTime = 0f;
        Vector2 startVelocity = rb.velocity;
        Vector2 knockbackVelocity = direction.normalized * knockbackForce;

        // Apply knockback force over duration
        while (elapsedTime < knockbackDuration)
        {
            elapsedTime += Time.deltaTime;
            float t = elapsedTime / knockbackDuration;
            // Smoothly reduce knockback force
            rb.velocity = Vector2.Lerp(knockbackVelocity, startVelocity, t);
            yield return null;
        }

        // Reset velocity and knockback state
        rb.velocity = startVelocity;
        isKnockedBack = false;
    }


    private void DropExp()
    {
        // Return if EnemyLoot component is missing
        if (enemyLoot == null) return;

        // Find player's PlayerExperience component
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player == null)
        {
            Debug.LogError("Player object with 'Player' tag not found!");
            return;
        }

        PlayerExperience playerExp = player.GetComponent<PlayerExperience>();
        if (playerExp != null)
        {
            // Add exp to player
            playerExp.AddExp(enemyLoot.ExpDrop);
        }
    }

    public void GetHitAnimEnd()
    {
        anim.SetBool("IsHit", false);
    }
}
