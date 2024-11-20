using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MazeEntrance : MonoBehaviour
{
    private void Awake()
    {
        EventManager.Instance.OnDroneCamActivated += OnDroneCamActivated;
        EventManager.Instance.OnDroneCamDeactivated += OnDroneCamDeactivated;
    }
    
    void OnDestroy()
    {
        EventManager.Instance.OnDroneCamActivated -= OnDroneCamActivated;
        EventManager.Instance.OnDroneCamDeactivated -= OnDroneCamDeactivated;
    }
    
    private void OnDroneCamActivated()
    {
        LevelManager.Instance.levelUIManager.RegisterTrackedObject(transform);
    }

    private void OnDroneCamDeactivated()
    {
        LevelManager.Instance.levelUIManager.UnregisterTrackedObject(transform);
    }
}
