
using System;
using System.Collections.Generic;
using UnityEngine;

public class LevelManager : MonoBehaviour
{
    public static LevelManager Instance { get; private set; }

    public LevelUIManager levelUIManager;
    
    public Transform WorldObjectsParent;

    private GameDifficulty Difficulty;

    public List<EnemyController> activeCombatEnemies = new List<EnemyController>();

    #region CameraSwitch

    private CameraSwitcher cameraSwitcher;
    private int switchRight = 40;

    #endregion

    public bool LevelInstantiated;
    
    public bool collectedAK47;
    
    public bool HasNexusCore { get; private set; }
    public bool DroneCamActive { get; private set; }
    
    public bool HasLevelStarted { get; private set; }

    private int zombieKillCount;

    public bool lightsTurnedOn;

    private bool isPaused;

    public bool readyButtonPressed;
    
    [SerializeField] private Camera mainCam, droneCam, minimapCam;
    private void Awake()
    {
        SetGameDifficulty(DataManager.Instance.difficulty);
        //SetGameDifficulty(GameDifficulty.EASY);
        if (Instance == null)
        {
            Instance = this;
        }
        
        AudioManager.Instance.OnSceneInitialized();
        
        cameraSwitcher = new CameraSwitcher(mainCam, droneCam, minimapCam);
    }

    private void Start()
    {
        HasNexusCore = false;
        EventManager.Instance.OnLevelInstantiated += OnLevelInstantiated;
        EventManager.Instance.OnLevelStarted += OnLevelStarted;
        
        EventManager.Instance.OnEnemyKilled += OnEnemyKilled;
        
        EventManager.Instance.OnDroneCamActivated += OnDroneCamActivated;
        EventManager.Instance.OnDroneCamDeactivated += OnDroneCamDeactivated;

        EventManager.Instance.OnLightsTurnedOn += OnLightsTurnedOn;

        EventManager.Instance.OnNexusCoreObtained += OnNexusCoreObtained;

        EventManager.Instance.OnGamePaused += OnGamePaused;
        EventManager.Instance.OnGameContinued += OnGameContinued;
        
        cameraSwitcher.DisableCameras();
        cameraSwitcher.ActivateDroneCam();
        //cameraSwitcher.ActivateMainCam();
    }

    private void OnDestroy()
    {
        EventManager.Instance.OnLevelInstantiated -= OnLevelInstantiated;
        EventManager.Instance.OnLevelStarted -= OnLevelStarted;
        
        EventManager.Instance.OnEnemyKilled -= OnEnemyKilled;
        
        EventManager.Instance.OnDroneCamActivated -= OnDroneCamActivated;
        EventManager.Instance.OnDroneCamDeactivated -= OnDroneCamDeactivated;
        
        EventManager.Instance.OnLightsTurnedOn -= OnLightsTurnedOn;
        
        EventManager.Instance.OnNexusCoreObtained -= OnNexusCoreObtained;
        
        EventManager.Instance.OnGamePaused -= OnGamePaused;
        EventManager.Instance.OnGameContinued -= OnGameContinued;
    }

    private void Update()
    {
        if (!LevelInstantiated) return;
        /*
        if (Input.GetKeyDown(KeyCode.Escape) && readyButtonPressed)
        {
            if (!isPaused) EventManager.Instance.InvokeOnGamePaused();
            else EventManager.Instance.InvokeOnGameContinued();
        }
        */
        
        CheckForCameraSwitch();
    }

    public void SetGameDifficulty(GameDifficulty difficulty)
    {
        Difficulty = difficulty;
    }

    public GameDifficulty GetGameDifficulty()
    {
        return Difficulty;
    }

    private void CheckForCameraSwitch()
    {
        if (Input.GetKeyDown(KeyCode.C) && !HasNexusCore && readyButtonPressed)
        {
            if (mainCam.gameObject.activeSelf)
            {
                if (switchRight > 0 && activeCombatEnemies.Count == 0)
                {
                    EventManager.Instance.InvokeOnDroneCamActivated();
                    cameraSwitcher.ActivateDroneCam();
                    switchRight--;
                }
            }
            else if (droneCam.gameObject.activeSelf)
            {
                if (!HasLevelStarted) EventManager.Instance.InvokeOnLevelStarted();
                EventManager.Instance.InvokeOnDroneCamDeactivated();
                
                cameraSwitcher.ActivateMainCam();
            }
        }
    }

    private void OnEnemyKilled()
    {
        zombieKillCount++;
    }



    private void OnGamePaused()
    {
        isPaused = true;
    }

    private void OnGameContinued()
    {
        isPaused = false;
    }

    private void OnDroneCamActivated()
    {
        
    }

    private void OnDroneCamDeactivated()
    {
        
    }

    private void OnLightsTurnedOn()
    {
        lightsTurnedOn = true;
        Color ambientColor = new Color(48.0f / 255.0f, 55.0f / 255.0f, 170.0f / 255.0f);
        RenderSettings.ambientLight = ambientColor;
    }

    private void OnNexusCoreObtained()
    {
        HasNexusCore = true;
        Color ambientColor = new Color(135.0f / 255.0f, 0.0f, 0.0f);
        RenderSettings.ambientLight = ambientColor;
    }

    private void OnLevelInstantiated()
    {
        LevelInstantiated = true;
        EventManager.Instance.InvokeOnDroneCamActivated();
    }

    private void OnLevelStarted()
    {
        HasLevelStarted = true;
    }
    
}
