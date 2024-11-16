
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using Random = UnityEngine.Random;

public class EnemySpawner : MonoBehaviour
{

    /* If difficulty:
     * EASY -> 5 zombie groups instantiated at first, 10 dist from spawn point
     * MODERATE -> 8 zombie groups instantiated at first, 18 dist from spawn point
     * HARD -> 12 zombie groups instantiated at first, 25 dist from spawn point
     */
    
    
    private MazeGenerator mazeGenerator; 
    
    private MazeCell topRightCell;
    private MazeCell bottomLeftCell;
    private MazeCell bottomRightCell;

    private int totalEnemyCount;

    [SerializeField] private GameObject zombieGroupPrefab_3;
    [SerializeField] private GameObject zombieGroupPrefab_2;

    private List<GameObject> zombieGroups = new List<GameObject>();
    private int zombieGroupCount;
    private float distFromSpawnPoint;
    
    private void Start()
    {
        mazeGenerator = GetComponent<MazeGenerator>();
        EventManager.Instance.OnMazeGenerated += OnMazeGenerated;
        EventManager.Instance.OnEnemyDied += OnEnemyDied;

        switch (LevelManager.Instance.GetGameDifficulty())
        {
            case GameDifficulty.EASY:
                zombieGroupCount = 5;
                distFromSpawnPoint = 10.0f;
                break;
            case GameDifficulty.MODERATE:
                zombieGroupCount = 8;
                distFromSpawnPoint = 18.0f;
                break;
            case GameDifficulty.HARD:
                zombieGroupCount = 12;
                distFromSpawnPoint = 22.0f;
                break;
            case GameDifficulty.None:
                Debug.LogError("[EnemySpawner] Game Difficulty: None");
                break;
        }
    }

    private void OnDestroy()
    {
        EventManager.Instance.OnMazeGenerated -= OnMazeGenerated;
        EventManager.Instance.OnEnemyDied -= OnEnemyDied;
    }

    private void OnMazeGenerated()
    {
        topRightCell = mazeGenerator.GetTopRightCell();
        bottomLeftCell = mazeGenerator.GetBottomLeftCell();
        bottomRightCell = mazeGenerator.GetBottomRightCell();

        for (int i = 0; i < zombieGroupCount; i++)
        {
            InstantiateZombieGroup(GetRandomSpawnPoint());
        }
        
        
    }

    private void OnEnemyDied(int id)
    {
        
    }
    
    
    private void InstantiateZombieGroup(Vector3 spawnPoint)
    {
        // Instantiate zombie group, new field in zombies for the group id's, add the groups in the zombieGroupsList
    }

    private Vector3 GetRandomSpawnPoint()
    {
        int cell = Random.Range(0, 2);
        Vector3 spawnPoint = Vector3.zero;
        if (cell == 1) // top right cell
        {
            spawnPoint = Extensions.GetRandomPointOnNavMesh(topRightCell.transform.position, distFromSpawnPoint);
        }
        else if (cell == 2) // bottom left cell
        {
            spawnPoint = Extensions.GetRandomPointOnNavMesh(bottomLeftCell.transform.position, distFromSpawnPoint);
        }
        else if (cell == 3) // bottom right cell
        {
            spawnPoint = Extensions.GetRandomPointOnNavMesh(bottomRightCell.transform.position, distFromSpawnPoint);
        }

        return spawnPoint;
    }
    
    
}
