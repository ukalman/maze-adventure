using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Unity.AI.Navigation;
using Random = UnityEngine.Random;

public class MazeGenerator : MonoBehaviour
{
    /* If difficulty:
     * EASY -> 10,10
     * MODERATE -> 15,15
     * HARD -> 22,22
     */
    
    [SerializeField] private MazeCell mazeCellPrefab;
    [SerializeField] private GameObject mazeEntranceDoor;
    [SerializeField] private GameObject mazeExitDoor;

    [SerializeField] private int mazeWidth, mazeDepth;
    [SerializeField] private float scaleX, scaleY, scaleZ;
    
    [SerializeField] private int seed;

    [SerializeField] private bool useSeed;

    [SerializeField] private Material wallMat;
    [SerializeField] private Material floorMat;
    [SerializeField] private Material doorMat;
    [SerializeField] private Material ammoCaseAK47Mat;
    [SerializeField] private Material ammoCaseM4Mat;
    [SerializeField] private Material firstAidMat;
    [SerializeField] private Material circuitBreakerMat;
    [SerializeField] private Material tableMat;
    
    private MazeCell[,] mazeGrid;

    private List<List<int>> firstAidPositions;
    private List<List<int>> circuitBreakerPositions;
    
    [SerializeField] private int firstAidCount = 5;
    private int circuitBreakerCount;

    private float waitingTime;

    [SerializeField] private int distFromSpawnOrigin;
    [SerializeField] private GameObject nexusCorePrefab;
    
    private void OnDestroy()
    {
        EventManager.Instance.OnRequirementsBeforeNavMeshSpawned -= OnRequirementsBeforeNavMeshSpawned;
        EventManager.Instance.OnLightsTurnedOn -= OnLightsTurnedOn;

        wallMat.SetFloat("_Metallic",1.0f);
        floorMat.SetFloat("_Metallic",1.0f);
        doorMat.SetFloat("_Metallic", 1.0f);
        ammoCaseAK47Mat.SetFloat("_Metallic",1.0f);
        ammoCaseM4Mat.SetFloat("_Metallic",1.0f);
        circuitBreakerMat.SetFloat("_Metallic", 1.0f);
        tableMat.SetFloat("_Metallic", 1.0f);
        firstAidMat.SetFloat("_Metallic",1.0f);
    }
    
    IEnumerator Start()
    {
        EventManager.Instance.OnRequirementsBeforeNavMeshSpawned += OnRequirementsBeforeNavMeshSpawned;
        EventManager.Instance.OnLightsTurnedOn += OnLightsTurnedOn;
        if (useSeed)
        {
            Random.InitState(seed);
        } 
        else
        {
            int randomSeed = Random.Range(1, 1000000);
            Random.InitState(randomSeed);
            Debug.Log(randomSeed);
        }

        switch (LevelManager.Instance.GetGameDifficulty())
        {
            case GameDifficulty.EASY:
                mazeWidth = 10;
                mazeDepth = 10;
                circuitBreakerCount = 1;
                distFromSpawnOrigin = 5;
                waitingTime = 0.05f;
                break;
            case GameDifficulty.MODERATE:
                mazeWidth = 13;
                mazeDepth = 13;
                circuitBreakerCount = 2;
                distFromSpawnOrigin = 8;
                waitingTime = 0.005f;
                break;
            case GameDifficulty.HARD:
                mazeWidth = 16;
                mazeDepth = 16;
                circuitBreakerCount = 2;
                distFromSpawnOrigin = 12;
                waitingTime = 0.0005f;
                break;
        }
        
        mazeGrid = new MazeCell[mazeWidth, mazeDepth];

        for (int x = 0; x < mazeWidth; x++)
        {
            for (int z = 0; z < mazeDepth; z++)
            {
                mazeGrid[x,z] = Instantiate(mazeCellPrefab, new Vector3(x, 0.0f, z), Quaternion.identity, transform);
                mazeGrid[x, z].transform.localPosition = new Vector3(x, 0.0f, z);
            }
        }

        yield return GenerateMaze(null, mazeGrid[0,0]);

        PlaceDoors();
        firstAidPositions = GetRandomPositionsFrom2DArray(mazeWidth, mazeDepth, firstAidCount);
        circuitBreakerPositions = GetRandomPositionsFrom2DArray(mazeWidth, mazeDepth, circuitBreakerCount,true);
        ActivateFirstAidKits();
        PlaceCircuitBreakers();
        PlaceNexusCore();
        
        //GetComponent<NavMeshSurface>().BuildNavMesh();
        EventManager.Instance.InvokeOnMazeGenerated();
    }

