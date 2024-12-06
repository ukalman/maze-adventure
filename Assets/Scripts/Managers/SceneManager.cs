using System;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public class SceneManager : MonoBehaviour
{
    public static SceneManager Instance { get; private set; }

    public Slider loadingSlider;
    
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

    
    public IEnumerator LoadLevelAsync(int sceneIndex)
    {
        AsyncOperation loadOperation = UnityEngine.SceneManagement.SceneManager.LoadSceneAsync(sceneIndex);
        
        loadOperation.allowSceneActivation = false;
        
        float fakeProgress = 0f;
        while (fakeProgress < 0.9f)
        {
            fakeProgress += Random.Range(0.01f, 0.05f);
            fakeProgress = Mathf.Clamp(fakeProgress, 0f, 0.9f);
            
            loadingSlider.value = fakeProgress;

            yield return new WaitForSeconds(Random.Range(0.05f, 0.1f));
        }
        
        while (!loadOperation.isDone)
        {
            // If the real loading progress is at least 0.9, wait for scene activation
            if (loadOperation.progress >= 0.9f)
            {
                loadingSlider.value = 1f;
                loadOperation.allowSceneActivation = true; // Activate the scene
            }

            yield return null;
        }
    }



    public void ReloadCurrentScene()
    {
        int currentSceneIndex = UnityEngine.SceneManagement.SceneManager.GetActiveScene().buildIndex;
        UnityEngine.SceneManagement.SceneManager.LoadScene(currentSceneIndex);
    }

   
    public void QuitGame()
    {
        Application.Quit();
    }

    
    
}