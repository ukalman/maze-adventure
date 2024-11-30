using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using Random = UnityEngine.Random;

public class AmmoCase : MonoBehaviour
{
    private GameObject interactionText;

    [SerializeField] private AmmoType ammoType;
    private int ammoAmount = 30;

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
        interactionText = LevelManager.Instance.levelUIManager.interactionText;
        ammoAmount = Random.Range(25, 50);
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
        if (isPaused) return;
        
        if (isPlayerIn && canCollect)
        {
            if (Input.GetKeyDown(KeyCode.E))
            {
                EventManager.Instance.InvokeOnAmmoCollected(ammoType, ammoAmount);
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
                if (ammoType == AmmoType.AK47_762 && !LevelManager.Instance.CollectedAK47)
                {
                    interactionText.GetComponent<TMP_Text>().text = "TO COLLECT THIS AMMO, YOU MUST FIRST ACQUIRE AN AK-47 RIFLE.";
                    canCollect = false;
                }
                
                else if (ammoType == AmmoType.AK47_762 && LevelManager.Instance.CollectedAK47)
                {
                    interactionText.GetComponent<TMP_Text>().text = "PRESS \"E\" TO COLLECT AK-47 AMMO.";
                    canCollect = true;
                }
                
                else if (ammoType == AmmoType.M4_556)
                {
                    interactionText.GetComponent<TMP_Text>().text = "PRESS \"E\" TO COLLECT M4 CARBINE AMMO.";
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
