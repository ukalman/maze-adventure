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
    private bool exited;

    private bool isPaused;

    [SerializeField] private Animator anim;
    
    private void Awake()
    {
        EventManager.Instance.OnDroneCamActivated += OnDroneCamActivated;
        EventManager.Instance.OnDroneCamDeactivated += OnDroneCamDeactivated;
        EventManager.Instance.OnGamePaused += OnGamePaused;
        EventManager.Instance.OnGameContinued += OnGameContinued;
    }

    void Start()
    {
        interactionText = LevelManager.Instance.levelUIManager.interactionText;
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
        
        if (isPlayerIn && canExit &&!exited)
        {
            if (Input.GetKeyDown(KeyCode.E))
            {
                interactionText.GetComponent<TMP_Text>().text = "";
                interactionText.SetActive(false);
                exited = true;
                anim.SetTrigger("DoorExit");
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
                    interactionText.GetComponent<TMP_Text>().text = "PRESS \"E\" TO EXIT THE NEXUS LAB.";
                    canExit = true;
                }
                else
                {
                    interactionText.GetComponent<TMP_Text>().text = "YOU CANNOT EXIT BEFORE RETRIEVING THE NEXUS CORE!";
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
