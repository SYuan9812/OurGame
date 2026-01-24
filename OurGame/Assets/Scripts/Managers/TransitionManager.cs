using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class TransitionManager : MonoBehaviour
{
    [Header("Transition Settings")]
    [Tooltip("Transition Key")]
    public KeyCode triggerKey = KeyCode.Space;

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
        SceneManager.LoadScene("Scene 1");
    }
}
