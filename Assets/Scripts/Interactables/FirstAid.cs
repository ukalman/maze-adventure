using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class FirstAid : MonoBehaviour
{
    private bool isUsed, isPlayerIn;
    private PlayerHealth playerHealth;

    private GameObject interactionText;

    private bool isPaused;
    
    [SerializeField] private GameObject minimapTile;
    [SerializeField] private Material usedMat;

    private void Awake()
    {
        EventManager.Instance.OnDroneCamActivated += OnDroneCamActivated;
        EventManager.Instance.OnDroneCamDeactivated += OnDroneCamDeactivated;
        EventManager.Instance.OnGamePaused += OnGamePaused;
        EventManager.Instance.OnGameContinued += OnGameContinued;
    }

    private void Start()
    {
        interactionText = GameManager.Instance.interactionText;
        playerHealth = GameManager.Instance.Player.GetComponent<PlayerHealth>();
    }

    private void OnDestroy()
    {
        EventManager.Instance.OnDroneCamActivated -= OnDroneCamActivated;
        EventManager.Instance.OnDroneCamDeactivated -= OnDroneCamDeactivated;
        EventManager.Instance.OnGamePaused -= OnGamePaused;
        EventManager.Instance.OnGameContinued -= OnGameContinued;
    }

    private void Update()
    {
        
        if (!LevelManager.Instance.HasLevelStarted) return;
        
        if (isPlayerIn && !isUsed)
        {
            if (Input.GetKeyDown(KeyCode.E) && playerHealth.CurrentHealth < 100.0f)
            {
                EventManager.Instance.InvokeOnFirstAidUsed();
                isUsed = true;
                interactionText.SetActive(false);
                minimapTile.GetComponent<Renderer>().material = usedMat;
                //minimapTile.SetActive(false);
            }
        }
    }

    private void OnTriggerEnter(Collider other) 
    {
        if (other.transform.root.CompareTag("Player"))
        {
            if (!isUsed && !isPlayerIn)
            {
                isPlayerIn = true;
                // Add your logic here, e.g., start healing the player, display a UI prompt, etc.
                if (playerHealth.CurrentHealth < 100.0f)
                {
                    interactionText.SetActive(true);
                    interactionText.GetComponent<TMP_Text>().text = "Press \"E\" to use the first aid kit.";
                }
            }
        }
        
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.transform.root.CompareTag("Player"))
        {
            if (!isUsed && isPlayerIn)
            {
                // Add logic here, e.g., stop healing, hide the UI prompt, etc.
                interactionText.GetComponent<TMP_Text>().text = "";
                interactionText.SetActive(false);
            }

            isPlayerIn = false;
        }
        
    }
    
    private void OnDroneCamActivated()
    {
        isPaused = true;
        if (!isUsed) LevelManager.Instance.levelUIManager.RegisterTrackedObject(transform);
    }

    private void OnDroneCamDeactivated()
    {
        isPaused = false;
        if (!isUsed) LevelManager.Instance.levelUIManager.UnregisterTrackedObject(transform);
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