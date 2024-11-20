using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;
using UnityEngine.Serialization;


public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }
    
    public Transform playerHead;
    public GameObject Player;
    public MovementStateManager PlayerMovement;

    public GameObject interactionText;

    public bool isGamePaused;
    
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
        
        //Camera.main.gameObject.AddComponent<CinemachineBrain>();
    }

    void Start()
    {
        
        PlayerMovement = Player.GetComponent<MovementStateManager>();

    }

    
    void Update()
    {
        
    }
    
    public void SetLayerRecursively(GameObject obj, int newLayer)
    {
        obj.layer = newLayer;

        foreach (Transform child in obj.transform)
        {
            SetLayerRecursively(child.gameObject, newLayer);
        }
    }

}
