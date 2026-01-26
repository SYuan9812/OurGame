using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerHealth : MonoBehaviour, IDamageable
{
    [Header("Config")]
    [SerializeField] private PlayerStats stats;


    [Header("damage effect")]  
    [SerializeField] private SpriteRenderer spriteRenderer;
    [SerializeField] private Color hurtColor = Color.red;
    [SerializeField] private float hurtFlashTime = 0.2f;


    [Header("heal effect")]
    [SerializeField] private Color healColor = Color.green;
    [SerializeField] private float healFlashTime = 0.2f;

    [Header("Death Settings")]
    [SerializeField] private string titleSceneName = "Title Scene";
    [SerializeField] private float deathDelay = 1.5f;
    [SerializeField] private bool disableInputOnDeath = true;

    private Color originalColor; 
    private PlayerAnimations playerAnimations;
    public bool isDead = false;  

    
    public System.Action<float> OnHealthChanged;
    public System.Action OnPlayerDead;

    private bool isInitialized = false;
    private bool hasInitiatedDeath = false;

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

        isInitialized = true;
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
        if (isDead || hasInitiatedDeath) return;

        hasInitiatedDeath = true;
        isDead = true;

        if (disableInputOnDeath)
        {
            PlayerMovement playerMovement = GetComponent<PlayerMovement>();
            if (playerMovement != null)
            {
                playerMovement.enabled = false;
            }
        }

        playerAnimations.SetDeadAnimation();
        OnPlayerDead?.Invoke();

        StartCoroutine(LoadTitleSceneAfterDelay());
    }

    private IEnumerator LoadTitleSceneAfterDelay()
    {
        yield return new WaitForSeconds(deathDelay);

        if (string.IsNullOrEmpty(titleSceneName))
        {
            yield break;
        }

        SceneManager.LoadScene(titleSceneName);
    }

    public void Heal(float healAmount)
    {
        if (isDead || !isInitialized) return;

        stats.Health += healAmount;
        stats.Health = Mathf.Min(stats.Health, stats.MaxHealth);
        OnHealthChanged?.Invoke(stats.Health);

        StartCoroutine(HealFlash()); // Flash green when healing
    }

    private IEnumerator HealFlash()
    {
        spriteRenderer.color = healColor;
        yield return new WaitForSeconds(healFlashTime);
        spriteRenderer.color = originalColor;
    }
}