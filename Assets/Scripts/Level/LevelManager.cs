
using System;
using UnityEngine;

public class LevelManager : MonoBehaviour
{
    public static LevelManager Instance { get; private set; }

    private GameDifficulty Difficulty;

    #region CameraSwitch

    private CameraSwitcher cameraSwitcher;
    private int switchRight = 3;

    #endregion

    public bool collectedAK47;
    
    
    [SerializeField] private Camera mainCam, droneCam, minimapCam;
    private void Awake()
    {
        SetGameDifficulty(GameDifficulty.MODERATE);
        
        if (Instance == null)
        {
            Instance = this;
        }
        
        cameraSwitcher = new CameraSwitcher(mainCam, droneCam, minimapCam);
    }

    private void Start()
    {
        cameraSwitcher.DisableCameras();
        cameraSwitcher.ActivateMainCam();
    }

    private void Update()
    {
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
        if (Input.GetKeyDown(KeyCode.C))
        {
            if (mainCam.gameObject.activeSelf)
            {
                if (switchRight > 0)
                {
                    cameraSwitcher.ActivateDroneCam();
                    switchRight--;
                }
            }
            else if (droneCam.gameObject.activeSelf)
            {
                cameraSwitcher.ActivateMainCam();
            }
        }
    }
    
    
}
