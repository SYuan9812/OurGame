using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UIManager : MonoBehaviour
{
    [Header("Stats")]
    [SerializeField] private PlayerStats stats;
    [SerializeField] private PlayerHealth playerHealth; // Reference to health script for event listening
    [SerializeField] private PlayerExperience playerExperience; // Reference to exp script
    [SerializeField] private PlayerMovement playerMovement; // Reference to movement script

    [Header("Bars")]
    [SerializeField] private Image HBar; //Health bar
    [SerializeField] private Image EBar; //Exp bar
    [SerializeField] private Image SBar; //Stamina bar
    [SerializeField] private Color staminaLockedColor = Color.gray; //Colors for stamina state (locked or unlocked)
    [SerializeField] private Color staminaNormalColor = Color.green; //Can run
    [SerializeField] private Color staminaRunningColor = Color.yellow; //Running

    [Header("Texts")]
    [SerializeField] private TextMeshProUGUI levelTMP;
    [SerializeField] private TextMeshProUGUI healthTMP;
    [SerializeField] private TextMeshProUGUI expTMP;

    private void Awake()
    {

        // Auto-find PlayerHealth if not assigned
        if (playerHealth == null)
        {
            playerHealth = FindAnyObjectByType<PlayerHealth>();
        }

        // Auto-find PlayerExperience if not assigned
        if (playerExperience == null)
        {
            playerExperience = FindAnyObjectByType<PlayerExperience>();
        }

        if (playerMovement == null)
        {
            playerMovement = FindAnyObjectByType<PlayerMovement>();
        }

        SBar.color = stats.isStaminaLocked ? staminaLockedColor : staminaNormalColor;
    }

    private void OnEnable()
    {
        //Health change for real-time UI update
        if (playerHealth != null)
        {
            playerHealth.OnHealthChanged += UpdateHealthUI;
        }
    }

    private void OnDisable()
    {
        //Prevent memory leaks
        if (playerHealth != null)
        {
            playerHealth.OnHealthChanged -= UpdateHealthUI;
        }
    }

    void Start()
    {
        stats.ResetPlayer();

        if (playerHealth != null)
        {
            playerHealth.Heal(0f); //Update
        }

        UpdatePlayerGUI();
    }

    void Update()
    {
        UpdatePlayerGUI();
    }


    private void UpdateHealthUI(float currentHealth)
    {
        if (stats == null || HBar == null || healthTMP == null) return;

        // Update health text (round to 0 decimal places for clean display)
        healthTMP.text = $"{Mathf.Round(currentHealth)} / {Mathf.Round(stats.MaxHealth)}";
        // Update health bar fill amount (prevent division by zero)
        HBar.fillAmount = stats.MaxHealth > 0 ? currentHealth / stats.MaxHealth : 0;
    }


    private void UpdateExpLevelUI()
    {
        if (stats == null || EBar == null || levelTMP == null || expTMP == null) return;

        // Update level text
        levelTMP.text = $"Level {stats.Level}";
        // Update exp text (round for clean display)
        expTMP.text = $"{Mathf.Round(stats.CurrentExp)} / {Mathf.Round(stats.NextLevelExp)}";
        // Update exp bar fill amount (prevent division by zero)
        EBar.fillAmount = stats.NextLevelExp > 0 ? stats.CurrentExp / stats.NextLevelExp : 0;
    }


    private void UpdateStaminaUI()
    {
        if (stats == null || SBar == null) return;

        // Update stamina bar fill amount
        SBar.fillAmount = stats.MaxStamina > 0 ? stats.CurrentStamina / stats.MaxStamina : 0;


        if (stats.isStaminaLocked)
        {
            SBar.color = staminaLockedColor;
        }
        else if (playerMovement != null && playerMovement.IsRunning)
        {
            SBar.color = staminaRunningColor;
        }
        else
        {
            SBar.color = staminaNormalColor;
        }
    }


    private void UpdatePlayerGUI ()
    {
        UpdateHealthUI(stats.Health);
        UpdateExpLevelUI();
        UpdateStaminaUI();
    }
}
