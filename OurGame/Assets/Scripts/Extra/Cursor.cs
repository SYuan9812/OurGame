using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Video;
using UnityEngine.SceneManagement;

public class Cursor : MonoBehaviour
{
    [Header("Pause Buttons")]
    public GameObject settingButtonObj;
    public GameObject exitButtonObj;
    public Color hoverColor = new Color(0.8f, 0.8f, 0.8f);

    [Header("Buttons Position")]
    public float settingXMin = -235;
    public float settingXMax = 220;
    public float settingYMin = 33;
    public float settingYMax = 139;

    public float exitXMin = -235;
    public float exitXMax = 220;
    public float exitYMin = -123;
    public float exitYMax = -23;

    [Header("Video Scene Fix")]
    public string titleSceneName = "Title Scene";
    public Canvas uiCanvas;

    private GamePauseManager pauseManager;
    private Color settingOriginalColor;
    private Color exitOriginalColor;
    private Image settingImg;
    private Image exitImg;
    private bool isTitleScene;

    IEnumerator Start()
    {
        yield return new WaitForSeconds(0.1f);

        UnityEngine.Cursor.visible = true;
        UnityEngine.Cursor.lockState = CursorLockMode.Confined;

        isTitleScene = SceneManager.GetActiveScene().name == titleSceneName;

        pauseManager = FindObjectOfType<GamePauseManager>();

        if (settingButtonObj != null)
        {
            settingImg = settingButtonObj.GetComponent<Image>();
            if (settingImg != null) settingOriginalColor = settingImg.color;
        }
        if (exitButtonObj != null)
        {
            exitImg = exitButtonObj.GetComponent<Image>();
            if (exitImg != null) exitOriginalColor = exitImg.color;
        }
    }

    void Update()
    {
        Vector3 mouseWorldPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        mouseWorldPos.z = 0;
        transform.position = mouseWorldPos;

        if (pauseManager != null && pauseManager.IsGamePaused)
        {
            CheckButtonHover();
            CheckButtonClick();
        }
        else
        {
            ResetButtonColor();
        }
    }

    private void CheckButtonHover()
    {
        Vector2 cursorUIPos = GetCursorUIPosition();

        bool isOnSetting = cursorUIPos.x >= settingXMin && cursorUIPos.x <= settingXMax &&
                           cursorUIPos.y >= settingYMin && cursorUIPos.y <= settingYMax;

        if (settingButtonObj != null && isOnSetting)
        {
            if (settingImg != null) settingImg.color = hoverColor;
        }
        else if (settingImg != null)
        {
            settingImg.color = settingOriginalColor;
        }

        bool isOnExit = cursorUIPos.x >= exitXMin && cursorUIPos.x <= exitXMax &&
                        cursorUIPos.y >= exitYMin && cursorUIPos.y <= exitYMax;

        if (exitButtonObj != null && isOnExit)
        {
            if (exitImg != null) exitImg.color = hoverColor;
        }
        else if (exitImg != null)
        {
            exitImg.color = exitOriginalColor;
        }
    }

    private void CheckButtonClick()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Vector2 cursorUIPos = GetCursorUIPosition();

            if (cursorUIPos.x >= settingXMin && cursorUIPos.x <= settingXMax &&
                cursorUIPos.y >= settingYMin && cursorUIPos.y <= settingYMax)
            {
                Debug.Log("Setting clicked");
            }

            if (cursorUIPos.x >= exitXMin && cursorUIPos.x <= exitXMax &&
                cursorUIPos.y >= exitYMin && cursorUIPos.y <= exitYMax)
            {
                Debug.Log("Exit button clicked");
                Application.Quit();
            }
        }
    }

    private Vector2 GetCursorUIPosition()
    {
        RectTransform canvasRect = settingButtonObj.GetComponentInParent<Canvas>().GetComponent<RectTransform>();
        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            canvasRect,
            Input.mousePosition,
            Camera.main,
            out Vector2 uiPos
        );
        return uiPos;
    }

    private void ResetButtonColor()
    {
        if (settingImg != null) settingImg.color = settingOriginalColor;
        if (exitImg != null) exitImg.color = exitOriginalColor;
    }

    private void OnApplicationQuit()
    {
        UnityEngine.Cursor.visible = true;
    }
}