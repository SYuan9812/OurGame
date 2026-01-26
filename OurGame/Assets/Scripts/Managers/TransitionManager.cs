using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class TransitionManager : MonoBehaviour
{
    [Header("Transition Settings")]
    [Tooltip("Transition Key")]
    public KeyCode triggerKey = KeyCode.Space;

    [Header("Scene Names")]
    public string endSceneName = "End Scene";
    public string titleSceneName = "Title Scene";
    public string defaultSceneName = "Scene 1";

    private bool isTransitioning = false;

    void Update()
    {
        if (Input.GetKeyDown(triggerKey) && !isTransitioning)
        {
            TriggerSceneTransition();
        }
    }

    private void TriggerSceneTransition()
    {
        isTransitioning = true;
        LoadTargetScene();
    }

    private void LoadTargetScene()
    {
        string currentSceneName = SceneManager.GetActiveScene().name;

        if (currentSceneName == endSceneName)
        {
            SceneManager.LoadScene(titleSceneName);
        }
        else
        {
            SceneManager.LoadScene(defaultSceneName);
        }

        isTransitioning = false;
    }
}
