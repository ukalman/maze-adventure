
// Ammo and Weapon (AK-47 Spawn)

using System;
using UnityEngine;
using Random = UnityEngine.Random;

public class AmmoSpawner : MonoBehaviour
{
    private MazeGenerator mazeGenerator;

    [SerializeField] private GameObject AK47Prefab;
    [SerializeField] private GameObject AK47TablePrefab;
    [SerializeField] private GameObject AmmoCaseAK47Prefab;
    [SerializeField] private GameObject AmmoCaseM4Prefab;

    private int AK47AmmoCaseCount;
    private int M4AmmoCaseCount;
    private int AK47Count;

    private int distFromSpawnPoint;
    
    private void Start()
    {
        mazeGenerator = GetComponent<MazeGenerator>();
        EventManager.Instance.OnMazeGenerated += OnMazeGenerated;
        EventManager.Instance.OnExtraAmmoDepleted += OnExtraAmmoDepleted;
        EventManager.Instance.OnAmmoCollected += OnAmmoCollected;
        AK47Count = Random.Range(1, 3);
        
        switch (LevelManager.Instance.GetGameDifficulty())
        {
            case GameDifficulty.EASY:
                AK47AmmoCaseCount = Random.Range(2, 4); // 2 to 3
                M4AmmoCaseCount = Random.Range(3,5); // 3 to 4
                distFromSpawnPoint = 8;
                break;
            case GameDifficulty.MODERATE:
                AK47AmmoCaseCount = Random.Range(2, 5); // 2 to 4
                M4AmmoCaseCount = Random.Range(2,5); // 2 to 5
                distFromSpawnPoint = 12;
                break;
            case GameDifficulty.HARD:
                AK47AmmoCaseCount = Random.Range(1, 4); // 1 to 3
                M4AmmoCaseCount = Random.Range(2,4); // 2 to 3
                distFromSpawnPoint = 20;
                break;
            case GameDifficulty.None:
                Debug.LogError("[AmmoSpawner] Game Difficulty: None");
                break;
        }
        
    }

    private void OnDestroy()
    {
        EventManager.Instance.OnMazeGenerated -= OnMazeGenerated;
        EventManager.Instance.OnExtraAmmoDepleted -= OnExtraAmmoDepleted;
        EventManager.Instance.OnAmmoCollected -= OnAmmoCollected;
    }

    // Instantiate ammo cases and AK-47s (1 or 2) somewhere randomly
    private void OnMazeGenerated()
    {
        for (int i = 0; i < AK47Count; i++)
        {
            SpawnAK47(GetRandomSpawnMazeCell());
        }
        
        for (int i = 0; i < M4AmmoCaseCount; i++)
        {
            SpawnM4AmmoCase(GetRandomSpawnMazeCell());
        }

        for (int j = 0; j < AK47AmmoCaseCount; j++)
        {
            SpawnAK47AmmoCase(GetRandomSpawnMazeCell());
        }
        
        EventManager.Instance.InvokeOnRequirementsBeforeNavMeshSpawned();
    }
    
    private void OnExtraAmmoDepleted(AmmoType ammoType)
    {
        // Instantiate 1-2 ammo cases somewhere randomly
        if (ammoType == AmmoType.M4_556 && M4AmmoCaseCount == 0)
        {
            int randomQuantity = Random.Range(1, 3);
            M4AmmoCaseCount += randomQuantity;
            SpawnM4AmmoCase(GetRandomSpawnMazeCell());
        }
        else if (ammoType == AmmoType.AK47_762 && AK47AmmoCaseCount == 0)
        {
            int randomQuantity = Random.Range(1, 3);
            AK47AmmoCaseCount += randomQuantity;
            SpawnAK47AmmoCase(GetRandomSpawnMazeCell());
        }
    }

    private void OnAmmoCollected(AmmoType ammoType, int amount)
    {
        if (ammoType == AmmoType.M4_556)
        {
            M4AmmoCaseCount--;
        }
        else if (ammoType == AmmoType.AK47_762)
        {
            AK47AmmoCaseCount--;
        }
    }

    private void SpawnAK47(Transform mazeCell)
    {
        if (Random.Range(1, 5) <= 2)
        {
            var AK47Table = Instantiate(AK47TablePrefab, mazeCell.position, Quaternion.identity, mazeCell);
            
            Vector3 currentLocalScale = AK47Table.transform.localScale;
            
            AK47Table.transform.localScale = new Vector3(
                currentLocalScale.x / mazeGenerator.GetScaleX(),
                currentLocalScale.y / mazeGenerator.GetScaleY(),
                currentLocalScale.z / mazeGenerator.GetScaleZ()
            );
            
            float randomXOffset = Random.Range(-0.34f, 0.34f);
            float randomZOffset = Random.Range(-0.34f, 0.34f);
            
            Vector3 currentLocalPosition = AK47Table.transform.localPosition;
            AK47Table.transform.localPosition = new Vector3(
                currentLocalPosition.x + randomXOffset,
                currentLocalPosition.y + 0.07f,
                currentLocalPosition.z + randomZOffset
            );
        }
        else
        {
            var AK47 = Instantiate(AK47Prefab, mazeCell.position, Quaternion.identity, mazeCell);
            
            Vector3 currentLocalScale = AK47.transform.localScale;
            
            AK47.transform.localScale = new Vector3(
                currentLocalScale.x / mazeGenerator.GetScaleX(),
                currentLocalScale.y / mazeGenerator.GetScaleY(),
                currentLocalScale.z / mazeGenerator.GetScaleZ()
            );
            
            float randomXOffset = Random.Range(-0.34f, 0.34f);
            float randomZOffset = Random.Range(-0.34f, 0.34f);
            
            Vector3 currentLocalPosition = AK47.transform.localPosition;
            AK47.transform.localPosition = new Vector3(
                currentLocalPosition.x + randomXOffset,
                currentLocalPosition.y,
                currentLocalPosition.z + randomZOffset
            );
            //AK47.transform.position.Set(AK47.transform.position.x + Random.Range(-10,10), AK47.transform.position.y, AK47.transform.position.z + Random.Range(-10,10));
        }
        
    }
    
