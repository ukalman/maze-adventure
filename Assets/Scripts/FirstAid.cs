using System;
using TMPro;
using UnityEngine;

public class FirstAid : MonoBehaviour
{
    private bool isUsed, isPlayerIn;
    private PlayerHealth playerHealth;

    private GameObject firstAidText;

    private void Start()
    {
        firstAidText = GameManager.Instance.firstAidText;
        if (firstAidText == null) Debug.Log("Yes, null.");
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
                firstAidText.SetActive(false);
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
                if (playerHealth.CurrentHealth < 100.0f) firstAidText.SetActive(true);
            }
        }
        
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.transform.root.CompareTag("Player"))
        {
            if (!isUsed && isPlayerIn)
            {
                // Let the Player's UI Manager know that he exited the collision area
                Debug.Log("Player exited the first aid area.");
                // Add your logic here, e.g., stop healing, hide the UI prompt, etc.
                firstAidText.SetActive(false);
            }

            isPlayerIn = false;
        }
        
    }
}