    private void OnRequirementsBeforeNavMeshSpawned()
    {
        GetComponent<NavMeshSurface>().BuildNavMesh();
        //EventManager.Instance.InvokeOnMazeGenerated();
        EventManager.Instance.InvokeOnNavMeshBaked();
    }
    
    private IEnumerator GenerateMaze(MazeCell previousCell, MazeCell currentCell)
    {
        currentCell.Visit();
        ClearWalls(previousCell, currentCell);

        yield return new WaitForSeconds(waitingTime);
        MazeCell nextCell;

        do
        {
            nextCell = GetNextUnvisitedCell(currentCell);

            if (nextCell != null)
            {
                yield return GenerateMaze(currentCell, nextCell);
            }
        } while (nextCell != null);
        
        
    }

    private MazeCell GetNextUnvisitedCell(MazeCell currentCell)
    {
        var unvisitedCells = GetUnvisitedCells(currentCell);

        return unvisitedCells.OrderBy(_ => Random.Range(1, 10)).FirstOrDefault();
    }

    private IEnumerable<MazeCell> GetUnvisitedCells(MazeCell currentCell)
    {
        int x = (int)currentCell.transform.localPosition.x;
        int z = (int)currentCell.transform.localPosition.z;

        if (x + 1 < mazeWidth)
        {
            var cellToRight = mazeGrid[x + 1, z];

            if (!cellToRight.IsVisited)
            {
                yield return cellToRight;
            }
        }

        if (x - 1 >= 0)
        {
            var cellToLeft = mazeGrid[x - 1, z];

            if (!cellToLeft.IsVisited)
            {
                yield return cellToLeft;
            }
        }

        if (z + 1 < mazeDepth)
        {
            var cellToFront = mazeGrid[x, z + 1];

            if (!cellToFront.IsVisited)
            {
                yield return cellToFront;
            }
        }
        
        if (z - 1 >= 0)
        {
            var cellToBack = mazeGrid[x, z -1];

            if (!cellToBack.IsVisited)
            {
                yield return cellToBack;
            }
        }
    }

    private void ClearWalls(MazeCell previousCell, MazeCell currentCell)
    {
        if (previousCell == null)
        {
            return;
        }

        if (previousCell.transform.localPosition.x < currentCell.transform.localPosition.x)
        {
            previousCell.ClearRightWall();
            currentCell.ClearLeftWall();
            return;
        }

        if (previousCell.transform.localPosition.x > currentCell.transform.localPosition.x)
        {
            previousCell.ClearLeftWall();
            currentCell.ClearRightWall();
            return;
        }

        if (previousCell.transform.localPosition.z < currentCell.transform.localPosition.z)
        {
            previousCell.ClearFrontWall();
            currentCell.ClearBackWall();
            return;
        }

        if (previousCell.transform.localPosition.z > currentCell.transform.localPosition.z)
        {
            previousCell.ClearBackWall();
            currentCell.ClearFrontWall();
            return;
        }
    }

    private void PlaceDoors()
    {
        var entrance = Instantiate(mazeEntranceDoor, mazeGrid[0, 0].GetLeftWall().transform.position, Quaternion.identity, mazeGrid[0, 0].GetLeftWall().transform);
        
        Vector3 entranceLocalScale = entrance.transform.localScale;
        entrance.transform.localScale = new Vector3(
            entranceLocalScale.x / GetScaleX(),
            entranceLocalScale.y / GetScaleY(),
            entranceLocalScale.z / GetScaleZ()
        );
        
        entrance.transform.rotation = Quaternion.Euler(0.0f, -90.0f, 0.0f);

        float xOffset = 0.1113f;
        float yOffset = -0.494f;

        entrance.transform.localPosition = new Vector3(xOffset, yOffset, 0.0f);
        
        var exit = Instantiate(mazeExitDoor, mazeGrid[mazeWidth-1, mazeDepth-1].GetRightWall().transform.position, Quaternion.identity, mazeGrid[mazeWidth -1, mazeDepth-1].GetRightWall().transform);
        
        Vector3 exitLocalScale = exit.transform.localScale;
        exit.transform.localScale = new Vector3(
            exitLocalScale.x / GetScaleX(),
            exitLocalScale.y / GetScaleY(),
            exitLocalScale.z / GetScaleZ()
        );

        xOffset = 0.3403f;
        exit.transform.rotation = Quaternion.Euler(0.0f, 90.0f, 0.0f);

        exit.transform.localPosition = new Vector3(xOffset, yOffset, 0.0f);

    }

