using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

[RequireComponent(typeof(Canvas))]
public class AutoSetRenderCamera : MonoBehaviour
{
    private Canvas targetCanvas;

    private void Awake()
    {
        targetCanvas = GetComponent<Canvas>();
        SceneManager.sceneLoaded += OnSceneLoaded;
        UpdateRenderCamera();
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        UpdateRenderCamera();
    }

    private void UpdateRenderCamera()
    {
        Camera mainCam = Camera.main;
        if (mainCam == null)
        {
            mainCam = FindObjectOfType<Camera>();
            if (mainCam == null)
            {
                return;
            }
        }

        targetCanvas.worldCamera = mainCam;
    }

    private void OnDestroy()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }
}
