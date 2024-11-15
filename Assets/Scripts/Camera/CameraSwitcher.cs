using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraSwitcher 
{
    private Camera mainCam, droneCam, minimapCam;

    public CameraSwitcher(Camera main, Camera drone, Camera minimap)
    {
        mainCam = main;
        droneCam = drone;
        minimapCam = minimap;
    }
    
    public void ActivateMainCam()
    {
        droneCam.gameObject.SetActive(false);
        mainCam.gameObject.SetActive(true);
        minimapCam.gameObject.SetActive(true);
    }

    public void ActivateDroneCam()
    {
        mainCam.gameObject.SetActive(false);
        minimapCam.gameObject.SetActive(false);
        droneCam.gameObject.SetActive(true);
    }
    
    public void DisableCameras()
    {
        mainCam.gameObject.SetActive(false);
        minimapCam.gameObject.SetActive(false);
        droneCam.gameObject.SetActive(false);
    }

}
