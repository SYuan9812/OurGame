using UnityEngine;
using UnityEngine.SceneManagement;

public class TitleButtonManager : MonoBehaviour
{
    [Header("New Game Coordinates")]
    [SerializeField] private float newGameMinX = -7.9f;
    [SerializeField] private float newGameMaxX = -3.6f;
    [SerializeField] private float newGameMinY = 1.53f;
    [SerializeField] private float newGameMaxY = 2.7f;

    [Header("Load Game Coordinates")]
    [SerializeField] private float loadGameMinX = -7.103f;
    [SerializeField] private float loadGameMaxX = -2.636f;
    [SerializeField] private float loadGameMinY = 0.094f;
    [SerializeField] private float loadGameMaxY = 1.046f;

    [Header("Settings Coordinates")]
    [SerializeField] private float settingsMinX = -7.427f;
    [SerializeField] private float settingsMaxX = -2.96f;
    [SerializeField] private float settingsMinY = -1.398f;
    [SerializeField] private float settingsMaxY = -0.377f;

    [Header("Exit Coordinates")]
    [SerializeField] private float exitMinX = -7.3f;
    [SerializeField] private float exitMaxX = -2.6f;
    [SerializeField] private float exitMinY = -2.77f;
    [SerializeField] private float exitMaxY = -1.78f;


    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Vector2 worldPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);

            if (IsPointInNewGameArea(worldPos))
            {
                OnNewGameClicked();
            }
            else if (IsPointInLoadGameArea(worldPos))
            {
                OnLoadGameClicked();
            }
            else if (IsPointInSettingsArea(worldPos))
            {
                OnSettingsClicked();
            }
            else if (IsPointInExitArea(worldPos))
            {
                OnExitClicked();
            }
        }
    }


    private bool IsPointInNewGameArea(Vector2 point)
    {
        return point.x >= newGameMinX && point.x <= newGameMaxX
            && point.y >= newGameMinY && point.y <= newGameMaxY;
    }

    private bool IsPointInLoadGameArea(Vector2 point)
    {
        return point.x >= loadGameMinX && point.x <= loadGameMaxX
            && point.y >= loadGameMinY && point.y <= loadGameMaxY;
    }

    private bool IsPointInSettingsArea(Vector2 point)
    {
        return point.x >= settingsMinX && point.x <= settingsMaxX
            && point.y >= settingsMinY && point.y <= settingsMaxY;
    }

    private bool IsPointInExitArea(Vector2 point)
    {
        return point.x >= exitMinX && point.x <= exitMaxX
            && point.y >= exitMinY && point.y <= exitMaxY;
    }


    private void OnNewGameClicked()
    {

        SceneManager.LoadScene("Scene 1");
    }

    private void OnLoadGameClicked()
    {
        Debug.Log("Load Game Operation Incomplete");
    }

    private void OnSettingsClicked()
    {
        Debug.Log("Settings Incomplete");
    }

    private void OnExitClicked()
    {
        Debug.Log("Application Quit, only works with packaged game");
        Application.Quit();
    }
}