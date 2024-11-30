
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class LevelManager : MonoBehaviour
{
    public static LevelManager Instance { get; private set; }

    public LevelUIManager levelUIManager;
    
    public Transform WorldObjectsParent;

    private GameDifficulty Difficulty;

    public List<EnemyController> activeCombatEnemies = new List<EnemyController>();

    [Header("Post Processing")] 
    [SerializeField] private GameObject nightVisionVolume;
    [SerializeField] private GameObject veinsVolume;
    [SerializeField] private GameObject nightVisionLight;
    
    #region CameraSwitch

    private CameraSwitcher cameraSwitcher;
    private int switchRight = 40;

    #endregion

    public bool LevelInstantiated;
    
    public bool CollectedAK47;
    
    public bool HasNexusCore { get; private set; }
    public bool DroneCamActive { get; private set; }
    
    public bool HasLevelStarted { get; private set; }

    public bool VeinsActivated;

    public int zombieKillCount;

    private bool isPaused;

    public bool readyButtonPressed;

    public bool playerDied;

    public bool LevelWon;

    public bool isDroneCamAvailable;

    [SerializeField] private AudioSource nightVisionAudioSource;
    
    [Header(("Cameras"))]
    [SerializeField] private Camera mainCam, droneCam, minimapCam;
    private void Awake()
    {
        if (DataManager.Instance != null) SetGameDifficulty(DataManager.Instance.selectedDifficulty);
        else SetGameDifficulty(GameDifficulty.EASY);
        //SetGameDifficulty(GameDifficulty.EASY);
        
        if (Instance == null)
        {
            Instance = this;
        }
        
        //cameraSwitcher = new CameraSwitcher(mainCam, droneCam, minimapCam);
        
    }

    private void Start()
    {
        cameraSwitcher = new CameraSwitcher(mainCam, droneCam, minimapCam);
        HasNexusCore = false;
        nightVisionVolume.SetActive(false);
        veinsVolume.SetActive(false);
        nightVisionLight.SetActive(false);
        OnVolumeChanged();
        
        EventManager.Instance.OnLevelInstantiated += OnLevelInstantiated;
        EventManager.Instance.OnLevelStarted += OnLevelStarted;
        
        EventManager.Instance.OnEnemyKilled += OnEnemyKilled;
        
        EventManager.Instance.OnDroneCamActivated += OnDroneCamActivated;
        EventManager.Instance.OnDroneCamDeactivated += OnDroneCamDeactivated;

        EventManager.Instance.OnLightsTurnedOn += OnLightsTurnedOn;

        EventManager.Instance.OnNexusCoreObtained += OnNexusCoreObtained;

        EventManager.Instance.OnGamePaused += OnGamePaused;
        EventManager.Instance.OnGameContinued += OnGameContinued;

        EventManager.Instance.OnPlayerDied += OnPlayerDied;

        EventManager.Instance.OnCountdownEnded += OnCountdownEnded;
        
        EventManager.Instance.OnMazeExit += OnMazeExit;

        EventManager.Instance.OnVolumeChanged += OnVolumeChanged;
        
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
        
        EventManager.Instance.OnPlayerDied -= OnPlayerDied;
        
        EventManager.Instance.OnCountdownEnded -= OnCountdownEnded;

        EventManager.Instance.OnMazeExit -= OnMazeExit;
        
        EventManager.Instance.OnVolumeChanged -= OnVolumeChanged;
    }

    private void Update()
    {
        if (!LevelInstantiated) return;
        
        if (playerDied) return;
        
        HandlePauseState();
        
        if (isPaused) return;
        HandleCameraSwitch();
        HandleNightVision();
    }

    private void SetGameDifficulty(GameDifficulty difficulty)
    {
        Difficulty = difficulty;
    }

    public GameDifficulty GetGameDifficulty()
    {
        return Difficulty;
    }

    private void HandlePauseState()
    {
        if (Input.GetKeyDown(KeyCode.Escape) && !DroneCamActive)
        {
            if (isPaused)
            {
                Time.timeScale = 1f;
                EventManager.Instance.InvokeOnGameContinued();
            }
            else
            {
                Time.timeScale = 0f; 
                EventManager.Instance.InvokeOnGamePaused();
            }
        }
    }
    
    private void HandleCameraSwitch()
    {
        if (Input.GetKeyDown(KeyCode.C) && !HasNexusCore && readyButtonPressed)
        {
            if (mainCam.gameObject.activeSelf)
            {
                if (isDroneCamAvailable && activeCombatEnemies.Count == 0)
                {
                    EventManager.Instance.InvokeOnDroneCamActivated();
                    cameraSwitcher.ActivateDroneCam();
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

    private IEnumerator DroneCamRefillCoroutine()
    {
        float duration = 60.0f;

        while (duration > 0.0f && !isDroneCamAvailable)
        {
            if (isPaused) yield return null;
            else
            {
                yield return new WaitForSeconds(1.0f);
                duration -= 1.0f;
            }
        }

        if (duration <= 0.0f)
        {
            isDroneCamAvailable = true;
            levelUIManager.UpdateDroneCamAvailabilityText(isDroneCamAvailable);
        }
    }

    private IEnumerator DroneCamCountdownCoroutine(float duration)
    {
        levelUIManager.UpdateDroneCamCountdown(duration);
        while (duration > 0.0f && DroneCamActive)
        {
            if (isPaused) yield return null;
            else
            {
                yield return new WaitForSeconds(1.0f);
                duration -= 1.0f;
                levelUIManager.UpdateDroneCamCountdown(duration);
            }
        }

        if (duration <= 0.0f)
        {
            if (!HasLevelStarted) EventManager.Instance.InvokeOnLevelStarted();
            EventManager.Instance.InvokeOnDroneCamDeactivated();
            cameraSwitcher.ActivateMainCam();
        }
    }
    
    private void HandleNightVision()
    {
        if (Input.GetKeyDown(KeyCode.X))
        {
            if (DroneCamActive || VeinsActivated) return;
            nightVisionVolume.SetActive(!nightVisionVolume.activeSelf);
            nightVisionLight.SetActive(!nightVisionLight.activeSelf);
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
        DroneCamActive = true;
        switch (Difficulty)
        {
            case GameDifficulty.MODERATE:
                StartCoroutine(DroneCamCountdownCoroutine(30.0f));
                break;
            case GameDifficulty.HARD:
                StartCoroutine(DroneCamCountdownCoroutine(30.0f));
                break;
        }
    }

    private void OnDroneCamDeactivated()
    {
        DroneCamActive = false;
        switch (Difficulty)
        {
            case GameDifficulty.EASY:
                isDroneCamAvailable = true;
                break;
            case GameDifficulty.MODERATE:
                isDroneCamAvailable = false;
                StartCoroutine(DroneCamRefillCoroutine());
                break;
            case GameDifficulty.HARD:
                isDroneCamAvailable = false;
                break;
        }
        levelUIManager.UpdateDroneCamAvailabilityText(isDroneCamAvailable);
    }

    private void OnLightsTurnedOn()
    {
        VeinsActivated = true;
        Color ambientColor = new Color(48.0f / 255.0f, 55.0f / 255.0f, 170.0f / 255.0f);
        RenderSettings.ambientLight = ambientColor;
        nightVisionVolume.SetActive(false);
        nightVisionLight.SetActive(false);
        veinsVolume.SetActive(true);

        isDroneCamAvailable = true;
        levelUIManager.UpdateDroneCamAvailabilityText(isDroneCamAvailable);
    }

    private void OnNexusCoreObtained()
    {
        HasNexusCore = true;
        Color ambientColor = new Color(135.0f / 255.0f, 0.0f, 0.0f);
        RenderSettings.ambientLight = ambientColor;
        isDroneCamAvailable = false;
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
    
    private void OnPlayerDied()
    {
        playerDied = true;
    }

    private void OnCountdownEnded()
    {
        playerDied = true;
    }

    private void OnMazeExit()
    {
        LevelWon = true;
        EventManager.Instance.InvokeOnLevelCompleted(Difficulty);
    }

    private void OnVolumeChanged()
    {
        nightVisionAudioSource.volume = AudioManager.Instance.masterVolume * AudioManager.Instance.sfxVolume;
    }
}
