
using System;
using UnityEngine;

public class LevelManager : MonoBehaviour
{
    private CameraSwitcher cameraSwitcher;
    private int switchRight = 3;
    
    [SerializeField] private Camera mainCam, droneCam, minimapCam;

    private void Awake()
    {
        cameraSwitcher = new CameraSwitcher(mainCam, droneCam, minimapCam);
    }

    private void Start()
    {
        cameraSwitcher.ActivateMainCam();
    }

    private void Update()
    {
        CheckForCameraSwitch();
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
