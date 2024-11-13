using System;
using UnityEngine;

public class FirstAid : MonoBehaviour
{
    private bool isUsed, isPlayerIn;
    private PlayerHealth playerHealth;

    private void Start()
    {
        playerHealth = GameManager.Instance.Player.GetComponent<PlayerHealth>();
    }

    private void Update()
    {
        if (isPlayerIn && !isUsed)
        {
            if (Input.GetKeyDown(KeyCode.E) && playerHealth.Health < 100.0f)
            {
                EventManager.Instance.InvokeOnFirstAidUsed();
                isUsed = true;
                // Use it
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
            }

            isPlayerIn = false;
        }
        
    }
}