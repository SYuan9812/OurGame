using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class LevelManager : MonoBehaviour
{
    public static LevelManager Instance;

    [Header("Global Data")]
    public PlayerStats playerStats;

    [Header("Level Settings")]
    public int totalEnemiesToKill = 5; //Enemies needed to kill
    public int currentEnemiesKilled = 0;
    public bool canAdvanceToNextLevel = false;

    [Header("UI Settings")]
    public UnityEngine.UI.Image progressBar; //Progress bar

    [Header("Scene Settings")]
    public string nextSceneName;
    public float sceneLoadDelay = 1f;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            SceneManager.sceneLoaded += OnSceneLoaded;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        UpdateProgressBar();
    }

    public void IncreaseProgress()
    {
        currentEnemiesKilled++;
        currentEnemiesKilled = Mathf.Clamp(currentEnemiesKilled, 0, totalEnemiesToKill);
        UpdateProgressBar();

        if (currentEnemiesKilled == totalEnemiesToKill)
        {
            canAdvanceToNextLevel = true;
            Debug.Log("Level complete! Press L at campfire to advance.");
        }
    }

    private void UpdateProgressBar()
    {
        if (progressBar != null)
        {
            progressBar.fillAmount = (float)currentEnemiesKilled / totalEnemiesToKill;
        }
    }

    public void AdvanceToNextLevel()
    {
        if (canAdvanceToNextLevel && !string.IsNullOrEmpty(nextSceneName))
        {
            SavePlayerData();
            StartCoroutine(LoadNextSceneWithDelay());
        }
    }

    private IEnumerator LoadNextSceneWithDelay()
    {
        yield return new WaitForSeconds(sceneLoadDelay);
        SceneManager.LoadScene(nextSceneName);
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        LoadPlayerData();

        progressBar = FindObjectOfType<UnityEngine.UI.Image>();
        if (progressBar != null)
        {
            UpdateProgressBar();
        }

        UIManager uiManager = FindObjectOfType<UIManager>();
        if (uiManager != null)
        {
            uiManager.UpdatePlayerGUI();
        }
    }

    private void SavePlayerData()
    {
        if (playerStats != null)
        {
            PlayerPrefs.SetInt("PlayerLevel", playerStats.Level);
            PlayerPrefs.SetFloat("PlayerHealth", playerStats.Health);
            PlayerPrefs.SetFloat("PlayerMaxHealth", playerStats.MaxHealth);
            PlayerPrefs.SetFloat("PlayerCurrentExp", playerStats.CurrentExp);
            PlayerPrefs.SetFloat("PlayerNextLevelExp", playerStats.NextLevelExp);
            PlayerPrefs.SetFloat("PlayerCurrentStamina", playerStats.CurrentStamina);
            PlayerPrefs.SetFloat("PlayerMaxStamina", playerStats.MaxStamina);
            PlayerPrefs.Save();
        }
    }

    private void LoadPlayerData()
    {
        if (playerStats != null)
        {
            playerStats.Level = PlayerPrefs.GetInt("PlayerLevel", playerStats.Level);
            playerStats.Health = PlayerPrefs.GetFloat("PlayerHealth", playerStats.Health);
            playerStats.MaxHealth = PlayerPrefs.GetFloat("PlayerMaxHealth", playerStats.MaxHealth);
            playerStats.CurrentExp = PlayerPrefs.GetFloat("PlayerCurrentExp", playerStats.CurrentExp);
            playerStats.NextLevelExp = PlayerPrefs.GetFloat("PlayerNextLevelExp", playerStats.NextLevelExp);
            playerStats.CurrentStamina = PlayerPrefs.GetFloat("PlayerCurrentStamina", playerStats.CurrentStamina);
            playerStats.MaxStamina = PlayerPrefs.GetFloat("PlayerMaxStamina", playerStats.MaxStamina);
        }
    }
}