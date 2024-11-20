using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainMenuManager : MonoBehaviour
{

    [SerializeField] private Camera guiCam;
    [SerializeField] private GameObject gameModeUI;
    
    [SerializeField] private AudioSource musicAudioSource;
    [SerializeField] private AudioSource sfxAudioSource;

    [SerializeField] private AudioClip mainMenuAmbience_1;
    [SerializeField] private AudioClip mainMenuAmbience_2;

    [SerializeField] private AudioClip buttonClickSound;
    [SerializeField] private AudioClip readyButtonSound;

    [SerializeField] private AudioClip futuristicWhooshSound;
    
    void Start()
    {
        EventManager.Instance.OnMainMenuReadyClicked += OnReadyClicked;
        musicAudioSource.clip = mainMenuAmbience_1;
        musicAudioSource.Play();
    }

    private void OnDestroy()
    {
        EventManager.Instance.OnMainMenuReadyClicked -= OnReadyClicked;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    
    private void OnReadyClicked()
    {
        sfxAudioSource.PlayOneShot(readyButtonSound);
        gameModeUI.SetActive(false);
        StartCoroutine(CameraZoomCoroutine());
    }

    private IEnumerator CameraZoomCoroutine()
    {
        yield return new WaitForSeconds(1.0f);
        sfxAudioSource.PlayOneShot(futuristicWhooshSound);

        yield return new WaitForSeconds(0.5f);
        float timer = 0.0f;
        float whooshInterval = 2.0f;

        while (timer <= whooshInterval)
        {
            timer += Time.deltaTime;
            guiCam.fieldOfView -= 0.3f;
            yield return null;
        }
        
    }
    
    public void PlayButtonSound()
    {
        sfxAudioSource.PlayOneShot(buttonClickSound);
    }
}
