using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
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
    public MazeCell[,] MazeGrid;

    public float totalTime;
    private bool isTimerRunning;

    public GameObject bulletImpactEffect;
    
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
        bulletImpactEffect.GetComponentsInChildren<ParticleSystem>().ToList().ForEach(ps => ps.Play());
        bulletImpactEffect.GetComponentsInChildren<ParticleSystem>().ToList().ForEach(ps => ps.Stop());
        
        State = GameState.Initialization;
        
        EventManager.Instance.OnGamePaused += OnGamePaused;
        EventManager.Instance.OnGameContinued += OnGameContinued;
        EventManager.Instance.OnNavMeshBaked += OnNavMeshBaked;
        EventManager.Instance.OnPlayerDied += OnPlayerDied;
        EventManager.Instance.OnCountdownEnded += OnCountdownEnded;
        EventManager.Instance.OnLevelCompleted += OnLevelCompleted;
        
        PlayerMovement = Player.GetComponent<MovementStateManager>();
    }

    private void OnDestroy()
    {
        EventManager.Instance.OnGamePaused -= OnGamePaused;
        EventManager.Instance.OnGameContinued -= OnGameContinued;
        EventManager.Instance.OnNavMeshBaked -= OnNavMeshBaked;
        EventManager.Instance.OnPlayerDied -= OnPlayerDied;
        EventManager.Instance.OnCountdownEnded -= OnCountdownEnded;
        EventManager.Instance.OnLevelCompleted -= OnLevelCompleted;
    }
    
    void Update()
    {
        
    }
    
    private IEnumerator TimerCountdown()
    {
        while (isTimerRunning)
        {
            while (State != GameState.Gameplay)
            {
                yield return null;
            }
            
            yield return new WaitForSecondsRealtime(1.0f);
            totalTime += 1.0f;
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

    private void OnPlayerDied()
    {
        State = GameState.LevelFailed;
        isTimerRunning = false;
    }

    private void OnCountdownEnded()
    {
        State = GameState.LevelFailed;
        isTimerRunning = false;
    }

    private void OnLevelCompleted(GameDifficulty difficulty) 
    {
        State = GameState.LevelCompleted;
        isTimerRunning = false;
    }

    public void OnStartClicked()
    {
        isTimerRunning = true;
        StartCoroutine(TimerCountdown());
    }
    
}
