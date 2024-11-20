using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class MazeExit : MonoBehaviour
{
    private GameObject interactionText;
    private bool isPlayerIn;
    private bool canExit;

    private bool isPaused;

    private void Awake()
    {
        EventManager.Instance.OnDroneCamActivated += OnDroneCamActivated;
        EventManager.Instance.OnDroneCamDeactivated += OnDroneCamDeactivated;
        EventManager.Instance.OnGamePaused += OnGamePaused;
        EventManager.Instance.OnGameContinued += OnGameContinued;
    }

    void Start()
    {
        interactionText = GameManager.Instance.interactionText;
    }

    private void OnDestroy()
    {
        EventManager.Instance.OnDroneCamActivated -= OnDroneCamActivated;
        EventManager.Instance.OnDroneCamDeactivated -= OnDroneCamDeactivated;
        EventManager.Instance.OnGamePaused -= OnGamePaused;
        EventManager.Instance.OnGameContinued -= OnGameContinued;
    }

    void Update()
    {
        if (!LevelManager.Instance.HasLevelStarted) return;
        
        if (isPaused) return;
        
        if (isPlayerIn && canExit)
        {
            if (Input.GetKeyDown(KeyCode.E))
            {
                interactionText.GetComponent<TMP_Text>().text = "";
                interactionText.SetActive(false);
                EventManager.Instance.InvokeOnMazeExit();
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.transform.root.CompareTag("Player"))
        {
            if (!isPlayerIn)
            {
                isPlayerIn = true;
                interactionText.SetActive(true);
                if (LevelManager.Instance.HasNexusCore)
                {
                    interactionText.GetComponent<TMP_Text>().text = "Press \"E\" to exit the Nexus Lab.";
                    canExit = true;
                }
                else
                {
                    interactionText.GetComponent<TMP_Text>().text = "You cannot exit before retrieving the Nexus core!";
                    canExit = false;
                }
            }
            
        }
    }
    
    private void OnTriggerExit(Collider other)
    {
        if (other.transform.root.CompareTag("Player"))
        {
            if (isPlayerIn)
            {
                interactionText.GetComponent<TMP_Text>().text = "";
                interactionText.SetActive(false);
                isPlayerIn = false;
            }
        }
        
    }
    
    private void OnDroneCamActivated()
    {
        isPaused = true;
        LevelManager.Instance.levelUIManager.RegisterTrackedObject(transform);
    }

    private void OnDroneCamDeactivated()
    {
        isPaused = false;
        LevelManager.Instance.levelUIManager.UnregisterTrackedObject(transform);
    }

    private void OnGamePaused()
    {
        isPaused = true;
    }

    private void OnGameContinued()
    {
        isPaused = false;
    }
}
