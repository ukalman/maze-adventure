using System;
using TMPro;
using UnityEngine;

public class FirstAid : MonoBehaviour
{
    private bool isUsed, isPlayerIn;
    private PlayerHealth playerHealth;

    private GameObject interactionText;

    private void Start()
    {
        interactionText = GameManager.Instance.interactionText;
        playerHealth = GameManager.Instance.Player.GetComponent<PlayerHealth>();
    }

    private void Update()
    {
        if (isPlayerIn && !isUsed)
        {
            if (Input.GetKeyDown(KeyCode.E) && playerHealth.CurrentHealth < 100.0f)
            {
                EventManager.Instance.InvokeOnFirstAidUsed();
                isUsed = true;
                interactionText.SetActive(false);
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
                Debug.Log("Player entered the first aid area.");
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
}