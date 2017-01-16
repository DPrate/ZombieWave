using UnityEngine;
using UnityEngine.SceneManagement;
using Diggames.Utilities;
using System.Collections;

public class LoadingScreen : MonoBehaviour
{
    public static LoadingScreen Instance;
    public GameObject LoadingScreenDisplay;

    void Awake()
    {
        if(Instance == null)
        {
            Instance = this;
        }
        else if(Instance != null)
        {
            Destroy(gameObject);
        }

        SceneManager.activeSceneChanged += OnSceneChanged;
    }

    private void OnSceneChanged(Scene previousScene, Scene newScene)
    {
        if(this != Instance)
            return;

        DebugLogger.LogMessage("Scene changed to " +newScene.name);
        ShowLoadingScreen(false);
    }

    public void ShowLoadingScreen(bool show)
    {
        if(show)
            LoadingScreenDisplay.SetActive(true);
        else
            LoadingScreenDisplay.SetActive(false);
    }
}
