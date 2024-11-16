using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DroneCameraController : MonoBehaviour
{
    [SerializeField] private Camera droneCam;
    [SerializeField] private MazeGenerator maze;
    [SerializeField] private float camHeight;
    [SerializeField] private float cameraTiltAngle = 90.0f;

    [SerializeField] private float camHeightFactor = 9.0f;
    
    private void Start()
    {
        camHeight = maze.GetGreaterEdge() * camHeightFactor;
        PositionCamera();
    }

    private void SetCamHeight()
    {
        switch (LevelManager.Instance.GetGameDifficulty())
        {
            case GameDifficulty.None:
                break;
            case GameDifficulty.EASY:
                camHeightFactor = 9.0f;
                break;
            case GameDifficulty.MODERATE:
                camHeightFactor = 15.0f;
                break;
            case GameDifficulty.HARD:
                camHeightFactor = 20.0f;
                break;
        }

        camHeight = maze.GetGreaterEdge() * camHeightFactor;
    }
    
    private void PositionCamera()
    {
        if (maze == null || droneCam == null)
        {
            Debug.LogError("MazeGenerator or Camera not assigned!");
            return;
        }

        Vector3 mazeScale = maze.transform.localScale;
        
        // Calculate maze dimensions in world space
        float mazeWorldWidth = maze.GetMazeWidth() * mazeScale.x;
        float mazeWorldDepth = maze.GetMazeWidth() * mazeScale.z;

        // Calculate maze center in world space
        float centerX = maze.transform.position.x + mazeWorldWidth / 2.0f - (mazeScale.x / 2.0f);
        float centerZ = maze.transform.position.z + mazeWorldDepth / 2.0f - (mazeScale.z / 2.0f);

        
        droneCam.transform.position = new Vector3(centerX, camHeight, centerZ);
        droneCam.transform.rotation = Quaternion.Euler(cameraTiltAngle, 0.0f, 0.0f);
    }
}
