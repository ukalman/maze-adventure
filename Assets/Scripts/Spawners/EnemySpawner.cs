using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

// Attached to Maze Game Object
public class EnemySpawner : MonoBehaviour
{

    /* If selectedDifficulty:
     * EASY -> 5 zombie groups instantiated at first, 10 dist from spawn point
     * MODERATE -> 8 zombie groups instantiated at first, 18 dist from spawn point
     * HARD -> 12 zombie groups instantiated at first, 25 dist from spawn point
     */
    
    private MazeGenerator mazeGenerator;

    private int maxZombieCount;
    private int currentZombieCount = 0;

    [SerializeField] private GameObject zombieGroupPrefab_3;
    [SerializeField] private GameObject zombieGroupPrefab_2;

    private Dictionary<GameObject,int> zombieGroups = new Dictionary<GameObject, int>();
    private int zombieGroupCount;
    private int distFromSpawnPoint;
    
    private void Start()
    {
        mazeGenerator = GetComponent<MazeGenerator>();
        EventManager.Instance.OnNavMeshBaked += OnNavMeshBaked;
        //EventManager.Instance.OnMazeGenerated += OnMazeGenerated;
        EventManager.Instance.OnEnemyKilled += OnEnemyKilled;

        EventManager.Instance.OnEnemyAwaken += OnEnemyAwaken;

        switch (LevelManager.Instance.GetGameDifficulty())
        {
            case GameDifficulty.EASY:
                maxZombieCount = 9;
                zombieGroupCount = 3;
                distFromSpawnPoint = 5;
                break;
            case GameDifficulty.MODERATE:
                maxZombieCount = 12;
                zombieGroupCount = 4;
                distFromSpawnPoint = 7;
                break;
            case GameDifficulty.HARD:
                maxZombieCount = 15;
                zombieGroupCount = 5;
                distFromSpawnPoint = 12;
                break;
            case GameDifficulty.None:
                Debug.LogError("[EnemySpawner] Game Difficulty: None");
                break;
        }
    }

    private void OnDestroy()
    {
        EventManager.Instance.OnNavMeshBaked -= OnNavMeshBaked;
        EventManager.Instance.OnEnemyKilled -= OnEnemyKilled;
        //EventManager.Instance.OnMazeGenerated -= OnMazeGenerated;
        
        EventManager.Instance.OnEnemyAwaken -= OnEnemyAwaken;
    }

    private void OnNavMeshBaked()
    {
        
        for (int i = 0; i < zombieGroupCount; i++)
        {
            SpawnZombieGroup(GetRandomSpawnPoint());
        }
        
        
        EventManager.Instance.InvokeOnLevelInstantiated();
    }

    private void OnEnemyKilled()
    {
        currentZombieCount--;
    }
    
    
    private void SpawnZombieGroup(Vector3 spawnPoint)
    {
        zombieGroups.Add(Instantiate(zombieGroupPrefab_3, spawnPoint, Quaternion.identity, LevelManager.Instance.WorldObjectsParent), 3);
        currentZombieCount += 3;
        
        /*
        // Instantiate zombie group, new field in zombies for the group id's, add the groups in the zombieGroupsList
        if (Random.Range(1, 6) <= 2)
        {
            // 3
            zombieGroups.Add(Instantiate(zombieGroupPrefab_3, spawnPoint, Quaternion.identity, LevelManager.Instance.WorldObjectsParent), 3);
            currentZombieCount += 3;
        }
        else
        {
            zombieGroups.Add(Instantiate(zombieGroupPrefab_2, spawnPoint, Quaternion.identity, LevelManager.Instance.WorldObjectsParent), 2);
            currentZombieCount += 2;
        }
        */
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
        
        return spawnPoint;

    }

    private void OnEnemyAwaken()
    {
        currentZombieCount++;
    }
    
}
