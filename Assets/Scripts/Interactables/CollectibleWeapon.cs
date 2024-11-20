using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;

public class CollectibleWeapon : MonoBehaviour
{
    private GameObject interactionText;
    
    [SerializeField] private WeaponName weaponName;

    private bool isPlayerIn;
    private bool canCollect;

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
        
        if (isPlayerIn && canCollect)
        {
            if (Input.GetKeyDown(KeyCode.E))
            {
                EventManager.Instance.InvokeOnWeaponAcquired(weaponName);
                interactionText.GetComponent<TMP_Text>().text = "";
                interactionText.SetActive(false);
                Destroy(gameObject);
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
                if (weaponName == WeaponName.AK47 && !LevelManager.Instance.collectedAK47)
                {
                    interactionText.GetComponent<TMP_Text>().text = "Press \"E\" to acquire the AK-47 rifle.";
                    canCollect = true;
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
