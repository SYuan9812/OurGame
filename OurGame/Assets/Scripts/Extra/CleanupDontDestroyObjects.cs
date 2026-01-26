using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class CleanupDontDestroyObjects : MonoBehaviour
{
    public List<string> objectsToDestroyByName = new List<string>()
    {
        "CursorCanva",
        "Manager",
        "PauseCanva",
        "PauseButtons"
    };
    public List<string> tagsToDestroy = new List<string>();

    private void Awake()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        CleanupObjects();
    }

    private void CleanupObjects()
    {
        List<GameObject> objectsToDestroy = new List<GameObject>();

        GameObject[] allObjects = FindObjectsOfType<GameObject>(true);
        foreach (string objName in objectsToDestroyByName)
        {
            foreach (GameObject obj in allObjects)
            {
                if (obj != null && obj.name == objName)
                {
                    objectsToDestroy.Add(obj);
                    break;
                }
            }
        }

        foreach (GameObject obj in objectsToDestroy)
        {
            if (obj != null)
            {
                DestroyImmediate(obj);
            }
        }

        List<GameObject> tagObjectsToDestroy = new List<GameObject>();
        foreach (string tag in tagsToDestroy)
        {
            GameObject[] objs = GameObject.FindGameObjectsWithTag(tag);
            if (objs.Length > 0)
            {
                foreach (GameObject obj in objs)
                {
                    if (obj != null)
                    {
                        tagObjectsToDestroy.Add(obj);
                    }
                }
            }
        }

        foreach (GameObject obj in tagObjectsToDestroy)
        {
            if (obj != null)
            {
                DestroyImmediate(obj);
            }
        }
    }

    private void OnDestroy()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }
}