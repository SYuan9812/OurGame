using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerExperience : MonoBehaviour
{
    [Header("Config")]
    [SerializeField] private PlayerStats stats;
    [SerializeField] private PlayerHealth playerHealth;

    [Header("Level Up Rewards")]
    [SerializeField] private float healthIncreasePerLevel = 2f;
    [SerializeField] private float staminaIncreasePerLevel = 20f;

    private void Awake()
    {
        // Auto-find PlayerHealth component if not assigned
        if (playerHealth == null)
        {
            playerHealth = GetComponent<PlayerHealth>();
        }
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.X))
        {
            AddExp(50f);
        }
    }


    public void AddExp(float amount)
    {
        stats.CurrentExp += amount;

        while (stats.CurrentExp >= stats.NextLevelExp)
        {
            stats.CurrentExp -= stats.NextLevelExp;
            NextLevel();
        }
    }

    private void NextLevel()
    {
        stats.Level++;
        stats.MaxHealth += healthIncreasePerLevel; //Update new max health

        if (playerHealth != null) //Healing player after leveling up
        {
            float healAmount = stats.MaxHealth - stats.Health; // Calculate exact heal needed
            playerHealth.Heal(healAmount);
        }

        stats.MaxStamina += staminaIncreasePerLevel;

        float CurrentExpRequired = stats.NextLevelExp;
        float NewNextLevelExp = Mathf.Round(CurrentExpRequired + stats.NextLevelExp 
            * (stats.ExpMultiplier / 100f));
        stats.NextLevelExp = NewNextLevelExp;
    }
}
