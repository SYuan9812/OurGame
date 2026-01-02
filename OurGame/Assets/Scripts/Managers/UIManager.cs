using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

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

    [Header("Level Progress")]
    [SerializeField] private Image PBar; //Level progress
    [SerializeField] private TextMeshProUGUI PBarText; //Level progress text


    private void Awake()
    {
        HBar = GameObject.Find("HealthBar")?.GetComponent<Image>();
        EBar = GameObject.Find("ExpBar")?.GetComponent<Image>();
        SBar = GameObject.Find("Stamina")?.GetComponent<Image>();

        levelTMP = GameObject.Find("Level TMP")?.GetComponent<TextMeshProUGUI>();
        healthTMP = GameObject.Find("HealthText")?.GetComponent<TextMeshProUGUI>();
        expTMP = GameObject.Find("ExpText")?.GetComponent<TextMeshProUGUI>();

        PBar = GameObject.Find("LevelProgress")?.GetComponent<Image>();
        PBarText = GameObject.Find("LevelText")?.GetComponent<TextMeshProUGUI>();

        if (playerHealth == null) playerHealth = FindAnyObjectByType<PlayerHealth>();
        if (playerExperience == null) playerExperience = FindAnyObjectByType<PlayerExperience>();
        if (playerMovement == null) playerMovement = FindAnyObjectByType<PlayerMovement>();

        if (SBar != null)
            SBar.color = stats.isStaminaLocked ? staminaLockedColor : staminaNormalColor;
    }

    private void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        //Finding objects after scene transition
        HBar = GameObject.Find("HealthBar")?.GetComponent<Image>();
        EBar = GameObject.Find("ExpBar")?.GetComponent<Image>();
        SBar = GameObject.Find("Stamina")?.GetComponent<Image>();

        levelTMP = GameObject.Find("Level TMP")?.GetComponent<TextMeshProUGUI>();
        healthTMP = GameObject.Find("HealthText")?.GetComponent<TextMeshProUGUI>();
        expTMP = GameObject.Find("ExpText")?.GetComponent<TextMeshProUGUI>();

        PBar = GameObject.Find("LevelProgress")?.GetComponent<Image>();
        PBarText = GameObject.Find("LevelText")?.GetComponent<TextMeshProUGUI>();

        if (SBar != null)
            SBar.color = stats.isStaminaLocked ? staminaLockedColor : staminaNormalColor;
    }

    void Start()
    {

        stats.ResetPlayer(true); // 只在第一次加载时执行


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


    public void UpdatePlayerGUI ()
    {
        UpdateHealthUI(stats.Health);
        UpdateExpLevelUI();
        UpdateStaminaUI();
        if (LevelManager.Instance != null && PBar != null) //Update level progress bar
        {
            PBar.fillAmount = (float)LevelManager.Instance.currentEnemiesKilled / LevelManager.Instance.totalEnemiesToKill;

            if (PBarText != null)
                PBarText.text = $"{LevelManager.Instance.currentEnemiesKilled} / {LevelManager.Instance.totalEnemiesToKill}";
        }
    }
}
