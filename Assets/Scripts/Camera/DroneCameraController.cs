using System;
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
    
    [SerializeField] private float zoomSpeed = 2000.0f; 
    [SerializeField] private float moveSpeed = 20.0f; 
    [SerializeField] private float maxOrthographicSize = 62.1f; 
    [SerializeField] private float minOrthographicSize = 15.0f; 
    
    [Header("Position Boundaries")]
    [SerializeField] private float minX = -6.0f;
    [SerializeField] private float maxX = 90.0f;
    [SerializeField] private float minZ = -10.0f;
    [SerializeField] private float maxZ = 80.0f;
    [SerializeField] private float projectionSize;
    
    private Vector3 startingPosition;
    
    private void Start()
    {
        camHeight = maze.GetGreaterEdge() * camHeightFactor;
        switch (LevelManager.Instance.GetGameDifficulty())
        {
            case GameDifficulty.EASY:
                minX = -6.0f;
                maxX = 90.0f;
                minZ = -10.0f;
                maxZ = 80.0f;
                projectionSize = 62.1f;
                break;
            case GameDifficulty.MODERATE:
                minX = -6.0f;
                maxX = 120.0f;
                minZ = -10.0f;
                maxZ = 120.0f;
                projectionSize = 75.0f;
                break;
            case GameDifficulty.HARD:
                minX = -15.0f;
                maxX = 180.0f;
                minZ = -10.0f;
                maxZ = 180.0f;
                projectionSize = 100.0f;
                break;
            
        }
        maxOrthographicSize = projectionSize;
        SetProjectionSize();
        PositionCamera();
    }

    private void Update()
    {
        if (!LevelManager.Instance.LevelInstantiated) return;
        
        HandleZoom();
        HandleMovement();
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

        startingPosition = new Vector3(centerX, camHeight, centerZ);
    }
    
    private void HandleZoom()
    {
        float scroll = Input.GetAxis("Mouse ScrollWheel");

        if (scroll != 0)
        {
            droneCam.orthographicSize -= scroll * zoomSpeed * Time.deltaTime;

            // Clamp the size to stay within the allowed range
            droneCam.orthographicSize = Mathf.Clamp(droneCam.orthographicSize, minOrthographicSize, maxOrthographicSize);
        }
    }

    private void HandleMovement()
    {
        // Get input for movement (WASD keys)
        float moveX = 0f;
        float moveZ = 0f;

        if (Input.GetKey(KeyCode.W)) moveZ += 1f; // Move forward (increase Z)
        if (Input.GetKey(KeyCode.S)) moveZ -= 1f; // Move backward (decrease Z)
        if (Input.GetKey(KeyCode.A)) moveX -= 1f; // Move left (decrease X)
        if (Input.GetKey(KeyCode.D)) moveX += 1f; // Move right (increase X)

        // Apply movement to the camera's position
        Vector3 newPosition = transform.position;
        newPosition.x += moveX * moveSpeed * Time.deltaTime;
        newPosition.z += moveZ * moveSpeed * Time.deltaTime;

        // Clamp the position to stay within boundaries
        newPosition.x = Mathf.Clamp(newPosition.x, minX, maxX);
        newPosition.z = Mathf.Clamp(newPosition.z, minZ, maxZ);

        // Set the new position
        transform.position = newPosition;
    }

    public void SetProjectionSize()
    {
        droneCam.orthographicSize = projectionSize;
    }

    public void SetStartingPosition()
    {
        droneCam.transform.position = startingPosition;
    }
    
    
}
