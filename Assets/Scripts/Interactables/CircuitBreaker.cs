using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class CircuitBreaker : MonoBehaviour
{
    private bool isPlayerIn;

    private GameObject interactionText;

    private bool isPaused;

    [SerializeField] private GameObject minimapTile;

    private void Awake()
    {
        EventManager.Instance.OnDroneCamActivated += OnDroneCamActivated;
        EventManager.Instance.OnDroneCamDeactivated += OnDroneCamDeactivated;
        EventManager.Instance.OnGamePaused += OnGamePaused;
        EventManager.Instance.OnGameContinued += OnGameContinued;
    }

    // Start is called before the first frame update
    void Start()
    {
        interactionText = LevelManager.Instance.levelUIManager.interactionText;
        EventManager.Instance.OnLightsTurnedOn += OnLightsTurnedOn;
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
        
        if (isPlayerIn && Input.GetKeyDown(KeyCode.E) && !LevelManager.Instance.VeinsActivated)
        {
            EventManager.Instance.InvokeOnLightsTurnedOn();
            interactionText.SetActive(false);
            minimapTile.SetActive(false);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.transform.root.CompareTag("Player"))
        {
            if (!LevelManager.Instance.VeinsActivated && !isPlayerIn)
            {
                isPlayerIn = true;
                interactionText.SetActive(true);
                interactionText.GetComponent<TMP_Text>().text = "PRES \"E\" TO USE ACTIVATE THE VEINS OF NEXUS.";
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.transform.root.CompareTag("Player"))
        {
            if (!LevelManager.Instance.VeinsActivated && isPlayerIn)
            {
                interactionText.GetComponent<TMP_Text>().text = "";
                interactionText.SetActive(false);
            }
            
            isPlayerIn = false;
        }
    }

    private void OnDroneCamActivated()
    {
        isPaused = true;
        if (!LevelManager.Instance.VeinsActivated) LevelManager.Instance.levelUIManager.RegisterTrackedObject(transform);
    }

    private void OnDroneCamDeactivated()
    {
        isPaused = false;
        if (!LevelManager.Instance.VeinsActivated) LevelManager.Instance.levelUIManager.UnregisterTrackedObject(transform);
    }

    private void OnGamePaused()
    {
        isPaused = true;
    }

    private void OnGameContinued()
    {
        isPaused = false;
    }

    private void OnLightsTurnedOn()
    {
        minimapTile.SetActive(false);
    }
}
