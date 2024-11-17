
using System;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.AI;
using Random = UnityEngine.Random;

// Attached to Maze Game Object
public class EnemySpawner : MonoBehaviour
{

    /* If difficulty:
     * EASY -> 5 zombie groups instantiated at first, 10 dist from spawn point
     * MODERATE -> 8 zombie groups instantiated at first, 18 dist from spawn point
     * HARD -> 12 zombie groups instantiated at first, 25 dist from spawn point
     */
    
    
    private MazeGenerator mazeGenerator; 
    
    private int totalZombieCount = 0;

    [SerializeField] private GameObject zombieGroupPrefab_3;
    [SerializeField] private GameObject zombieGroupPrefab_2;

    private List<GameObject> zombieGroups = new List<GameObject>();
    private int zombieGroupCount;
    private int distFromSpawnPoint;
    
    private void Start()
    {
        mazeGenerator = GetComponent<MazeGenerator>();
        EventManager.Instance.OnNavMeshBaked += OnNavMeshBaked;
        //EventManager.Instance.OnMazeGenerated += OnMazeGenerated;
        EventManager.Instance.OnEnemyDied += OnEnemyDied;

        switch (LevelManager.Instance.GetGameDifficulty())
        {
            case GameDifficulty.EASY:
                zombieGroupCount = 5;
                distFromSpawnPoint = 5;
                break;
            case GameDifficulty.MODERATE:
                zombieGroupCount = 8;
                distFromSpawnPoint = 10;
                break;
            case GameDifficulty.HARD:
                zombieGroupCount = 12;
                distFromSpawnPoint = 18;
                break;
            case GameDifficulty.None:
                Debug.LogError("[EnemySpawner] Game Difficulty: None");
                break;
        }
    }

    private void OnDestroy()
    {
        EventManager.Instance.OnNavMeshBaked -= OnNavMeshBaked;
        //EventManager.Instance.OnMazeGenerated -= OnMazeGenerated;
        EventManager.Instance.OnEnemyDied -= OnEnemyDied;
    }

    private void OnNavMeshBaked()
    {
        for (int i = 0; i < zombieGroupCount; i++)
        {
            SpawnZombieGroup(GetRandomSpawnPoint());
        }
        
    }

    private void OnEnemyDied(int id)
    {
        
    }
    
    
    private void SpawnZombieGroup(Vector3 spawnPoint)
    {
        // Instantiate zombie group, new field in zombies for the group id's, add the groups in the zombieGroupsList
        if (Random.Range(1, 6) <= 2)
        {
            // 3
            zombieGroups.Add(Instantiate(zombieGroupPrefab_3, spawnPoint, Quaternion.identity, LevelManager.Instance.WorldObjectsParent));
            totalZombieCount += 3;
        }
        else
        {
            zombieGroups.Add(Instantiate(zombieGroupPrefab_2, spawnPoint, Quaternion.identity, LevelManager.Instance.WorldObjectsParent));
            totalZombieCount += 2;
        }
    }

    private Vector3 GetRandomSpawnPoint()
    {
        int cell = Random.Range(0, 3);
        Vector3 spawnPoint = Vector3.zero;
        MazeCell mazeCell;
        if (cell == 0) // top right cell
        {
            mazeCell = Extensions.GetRandomElementWithinDistance(mazeGenerator.GetMazeGrid(), 0,
                mazeGenerator.GetMazeDepth() - 1, distFromSpawnPoint);
            spawnPoint = mazeCell.transform.position;
            //spawnPoint = Extensions.GetRandomPointOnNavMesh(topRightCell.transform.position, distFromSpawnPoint);
        }
        else if (cell == 1) // bottom left cell
        {
            mazeCell = Extensions.GetRandomElementWithinDistance(mazeGenerator.GetMazeGrid(), mazeGenerator.GetMazeWidth()-1,
                0, distFromSpawnPoint);
            spawnPoint = mazeCell.transform.position;
            //spawnPoint = Extensions.GetRandomPointOnNavMesh(bottomLeftCell.transform.position, distFromSpawnPoint);
        }
        else if (cell == 2) // bottom right cell
        {
            mazeCell = Extensions.GetRandomElementWithinDistance(mazeGenerator.GetMazeGrid(), mazeGenerator.GetMazeWidth()-1,
                mazeGenerator.GetMazeDepth() -1, distFromSpawnPoint);
            spawnPoint = mazeCell.transform.position;
            //spawnPoint = Extensions.GetRandomPointOnNavMesh(bottomRightCell.transform.position, distFromSpawnPoint);
        }
        else
        {
            Debug.Log("Cell value: " + cell);
        }
        
        return spawnPoint;

    }
    
    
}
