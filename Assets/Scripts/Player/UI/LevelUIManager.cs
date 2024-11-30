
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class LevelUIManager : MonoBehaviour
{
    [Header("Canvas")]
    [SerializeField] private Canvas mainCamCanvas;
    [SerializeField] private Canvas droneCamObjects;
    [SerializeField] private Canvas droneCamCanvas;
    [SerializeField] private Canvas generalCanvas;
    [SerializeField] private RectTransform canvasRectTransform;

    [Header("Text")] 
    public GameObject interactionText;
    [SerializeField] private TMP_Text objectiveInfoText;
    [SerializeField] private TMP_Text scanningText;
    [SerializeField] private TMP_Text youDiedText;
    [SerializeField] private TMP_Text droneCamAvailabilityText;
    [SerializeField] private TMP_Text sprayPaintUsesText;
    [SerializeField] private GameObject droneCamCountdownGO;
    [SerializeField] private GameObject levelInfo;
    [SerializeField] private GameObject easyLevelInfo;
    [SerializeField] private GameObject moderateLevelInfo;
    [SerializeField] private GameObject hardLevelInfo;
    
    
    [Header("Prefabs")]
    [SerializeField] private GameObject playerIconPrefab;
    [SerializeField] private GameObject zombieIconPrefab;
    [SerializeField] private GameObject ak47IconPrefab;
    [SerializeField] private GameObject ammoCaseIconPrefab;
    [SerializeField] private GameObject firstAidIconPrefab;
    [SerializeField] private GameObject circuitBreakerIconPrefab;
    [SerializeField] private GameObject nexusCoreIconPrefab;
    [SerializeField] private GameObject entranceIconPrefab;
    [SerializeField] private GameObject exitIconPrefab;

    [Header("Buttons")] 
    [SerializeField] private GameObject readyButton;
    [SerializeField] private GameObject startButton;
    
    
    [Header("Drone Cam")]
    [SerializeField] Camera droneCam;
    
    [Header("General Panels")]
    [SerializeField] private GameObject gamePausedPanel;
    [SerializeField] private GameObject levelStartedPanel;
    [SerializeField] private GameObject playerDiedPanel;
    [SerializeField] private GameObject levelCompletedPanel;

    [Header("Sliders")]
    [SerializeField] private Slider masterVolumeSlider;
    [SerializeField] private Slider musicVolumeSlider;
    [SerializeField] private Slider sfxVolumeSlider;

    [Header("Images")]
    [SerializeField] private Image crosshair;
    [SerializeField] private Image fadeOutImage;
    private float fadeDuration = 2.0f;
    
    private bool isDroneCamActive;
    private bool isScanning;
    private DroneCameraController droneCamController;
    
    private Dictionary<Transform, RectTransform> trackedIcons = new Dictionary<Transform, RectTransform>();
    
    private float interval = 0.5f; // For the scanning text

    [SerializeField] private TMP_Text timerText;
    [SerializeField] private float startTimeInMinutes;

    private float timeRemaining;
    private bool isTimerRunning = true;

    private bool isPaused;

    private void Awake()
    {
        EventManager.Instance.OnDroneCamActivated += OnDroneCamActivated;
        EventManager.Instance.OnDroneCamDeactivated += OnDroneCamDeactivated;
        EventManager.Instance.OnGamePaused += OnGamePaused;
        EventManager.Instance.OnGameContinued += OnGameContinued;

        EventManager.Instance.OnLightsTurnedOn += OnLightsTurnedOn;
        EventManager.Instance.OnNexusCoreObtained += OnNexusCoreObtained;

        EventManager.Instance.OnLevelInstantiated += StopScanning;
        
        EventManager.Instance.OnPlayerDied += OnPlayerDied;

        EventManager.Instance.OnCountdownEnded += OnCountdownEnded;

        EventManager.Instance.OnLevelCompleted += OnLevelCompleted;
    }

    private void Start()
    {
        
        switch (LevelManager.Instance.GetGameDifficulty())
        {
            case GameDifficulty.EASY:
                startTimeInMinutes = 3.5f;
                break;
            case GameDifficulty.MODERATE:
                startTimeInMinutes = 3.0f;
                break;
            case GameDifficulty.HARD:
                startTimeInMinutes = 2.5f;
                break;
        }

    
        timeRemaining = startTimeInMinutes * 60.0f;
        UpdateTimerDisplay();
        InitializeSliders();
        
        objectiveInfoText.text = "OBJECTIVE: LOCATE THE CIRCUIT BREAKER AND ACTIVATE THE NEXUS VEINS.";
        
        generalCanvas.enabled = false;
        droneCamCanvas.enabled = false;
        droneCamController = droneCam.GetComponent<DroneCameraController>();
        
        mainCamCanvas.enabled = false;
        droneCamCanvas.enabled = true;
        
        levelStartedPanel.SetActive(false);
        gamePausedPanel.SetActive(false);

        isScanning = true;
        StartCoroutine(ScanningLoop());
    }

    private void OnDestroy()
    {
        EventManager.Instance.OnDroneCamActivated -= OnDroneCamActivated;
        EventManager.Instance.OnDroneCamDeactivated -= OnDroneCamDeactivated;
        EventManager.Instance.OnGamePaused -= OnGamePaused;
        EventManager.Instance.OnGameContinued -= OnGameContinued;
        
        EventManager.Instance.OnLightsTurnedOn -= OnLightsTurnedOn;
        EventManager.Instance.OnNexusCoreObtained -= OnNexusCoreObtained;
        
        EventManager.Instance.OnLevelInstantiated -= StopScanning;
        
        EventManager.Instance.OnPlayerDied -= OnPlayerDied;
        
        EventManager.Instance.OnCountdownEnded -= OnCountdownEnded;
        
        EventManager.Instance.OnLevelCompleted -= OnLevelCompleted;
    }

    private void Update()
    {
        if (isDroneCamActive) SetDroneCamIcons();
    }
    
    private void InitializeSliders()
    {
        masterVolumeSlider.value = DataManager.Instance.masterVolume;
        musicVolumeSlider.value = DataManager.Instance.musicVolume;
        sfxVolumeSlider.value = DataManager.Instance.sfxVolume;
        
        masterVolumeSlider.onValueChanged.AddListener(OnMasterVolumeChanged);
        musicVolumeSlider.onValueChanged.AddListener(OnMusicVolumeChanged);
        sfxVolumeSlider.onValueChanged.AddListener(OnSFXVolumeChanged);
    }
    
    private IEnumerator ScanningLoop()
    {
        string baseText = "Scanning";
        int dotCount = 0;

        while (isScanning)
        {
            scanningText.text = baseText + new string('.', dotCount);
            
            dotCount = (dotCount + 1) % 4;
            
            yield return new WaitForSeconds(interval);
        }
    }
    
    public void StopScanning()
    {
        isScanning = false;
        scanningText.gameObject.SetActive(false);
        OnScanningCompleted();
    }

    private IEnumerator TimerCountdown()
    {
        while (timeRemaining > 0.0f && isTimerRunning)
        {
            yield return new WaitForSeconds(1.0f);
            timeRemaining -= 1.0f;
            UpdateTimerDisplay();
        }

        if (timeRemaining <= 0)
        {
            timeRemaining = 0;
            UpdateTimerDisplay();
            TimerEnded();
        }
    }

    private void UpdateTimerDisplay()
    {
        int minutes = Mathf.FloorToInt(timeRemaining / 60); 
        int seconds = Mathf.FloorToInt(timeRemaining % 60); 
        
        timerText.text = $"{minutes:0}:{seconds:00}";
    }
    
    private void TimerEnded()
    {
        EventManager.Instance.InvokeOnCountdownEnded();
        // game over
    }

    private void StopTimer()
    {
        isTimerRunning = false;
    }

    private void StartTimer()
    {
        if (timeRemaining > 0)
            isTimerRunning = true;
    }

    public void UpdateDroneCamCountdown(float duration)
    {
        droneCamCountdownGO.GetComponent<TMP_Text>().text = "DRONE CAM UPLINK DURATION: " + (int)duration;
    }

    public void UpdateDroneCamAvailabilityText(bool available)
    {
        if (available)
        {
            droneCamAvailabilityText.text = "AVAILABLE";
            droneCamAvailabilityText.color = new Color(0.0f, 0.764151f, 0.0f);
        }
        else
        {
            droneCamAvailabilityText.text = "UNAVAILABLE";
            droneCamAvailabilityText.color = Color.red;
        }
    }

    public void UpdateSprayPaintUsesText(int usesLeft)
    {
        sprayPaintUsesText.text = "SPRAY PAINT USES LEFT: " + usesLeft;
    }

    public void RegisterTrackedObject(Transform trackedObject)
    {
        if (!trackedIcons.ContainsKey(trackedObject))
        {
            if (trackedObject.name.Equals("Player"))
            {
                GameObject newIcon = Instantiate(playerIconPrefab, droneCamObjects.transform);
                RectTransform iconRect = newIcon.GetComponent<RectTransform>();
                trackedIcons[trackedObject] = iconRect;
            } 
            else if (trackedObject.tag.Equals("Zombie"))
            {
                GameObject newIcon = Instantiate(zombieIconPrefab, droneCamObjects.transform);
                RectTransform iconRect = newIcon.GetComponent<RectTransform>();
                trackedIcons[trackedObject] = iconRect;
            }
            else if (trackedObject.tag.Equals("AmmoCase"))
            {
                GameObject newIcon = Instantiate(ammoCaseIconPrefab, droneCamObjects.transform);
                RectTransform iconRect = newIcon.GetComponent<RectTransform>();
                trackedIcons[trackedObject] = iconRect;
            }
            else if (trackedObject.tag.Equals("AK47Collectible"))
            {
                GameObject newIcon = Instantiate(ak47IconPrefab, droneCamObjects.transform);
                RectTransform iconRect = newIcon.GetComponent<RectTransform>();
                trackedIcons[trackedObject] = iconRect;
            }
            else if (trackedObject.tag.Equals("FirstAid"))
            {
                GameObject newIcon = Instantiate(firstAidIconPrefab, droneCamObjects.transform);
                RectTransform iconRect = newIcon.GetComponent<RectTransform>();
                trackedIcons[trackedObject] = iconRect;
            }
            else if (trackedObject.name.Equals("MazeExitDoor"))
            {
                GameObject newIcon = Instantiate(firstAidIconPrefab, droneCamObjects.transform);
                RectTransform iconRect = newIcon.GetComponent<RectTransform>();
                trackedIcons[trackedObject] = iconRect;
            }   
            else if (trackedObject.tag.Equals("CircuitBreaker"))
            {
                GameObject newIcon = Instantiate(circuitBreakerIconPrefab, droneCamObjects.transform);
                RectTransform iconRect = newIcon.GetComponent<RectTransform>();
                trackedIcons[trackedObject] = iconRect;
            }
            else if (trackedObject.tag.Equals("Entrance"))
            {
                GameObject newIcon = Instantiate(entranceIconPrefab, droneCamObjects.transform);
                RectTransform iconRect = newIcon.GetComponent<RectTransform>();
                trackedIcons[trackedObject] = iconRect;
            }
            else if (trackedObject.tag.Equals("Exit"))
            {
                GameObject newIcon = Instantiate(exitIconPrefab, droneCamObjects.transform);
                RectTransform iconRect = newIcon.GetComponent<RectTransform>();
                trackedIcons[trackedObject] = iconRect;
            }
            else if (trackedObject.tag.Equals("NexusCore"))
            {
                GameObject newIcon = Instantiate(nexusCoreIconPrefab, droneCamObjects.transform);
                RectTransform iconRect = newIcon.GetComponent<RectTransform>();
                trackedIcons[trackedObject] = iconRect;
            }
        }
       
    }
    
    public void UnregisterTrackedObject(Transform trackedObject)
    {
        if (trackedIcons.TryGetValue(trackedObject, out RectTransform iconRect))
        {
            Destroy(iconRect.gameObject);
            trackedIcons.Remove(trackedObject);
        }
    }

    public void SetDroneCamIcons()
    {
        foreach (var pair in trackedIcons)
        {
            Transform trackedObject = pair.Key;
            RectTransform iconRect = pair.Value;

            if (trackedObject != null)
            {
                // Convert world position to screen position
                Vector3 screenPosition = droneCam.WorldToScreenPoint(trackedObject.position);

                // Check if the object is within the Drone Camera's view
                if (screenPosition.z > 0 &&
                    screenPosition.x >= 0 && screenPosition.x <= Screen.width &&
                    screenPosition.y >= 0 && screenPosition.y <= Screen.height)
                {
                    iconRect.gameObject.SetActive(true); // Show the icon

                    // Convert screen position to local position in the canvas
                    RectTransformUtility.ScreenPointToLocalPointInRectangle(
                        canvasRectTransform, 
                        screenPosition, 
                        droneCam, 
                        out Vector2 localPoint
                    );

                    iconRect.anchoredPosition = localPoint; // Update position
                }
                else
                {
                    iconRect.gameObject.SetActive(false); // Hide if outside camera view
                }
            }
            else
            {
                // Remove null references
                UnregisterTrackedObject(trackedObject);
            }
        }
    }
    
    private void OnDroneCamActivated()
    {
        mainCamCanvas.enabled = false;
        //droneCamCanvas.gameObject.SetActive(true);
        droneCamCanvas.enabled = true;
        droneCamObjects.enabled = true;
        isDroneCamActive = true;

        if (LevelManager.Instance.GetGameDifficulty() == GameDifficulty.MODERATE ||
            LevelManager.Instance.GetGameDifficulty() == GameDifficulty.HARD && !LevelManager.Instance.HasLevelStarted)
        {
            droneCamCountdownGO.SetActive(true);
        }
    }

    private void OnDroneCamDeactivated()
    {
        isDroneCamActive = false;
        droneCamController.SetProjectionSize();
        droneCamController.SetStartingPosition();
        //droneCamCanvas.gameObject.SetActive(false);
        droneCamObjects.enabled = false;
        droneCamCanvas.enabled = false;
        mainCamCanvas.enabled = true;
    }

    private void OnGamePaused()
    {
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        isPaused = true;
        StopTimer();
        generalCanvas.enabled = true;
        gamePausedPanel.SetActive(true);
    }

    private void OnGameContinued()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        isPaused = false;
        StartTimer();
        gamePausedPanel.SetActive(false);
        generalCanvas.enabled = false;
    }

    private void OnLightsTurnedOn()
    {
        objectiveInfoText.text = "OBJECTIVE: RETRIEVE THE NEXUS CORE FROM ITS PORT.";
    }

    private void OnNexusCoreObtained()
    {
        objectiveInfoText.color = Color.red;
        objectiveInfoText.text = "ESCAPE THE LAB!";
        droneCamAvailabilityText.transform.parent.gameObject.SetActive(false);
        sprayPaintUsesText.gameObject.SetActive(false);
        timerText.gameObject.SetActive(true);
        StartCoroutine(TimerCountdown());
    }

    public void OnScanningCompleted()
    {
        generalCanvas.enabled = true;
        levelStartedPanel.SetActive(true);
    }

    public void OnReadyClicked()
    {
        levelInfo.SetActive(false);
        
        switch (LevelManager.Instance.GetGameDifficulty())
        {
            case GameDifficulty.EASY:
                easyLevelInfo.SetActive(true);
                break;
            case GameDifficulty.MODERATE:
                moderateLevelInfo.SetActive(true);
                break;
            case GameDifficulty.HARD:
                hardLevelInfo.SetActive(true);
                break;
        }
        
        readyButton.SetActive(false);
        startButton.SetActive(true);
    }
    
    public void OnStartClicked()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        LevelManager.Instance.readyButtonPressed = true;
        generalCanvas.enabled = false;
        levelStartedPanel.SetActive(false);
    }
    
    private void OnMasterVolumeChanged(float value)
    {
        DataManager.Instance.masterVolume = value;
        DataManager.Instance.SaveVolumeSettings();
        AudioManager.Instance.OnMasterVolumeChanged(value);
        EventManager.Instance.InvokeOnVolumeChanged();
    }

    private void OnMusicVolumeChanged(float value)
    {
        DataManager.Instance.musicVolume = value;
        DataManager.Instance.SaveVolumeSettings();
        AudioManager.Instance.OnMusicVolumeChanged(value);
    }

    private void OnSFXVolumeChanged(float value)
    {
        DataManager.Instance.sfxVolume = value;
        DataManager.Instance.SaveVolumeSettings();
        AudioManager.Instance.OnSFXVolumeChanged(value);
        EventManager.Instance.InvokeOnVolumeChanged();
    }

    private void OnPlayerDied()
    {
        youDiedText.text = "YOU HAVE BEEN SLAUGHTERED BY THE ZOMBIES.";
        StartCoroutine(PlayerDiedCoroutine());
    }
    
    private void OnCountdownEnded()
    {
        youDiedText.text = "THE NEXUS HAS SELF-DESTRUCTED, OBLITERATING YOU AND THE CORE.";
        StartCoroutine(CountdownEndCoroutine());
    }

    private void OnLevelCompleted(GameDifficulty difficulty)
    {
        StartCoroutine(LevelCompletedCoroutine());
    }
    
    private IEnumerator PlayerDiedCoroutine()
    {
        yield return new WaitForSeconds(2.0f);
        yield return StartCoroutine(FadeOut());
    }

    private IEnumerator CountdownEndCoroutine()
    {
        yield return StartCoroutine(FadeOut());
    }

    private IEnumerator LevelCompletedCoroutine()
    {
        StopTimer();
        yield return new WaitForSeconds(2.0f);
        yield return StartCoroutine(FadeOut());
    }
    
    private IEnumerator FadeOut()
    {
        fadeOutImage.gameObject.SetActive(true);

        Color fadeColor = fadeOutImage.color;
        fadeColor.a = 0f;
        fadeOutImage.color = fadeColor;

        float elapsedTime = 0f;
        
        while (elapsedTime < fadeDuration)
        {
            elapsedTime += Time.deltaTime;
            fadeColor.a = Mathf.Clamp01(elapsedTime / fadeDuration);
            fadeOutImage.color = fadeColor;

            yield return null; 
        }
        
        fadeColor.a = 1f;
        fadeOutImage.color = fadeColor;

        OnFadeComplete();
    }

    private void OnFadeComplete()
    {
        generalCanvas.enabled = true;
        crosshair.enabled = false;
        
        GameManager.Instance.Player.SetActive(false);
        
        if (GameManager.Instance.State == GameState.LevelFailed) playerDiedPanel.SetActive(true);
        else if (GameManager.Instance.State == GameState.LevelCompleted) levelCompletedPanel.SetActive(true);
    }
    
}
