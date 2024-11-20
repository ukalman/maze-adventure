using System;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneManager : MonoBehaviour
{
    public static SceneManager Instance { get; private set; }

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        
        
        Instance = this;
        DontDestroyOnLoad(gameObject); 
    }

    private void Start()
    {
        EventManager.Instance.OnMazeExit += OnMazeExit;
    }

    private void OnDestroy()
    {
        EventManager.Instance.OnMazeExit -= OnMazeExit;
    }

    /// <summary>
    /// Load a scene by its name.
    /// </summary>
    /// <param name="sceneName">The name of the scene to load.</param>
    public void LoadScene(string sceneName)
    {
        if (!string.IsNullOrEmpty(sceneName))
        {
            UnityEngine.SceneManagement.SceneManager.LoadScene(sceneName);
        }
        else
        {
            Debug.LogWarning("Scene name is null or empty!");
        }
    }

    /// <summary>
    /// Load a scene by its build index.
    /// </summary>
    /// <param name="sceneIndex">The build index of the scene to load.</param>
    public void LoadScene(int sceneIndex)
    {
        if (sceneIndex >= 0 && sceneIndex < UnityEngine.SceneManagement.SceneManager.sceneCountInBuildSettings)
        {
            UnityEngine.SceneManagement.SceneManager.LoadScene(sceneIndex);
        }
        else
        {
            Debug.LogWarning("Invalid scene index!");
        }
    }

    /// <summary>
    /// Reload the current scene.
    /// </summary>
    public void ReloadCurrentScene()
    {
        int currentSceneIndex = UnityEngine.SceneManagement.SceneManager.GetActiveScene().buildIndex;
        UnityEngine.SceneManagement.SceneManager.LoadScene(currentSceneIndex);
    }

    /// <summary>
    /// Quit the application.
    /// </summary>
    public void QuitGame()
    {
        Debug.Log("Quitting the game...");
        Application.Quit();
    }

    private void OnMazeExit()
    {
        LoadScene(0);
    }
    
}