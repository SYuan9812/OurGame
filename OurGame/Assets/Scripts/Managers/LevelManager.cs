using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class LevelManager : MonoBehaviour
{
    public enum LevelType { EnemyCount, BossFight, FinalLevel }
    public static LevelManager Instance;

    [Header("Global Data")]
    public PlayerStats playerStats;

    [Header("Level Settings")]
    public LevelType currentLevelType = LevelType.EnemyCount;
    public int totalEnemiesToKill = 5; //Enemies needed to kill
    public int currentEnemiesKilled = 0;
    public bool canAdvanceToNextLevel = false;

    [Header("UI Settings")]
    public UnityEngine.UI.Image progressBar; //Progress bar

    [Header("Scene Settings")]
    public string nextSceneName;
    public string finalSceneName = "End Scene";
    public float sceneLoadDelay = 1f;

    [Header("Final Level Animation")]
    public PlayerAnimations playerAnimations;
    public string endAnimationTrigger = "TheEnd";
    private int endAnimationTriggerHash;
    private bool isPlayingEndAnimation = false;

    private bool isBossKilled = false;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            SceneManager.sceneLoaded += OnSceneLoaded;
            endAnimationTriggerHash = Animator.StringToHash(endAnimationTrigger);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        UpdateProgressBar();
        if (playerAnimations == null)
        {
            playerAnimations = FindObjectOfType<PlayerAnimations>();
        }
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

    public void OnBossKilled()
    {
        if (currentLevelType != LevelType.BossFight && currentLevelType != LevelType.FinalLevel) return;

        isBossKilled = true;
        canAdvanceToNextLevel = true;
        if (currentLevelType == LevelType.FinalLevel)
        {
            Debug.Log("Final Boss defeated! Press L to enter portal.");
        }
        else
        {
            Debug.Log("Boss defeated! Press L at campfire to advance.");
        }

        if (progressBar != null) progressBar.gameObject.SetActive(false);
    }

    private void UpdateProgressBar()
    {
        if (currentLevelType == LevelType.BossFight || currentLevelType == LevelType.FinalLevel || progressBar == null) return;

        if (progressBar != null)
        {
            progressBar.fillAmount = (float)currentEnemiesKilled / totalEnemiesToKill;
        }
    }

    public void AdvanceToNextLevel()
    {
        if (!canAdvanceToNextLevel || isPlayingEndAnimation) return;

        if (currentLevelType == LevelType.FinalLevel)
        {
            StartCoroutine(PlayEndAnimationThenLoadScene());
        }
        else
        {
            SavePlayerData();
            StartCoroutine(LoadNextSceneWithDelay());
        }
    }

    private IEnumerator PlayEndAnimationThenLoadScene()
    {
        isPlayingEndAnimation = true;

        if (playerAnimations != null && playerAnimations.GetComponent<Animator>() != null)
        {
            Animator playerAnimator = playerAnimations.GetComponent<Animator>();
            playerAnimator.SetTrigger(endAnimationTriggerHash);

            AnimationClip[] clips = playerAnimator.runtimeAnimatorController.animationClips;
            float animationLength = 1.017f;
            foreach (var clip in clips)
            {
                if (clip.name.Contains("TheEnd"))
                {
                    animationLength = clip.length;
                    break;
                }
            }

            if (animationLength <= 0)
            {
                animationLength = sceneLoadDelay;
            }

            yield return new WaitForSeconds(animationLength);
        }
        else
        {
            Debug.LogWarning("Player Animator not found! Loading scene directly.");
            yield return new WaitForSeconds(sceneLoadDelay);
        }

        SavePlayerData();
        SceneManager.LoadScene(finalSceneName);
        isPlayingEndAnimation = false;
    }

    private IEnumerator LoadNextSceneWithDelay()
    {
        yield return new WaitForSeconds(sceneLoadDelay);
        SceneManager.LoadScene(nextSceneName);
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        currentEnemiesKilled = 0;
        canAdvanceToNextLevel = false;
        isBossKilled = false;

        LoadPlayerData();

        ConfigureLevelBySceneName(scene.name);

        progressBar = FindObjectOfType<UnityEngine.UI.Image>();
        UpdateProgressBar();

        playerAnimations = FindObjectOfType<PlayerAnimations>();

        UIManager uiManager = FindObjectOfType<UIManager>();
        if (uiManager != null)
        {
            uiManager.UpdatePlayerGUI();
        }
    }

    private void ConfigureLevelBySceneName(string sceneName)
    {
        if (sceneName.Contains("Scene 1") || sceneName.Contains("Level 1"))
        {
            currentLevelType = LevelType.EnemyCount;
            if (progressBar != null) progressBar.gameObject.SetActive(true);
        }
        else if (sceneName.Contains("Scene 2") || sceneName.Contains("Level 2"))
        {
            currentLevelType = LevelType.FinalLevel;
            if (progressBar != null) progressBar.gameObject.SetActive(false);
        }
        // Add more scenes here
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