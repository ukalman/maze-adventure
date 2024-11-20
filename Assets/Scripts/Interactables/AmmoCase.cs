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
        interactionText = GameManager.Instance.interactionText;
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
                if (ammoType == AmmoType.AK47_762 && !LevelManager.Instance.collectedAK47)
                {
                    interactionText.GetComponent<TMP_Text>().text = "To collect this ammo, you must first acquire an AK-47 rifle.";
                    canCollect = false;
                }
                
                else if (ammoType == AmmoType.AK47_762 && LevelManager.Instance.collectedAK47)
                {
                    interactionText.GetComponent<TMP_Text>().text = "Press \"E\" to collect AK-47 ammo.";
                    canCollect = true;
                }
                
                else if (ammoType == AmmoType.M4_556)
                {
                    interactionText.GetComponent<TMP_Text>().text = "Press \"E\" to collect M4 Carbine ammo.";
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
        Debug.Log("An ammo case is activated!");
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