    private void SpawnAK47AmmoCase(Transform mazeCell)
    {
        var ammoCase = Instantiate(AmmoCaseAK47Prefab, mazeCell.position, Quaternion.identity, mazeCell);
        //ammoCase.transform.position.Set(ammoCase.transform.position.x + Random.Range(-5,5), ammoCase.transform.position.y, ammoCase.transform.position.z + Random.Range(-5,5));
        
        Vector3 currentLocalScale = ammoCase.transform.localScale;
            
        ammoCase.transform.localScale = new Vector3(
            currentLocalScale.x / mazeGenerator.GetScaleX(),
            currentLocalScale.y / mazeGenerator.GetScaleY(),
            currentLocalScale.z / mazeGenerator.GetScaleZ()
        );
        
        float randomXOffset = Random.Range(-0.34f, 0.34f);
        float randomZOffset = Random.Range(-0.34f, 0.34f);
        
        Vector3 currentLocalPosition = ammoCase.transform.localPosition;
        ammoCase.transform.localPosition = new Vector3(
            currentLocalPosition.x + randomXOffset,
            currentLocalPosition.y,
            currentLocalPosition.z + randomZOffset
        );
        
        float randomYRotation = Random.Range(-180f, 180f);
        ammoCase.transform.rotation = Quaternion.Euler(0f, randomYRotation, 0f);
    }

    private void SpawnM4AmmoCase(Transform mazeCell)
    {
        var ammoCase = Instantiate(AmmoCaseM4Prefab, mazeCell.position, Quaternion.identity, mazeCell);
        //ammoCase.transform.position.Set(ammoCase.transform.position.x + Random.Range(-5,5), ammoCase.transform.position.y, ammoCase.transform.position.z + Random.Range(-5,5));
        
        Vector3 currentLocalScale = ammoCase.transform.localScale;
            
        ammoCase.transform.localScale = new Vector3(
            currentLocalScale.x / mazeGenerator.GetScaleX(),
            currentLocalScale.y / mazeGenerator.GetScaleY(),
            currentLocalScale.z / mazeGenerator.GetScaleZ()
        );
        
        float randomXOffset = Random.Range(-0.34f, 0.34f);
        float randomZOffset = Random.Range(-0.34f, 0.34f);
        
        Vector3 currentLocalPosition = ammoCase.transform.localPosition;
        ammoCase.transform.localPosition = new Vector3(
            currentLocalPosition.x + randomXOffset,
            currentLocalPosition.y,
            currentLocalPosition.z + randomZOffset
        );
        
        float randomYRotation = Random.Range(-180f, 180f);
        ammoCase.transform.rotation = Quaternion.Euler(0f, randomYRotation, 0f);
    }
    
    private Transform GetRandomSpawnMazeCell()
    {
        int cell = Random.Range(0, 4);
        //Vector3 spawnPoint = Vector3.zero;
        if (cell == 0) // top left cell
        {
            var mazeCell = Extensions.GetRandomElementWithinDistance(mazeGenerator.GetMazeGrid(), 0,
                0, distFromSpawnPoint);
            return mazeCell.transform;
            //spawnPoint = mazeCell.transform;

        }
        if (cell == 1) // top right cell
        {
            var mazeCell = Extensions.GetRandomElementWithinDistance(mazeGenerator.GetMazeGrid(), 0,
                mazeGenerator.GetMazeDepth() - 1, distFromSpawnPoint);
            return mazeCell.transform;
            //spawnPoint = mazeCell.transform.position;
            
        }
        if (cell == 2) // bottom left cell
        {
            var mazeCell = Extensions.GetRandomElementWithinDistance(mazeGenerator.GetMazeGrid(), mazeGenerator.GetMazeWidth()-1,
                0, distFromSpawnPoint);
            return mazeCell.transform;
            //spawnPoint = mazeCell.transform.position;
            
        }
         if (cell == 3) // bottom right cell
        {
            var mazeCell = Extensions.GetRandomElementWithinDistance(mazeGenerator.GetMazeGrid(), mazeGenerator.GetMazeWidth()-1,
                mazeGenerator.GetMazeDepth() -1, distFromSpawnPoint);
            return mazeCell.transform;
            //spawnPoint = mazeCell.transform.position;
        }

        return null;
    }
    
}
