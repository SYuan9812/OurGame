using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GamePauseManager : MonoBehaviour
{
    [Header("Pause Settings")]
    [Tooltip("Pause Key")]
    public KeyCode pauseKey = KeyCode.Escape;
    [Tooltip("Allow Toggle")]
    public bool allowToggle = true;

    [Header("References")]
    [Tooltip("Player Movement")]
    public PlayerMovement playerMovement;
    [Tooltip("Weapon System")]
    public PlayerWeaponManager playerWeaponManager;
    [Tooltip("Pause Panel")]
    public GameObject pausePanel;

    [Header("Gray Screen Settings")]
    public GameObject grayScreenPanel;
    [Range(0f, 1f)]
    public float grayScreenAlpha = 0.7f;

    private CanvasGroup grayScreenCanvasGroup;
    public bool IsGamePaused { get; private set; } = false;

    private void Awake()
    {
        if (grayScreenPanel != null)
        {
            grayScreenCanvasGroup = grayScreenPanel.GetComponent<CanvasGroup>();
            if (grayScreenCanvasGroup == null)
            {
                grayScreenCanvasGroup = grayScreenPanel.AddComponent<CanvasGroup>();
            }

            grayScreenCanvasGroup.alpha = 0f;
            grayScreenPanel.SetActive(false);
        }

        if (pausePanel != null)
        {
            pausePanel.SetActive(false);
        }
    }

    private void Update()
    {
        if (Input.GetKeyDown(pauseKey))
        {
            if (allowToggle)
            {
                TogglePause();
            }
            else if (!IsGamePaused)
            {
                // No toggle allowed
                PauseGame();
            }
        }
    }

    public void TogglePause()
    {
        if (IsGamePaused)
        {
            ResumeGame();
        }
        else
        {
            PauseGame();
        }
    }

    public void PauseGame()
    {
        if (IsGamePaused) return;

        Time.timeScale = 0f;
        Time.fixedDeltaTime = 0f;

        if (playerMovement != null) playerMovement.enabled = false;
        if (playerWeaponManager != null) playerWeaponManager.enabled = false;
        if (pausePanel != null) pausePanel.SetActive(true);
        ShowGrayScreen();

        IsGamePaused = true;
    }

    public void ResumeGame()
    {
        if (!IsGamePaused) return;

        Time.timeScale = 1f;
        Time.fixedDeltaTime = Time.timeScale * 0.02f;

        if (playerMovement != null) playerMovement.enabled = true;
        if (playerWeaponManager != null) playerWeaponManager.enabled = true;
        if (pausePanel != null) pausePanel.SetActive(false);
        HideGrayScreen();

        IsGamePaused = false;
    }

    private void ShowGrayScreen()
    {
        if (grayScreenPanel == null || grayScreenCanvasGroup == null) return;

        grayScreenPanel.SetActive(true);
        grayScreenCanvasGroup.alpha = grayScreenAlpha;
    }

    private void HideGrayScreen()
    {
        if (grayScreenPanel == null || grayScreenCanvasGroup == null) return;

        grayScreenCanvasGroup.alpha = 0f;
        grayScreenPanel.SetActive(false);
    }

    private void OnApplicationQuit()
    {
        Time.timeScale = 1f;
        Time.fixedDeltaTime = 0.02f;
    }
}
