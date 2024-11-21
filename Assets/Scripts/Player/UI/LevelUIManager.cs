
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;

public class LevelUIManager : MonoBehaviour
{
    
    [SerializeField] private Canvas mainCamCanvas;
    [SerializeField] private Canvas droneCamObjects;
    [SerializeField] private Canvas droneCamCanvas;
    [SerializeField] private Canvas generalCanvas;
    [SerializeField] private RectTransform canvasRectTransform;

    [SerializeField] private TMP_Text objectiveInfoText;
    [SerializeField] private TMP_Text scanningText;
    
    [SerializeField] private GameObject playerIconPrefab;
    [SerializeField] private GameObject zombieIconPrefab;
    [SerializeField] private GameObject ak47IconPrefab;
    [SerializeField] private GameObject ammoCaseIconPrefab;
    [SerializeField] private GameObject firstAidIconPrefab;
    [SerializeField] private GameObject circuitBreakerIconPrefab;
    [SerializeField] private GameObject nexusCoreIconPrefab;
    [SerializeField] private GameObject entranceIconPrefab;
    [SerializeField] private GameObject exitIconPrefab;

    [SerializeField] Camera droneCam;
    
    [SerializeField] private GameObject gamePausedPanel;
    [SerializeField] private GameObject levelStartedPanel;
    
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
    }

    private void Start()
    {
        
        switch (LevelManager.Instance.GetGameDifficulty())
        {
            case GameDifficulty.EASY:
                startTimeInMinutes = 3.0f;
                break;
            case GameDifficulty.MODERATE:
                startTimeInMinutes = 4.0f;
                break;
            case GameDifficulty.HARD:
                startTimeInMinutes = 5.0f;
                break;
        }
        
        timeRemaining = startTimeInMinutes * 60.0f;
        UpdateTimerDisplay();
        
        objectiveInfoText.text = "Objective: Locate the circuit breaker and activate the Nexus veins.";
        
        generalCanvas.enabled = false;
        droneCamCanvas.enabled = false;
        droneCamController = droneCam.GetComponent<DroneCameraController>();
        
        mainCamCanvas.enabled = false;
        droneCamCanvas.enabled = true;

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
    }

    private void Update()
    {
        if (isDroneCamActive) SetDroneCamIcons();
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
        Debug.Log("Timer has ended!");
        // game over
    }

    public void StopTimer()
    {
        isTimerRunning = false;
    }

    public void StartTimer()
    {
        if (timeRemaining > 0)
            isTimerRunning = true;
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
            else
            {
                Debug.Log("nah, tracked object: " + trackedObject.name);
            }
            //GameObject newIcon = Instantiate(iconPrefab, droneCamObjects.transform);
            //RectTransform iconRect = newIcon.GetComponent<RectTransform>();
            //trackedIcons[trackedObject] = iconRect;
        }
        else
        {
            Debug.Log("Object is contained!");
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

    public void DestroyDroneCamIcons()
    {
        //trackedIcons.Clear();
    }


    private void OnDroneCamActivated()
    {
        mainCamCanvas.enabled = false;
        //droneCamCanvas.gameObject.SetActive(true);
        droneCamCanvas.enabled = true;
        droneCamObjects.enabled = true;
        isDroneCamActive = true;
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
        StopGame();
        isPaused = true;
        StopTimer();
    }

    private void OnGameContinued()
    {
        ContinueGame();
        isPaused = false;
        StartTimer();
    }

    private void OnLightsTurnedOn()
    {
        objectiveInfoText.text = "Objective: Retrieve the Nexus core from its port.";
    }

    private void OnNexusCoreObtained()
    {
        objectiveInfoText.color = Color.red;
        objectiveInfoText.text = "ESCAPE THE LAB!";
        timerText.gameObject.SetActive(true);
        StartCoroutine(TimerCountdown());
    }

    public void StopGame()
    {
        generalCanvas.enabled = true;
        gamePausedPanel.SetActive(true);
    }
    
    public void ContinueGame()
    {
        EventManager.Instance.InvokeOnGameContinued();
        generalCanvas.enabled = false;
        gamePausedPanel.SetActive(false);
    }

    public void OnScanningCompleted()
    {
        generalCanvas.enabled = true;
        levelStartedPanel.SetActive(true);
    }

    public void OnReadyButtonPressed()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        LevelManager.Instance.readyButtonPressed = true;
        generalCanvas.enabled = false;
        levelStartedPanel.SetActive(false);
    }
    

}
