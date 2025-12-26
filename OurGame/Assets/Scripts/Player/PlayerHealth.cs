using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerHealth : MonoBehaviour, IDamageable
{
    [Header("Config")]
    [SerializeField] private PlayerStats stats;

    [Header("damage effect")]  
    [SerializeField] private SpriteRenderer spriteRenderer;
    [SerializeField] private Color hurtColor = Color.red;
    [SerializeField] private float hurtFlashTime = 0.2f;

    private Color originalColor; 
    private PlayerAnimations playerAnimations;
    public bool isDead = false;  

    
    public System.Action<float> OnHealthChanged;
    public System.Action OnPlayerDead;

    private void Awake()
    {
        playerAnimations = GetComponent<PlayerAnimations>();
        //
        if (spriteRenderer == null)
            spriteRenderer = GetComponent<SpriteRenderer>();
        originalColor = spriteRenderer.color;

        if (stats != null)
        {
            stats.Health = stats.MaxHealth;
            OnHealthChanged?.Invoke(stats.Health); // Update UI on start
        }
    }

    private void Update()
    {
        
    }

    public void TakeDamage(float amount)
    {
        if (isDead) return;  // 
        if (stats.Health <= 0f) return;
        DamageManager.Instance.ShowDamageText(amount, transform);
        stats.Health -= amount;
        stats.Health = Mathf.Max(stats.Health, 0f);

        OnHealthChanged?.Invoke(stats.Health);  // 
        StartCoroutine(HurtFlash());  // 

        if (stats.Health <= 0f)
        {
            stats.Health = 0f;
            PlayerDead();
        }
    }

    // 
    private IEnumerator HurtFlash()
    {
        spriteRenderer.color = hurtColor;
        yield return new WaitForSeconds(hurtFlashTime);
        spriteRenderer.color = originalColor;
    }

    private void PlayerDead()
    {
        if (isDead) return;
        isDead = true;  //
        playerAnimations.SetDeadAnimation();
        OnPlayerDead?.Invoke();
    }

    //
    public void Heal(float healAmount)
    {
        if (isDead) return;

        stats.Health += healAmount;
        stats.Health = Mathf.Min(stats.Health, stats.MaxHealth);
        OnHealthChanged?.Invoke(stats.Health);
    }
}