    private void ActivateFirstAidKits()
    {
        for (int i = 0; i < firstAidCount; i++)
        {
            mazeGrid[firstAidPositions[i][0],firstAidPositions[i][1]].ActivateFirstAidRandomly();
        }
        
        for (int x = 0; x < mazeWidth; x++)
        {
            for (int z = 0; z < mazeDepth; z++)
            {
                if (mazeGrid[x,z].hasFirstAid) continue;
                mazeGrid[x,z].DestroyAllFirstAidKits();
            }
        }
    }

    private void PlaceCircuitBreakers()
    {
        for (int i = 0; i < circuitBreakerCount; i++)
        {
            mazeGrid[circuitBreakerPositions[i][0],circuitBreakerPositions[i][1]].ActivateCircuitBreakerRandomly();
        }
        
        for (int x = 0; x < mazeWidth; x++)
        {
            for (int z = 0; z < mazeDepth; z++)
            {
                if (mazeGrid[x,z].hasCircuitBreaker) continue;
                mazeGrid[x,z].DestroyAllCircuitBreakers();
            }
        }
    }

    private void PlaceNexusCore()
    {
        // top right cell
        var mazeCell = Extensions.GetRandomElementWithinDistance(GetMazeGrid(), 0,
            GetMazeDepth() - 1, distFromSpawnOrigin);
        
        var nexusCore = Instantiate(nexusCorePrefab, mazeCell.transform.position, Quaternion.identity, mazeCell.transform);
            
        Vector3 currentLocalScale = nexusCore.transform.localScale;
            
        nexusCore.transform.localScale = new Vector3(
            currentLocalScale.x / GetScaleX(),
            currentLocalScale.y / GetScaleY(),
            currentLocalScale.z / GetScaleZ()
        );
        
    }

    private void OnLightsTurnedOn()
    {
        wallMat.SetFloat("_Metallic",0.0f);
        //wallMat.SetFloat("_Smoothness",1.0f);
        floorMat.SetFloat("_Metallic",0.0f);
        //floorMat.SetFloat("_Smoothness",1.0f);
        doorMat.SetFloat("_Metallic", 0.0f);
        ammoCaseAK47Mat.SetFloat("_Metallic",0.0f);
        ammoCaseM4Mat.SetFloat("_Metallic",0.0f);
        circuitBreakerMat.SetFloat("_Metallic", 0.0f);
        tableMat.SetFloat("_Metallic", 0.0f);
        firstAidMat.SetFloat("_Metallic",0.0f);
    }
    
    List<List<int>> GetRandomPositionsFrom2DArray(int width, int depth, int count)
    {
        List<List<int>> selectedPositions = new List<List<int>>();
        HashSet<(int, int)> usedPositions = new HashSet<(int, int)>();
        
        while (selectedPositions.Count < count)
        {
            int x = Random.Range(0, width);
            int y = Random.Range(0, depth);
            
            if (!usedPositions.Contains((x, y)))
            {
                usedPositions.Add((x, y));
                selectedPositions.Add(new List<int> { x, y });
            }
        }

        return selectedPositions;
    }
    
    List<List<int>> GetRandomPositionsFrom2DArray(int width, int depth, int count, bool forCircuitBreaker)
    {
        List<List<int>> selectedPositions = new List<List<int>>();
        HashSet<(int, int)> usedPositions = new HashSet<(int, int)>();
        
        while (selectedPositions.Count < count)
        {
            int x = Random.Range(0, width);
            int y = Random.Range(0, depth);
            
            if (!usedPositions.Contains((x, y)) && !mazeGrid[x,y].hasFirstAid)
            {
                usedPositions.Add((x, y));
                selectedPositions.Add(new List<int> { x, y });
            }
        }

        return selectedPositions;
    }

    public MazeCell[,] GetMazeGrid()
    {
        return mazeGrid;
    }
    
    public int GetMazeWidth()
    {
        return mazeWidth;
    }

    public int GetMazeDepth()
    {
        return mazeDepth;
    }

    public float GetScaleX()
    {
        return scaleX;
    }
    
    public float GetScaleY()
    {
        return scaleY;
    }
    
    public float GetScaleZ()
    {
        return scaleZ;
    }

    public int GetGreaterEdge()
    {
        return mazeWidth >= mazeDepth ? mazeWidth : mazeDepth;
    }
    
}
