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

    [SerializeField] private int mazeWidth, mazeDepth;
    [SerializeField] private float scaleX, scaleY, scaleZ;
    
    [SerializeField] private int seed;

    [SerializeField] private bool useSeed;
    
    private MazeCell[,] mazeGrid;

    private List<List<int>> firstAidPositions;
    [SerializeField] private int firstAidCount = 5;
    
    private void OnDestroy()
    {
        EventManager.Instance.OnRequirementsBeforeNavMeshSpawned -= OnRequirementsBeforeNavMeshSpawned;
    }
    
    IEnumerator Start()
    {
        EventManager.Instance.OnRequirementsBeforeNavMeshSpawned += OnRequirementsBeforeNavMeshSpawned;
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
                break;
            case GameDifficulty.MODERATE:
                mazeWidth = 15;
                mazeDepth = 15;
                break;
            case GameDifficulty.HARD:
                mazeWidth = 22;
                mazeDepth = 22;
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

        firstAidPositions = GetRandomPositionsFrom2DArray(mazeWidth, mazeDepth, firstAidCount);
        ActivateFirstAidKits();
        
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

        //yield return new WaitForSeconds(0.05f);
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

    public Vector2Int GetTopRightCellCoordinates()
    {
        return new Vector2Int(0, mazeDepth - 1);
        //return mazeGrid[0, mazeDepth - 1];
    }
    
    public MazeCell GetBottomLeftCell()
    {
        return mazeGrid[mazeWidth - 1, 0];
    }
    
    public MazeCell GetBottomRightCell()
    {
        return mazeGrid[mazeWidth - 1, mazeDepth - 1];
    }

    void Update()
    {
        
    }
}
