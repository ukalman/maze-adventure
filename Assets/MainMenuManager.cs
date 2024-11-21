using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainMenuManager : MonoBehaviour
{

    [SerializeField] private Camera guiCam;
    
    [SerializeField] private GameObject mainMenuContainer;
    [SerializeField] private GameObject background;
    [SerializeField] private GameObject sceneModels;
    [SerializeField] private GameObject loadingScreen;
    
    [SerializeField] private AudioSource musicAudioSource;
    [SerializeField] private AudioSource sfxAudioSource;

    [SerializeField] private AudioClip mainMenuAmbience_1;
    [SerializeField] private AudioClip mainMenuAmbience_2;

    [SerializeField] private AudioClip buttonClickSound;
    [SerializeField] private AudioClip readyButtonSound;

    [SerializeField] private AudioClip futuristicWhooshSound;
    
    void Start()
    {
        musicAudioSource.clip = mainMenuAmbience_1;
        musicAudioSource.Play();
    }

    public void OnLaunchClicked()
    {
        sfxAudioSource.PlayOneShot(readyButtonSound);
        mainMenuContainer.SetActive(false);
        StartCoroutine(LoadLevelCoroutine());
    }

    private IEnumerator LoadLevelCoroutine()
    {
        yield return StartCoroutine(CameraZoomCoroutine());
        StartCoroutine(SceneManager.Instance.LoadLevelAsync(1));

    }

    private IEnumerator CameraZoomCoroutine()
    {
        yield return new WaitForSeconds(1.0f);
        sfxAudioSource.PlayOneShot(futuristicWhooshSound);

        yield return new WaitForSeconds(0.3f);
        float timer = 0.0f;
        float whooshInterval = 0.3f;

        while (timer <= whooshInterval)
        {
            timer += Time.deltaTime;
            if (guiCam.fieldOfView >= 0.5f) guiCam.fieldOfView -= 0.4f;
            yield return null;
        }
        
        background.SetActive(false);
        Destroy(sceneModels);
        loadingScreen.SetActive(true);
    }
    
    public void PlayButtonSound()
    {
        sfxAudioSource.PlayOneShot(buttonClickSound);
    }
}
