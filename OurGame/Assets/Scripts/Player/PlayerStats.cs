using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "PlayerStats", menuName = "Player Stats")]
public class PlayerStats : ScriptableObject
{
    [Header("Config")]
    public int Level;

    [Header("Health")]
    public float Health;
    [SerializeField] private float baseMaxHealth = 10f;
    public float MaxHealth;

    [Header("Stamina")]
    [SerializeField] private float baseMaxStamina = 40f;
    public float CurrentStamina;
    public float MaxStamina;
    public float staminaConsumeRate = 20f; //per sec
    public float staminaRecoverRate = 10f; //per sec
    public bool isStaminaLocked; //player can't run before stamina is max when stamina clears

    [Header("Exp")]
    public float CurrentExp;
    public float NextLevelExp;
    public float InitialNextLevelExp; //Exp needed to reach lv2 (used for initialization)
    [Range(1f, 100f)]public float ExpMultiplier;

    private void OnEnable()
    {
        // Only initialize if MaxHealth is not set
        if (MaxHealth <= 0f)
        {
            MaxHealth = baseMaxHealth;
        }

        if (MaxStamina <= 0f)
        {
            MaxStamina = baseMaxStamina;
        }

        isStaminaLocked = false;
    }

    public void ResetPlayer(bool resetLevel = false)
    {
        // Reset health
        MaxHealth = baseMaxHealth;
        Health = MaxHealth;

        // Reset stamina
        MaxStamina = baseMaxStamina;
        CurrentStamina = MaxStamina;
        isStaminaLocked = false;

        // Reset exp and level
        if (resetLevel)
        {
            Level = 1;
            CurrentExp = 0f;
            NextLevelExp = InitialNextLevelExp;
        }
    }
}
