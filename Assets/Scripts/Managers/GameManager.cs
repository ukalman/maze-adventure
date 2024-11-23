using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;
using UnityEngine.Serialization;


public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    public GameState State;
    
    public Transform playerHead;
    public GameObject Player;
    public MovementStateManager PlayerMovement;

    public bool isGamePaused;
    
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        /*
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject); 
        */
    }

    void Start()
    {
        State = GameState.Initialization;
        
        EventManager.Instance.OnGamePaused += OnGamePaused;
        EventManager.Instance.OnGameContinued += OnGameContinued;
        EventManager.Instance.OnNavMeshBaked += OnNavMeshBaked;
        
        PlayerMovement = Player.GetComponent<MovementStateManager>();
    }

    private void OnDestroy()
    {
        EventManager.Instance.OnGamePaused -= OnGamePaused;
        EventManager.Instance.OnGameContinued -= OnGameContinued;
        EventManager.Instance.OnNavMeshBaked -= OnNavMeshBaked;
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

    private void OnGamePaused()
    {
        Time.timeScale = 0f;
        State = GameState.Paused;
    }

    private void OnGameContinued()
    {
        Time.timeScale = 1f;
        State = GameState.Gameplay;
    }

    private void OnNavMeshBaked()
    {
        State = GameState.Gameplay;
    }
    
}
