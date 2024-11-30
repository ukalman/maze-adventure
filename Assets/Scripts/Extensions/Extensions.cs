
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Extensions
{
    // Method to get a random point within the NavMesh
    public static Vector3 GetRandomPointOnNavMesh(Vector3 origin, float distance)
    {
        Vector3 randomDirection = Random.insideUnitSphere * distance;
        randomDirection += origin;

        NavMeshHit hit;
        if (NavMesh.SamplePosition(randomDirection, out hit, distance, NavMesh.AllAreas))
        {
            return hit.position;
        }

        return Vector3.zero; 
    }
    
    public static T GetRandomElementWithinDistance<T>(T[,] array, int refX, int refY, int maxDistance)
    {
        int width = array.GetLength(0);
        int height = array.GetLength(1);
        List<(int x, int y)> validPositions = new List<(int x, int y)>();

        // Loop through possible positions within the maxDistance
        for (int x = Mathf.Max(0, refX - maxDistance); x <= Mathf.Min(width - 1, refX + maxDistance); x++)
        {
            for (int y = Mathf.Max(0, refY - maxDistance); y <= Mathf.Min(height - 1, refY + maxDistance); y++)
            {
                // Calculate Chebyshev distance
                int chebyshevDistance = Mathf.Max(Mathf.Abs(x - refX), Mathf.Abs(y - refY));

                if (chebyshevDistance <= maxDistance)
                {
                    validPositions.Add((x, y));
                }
            }
        }

        if (validPositions.Count == 0)
        {
            Debug.LogWarning("No valid positions found within the specified distance.");
            return default(T); 
        }

        // Select a random position from the list
        int randomIndex = Random.Range(0, validPositions.Count);
        int selectedX = validPositions[randomIndex].x;
        int selectedY = validPositions[randomIndex].y;
        
        return array[selectedX, selectedY];
    }
    
    public static Vector3 GetSpawnPointFarFromPlayer(MazeCell[,] mazeGrid, Vector3 playerPosition, float minDistanceFromPlayer)
    {
        int width = mazeGrid.GetLength(0);
        int height = mazeGrid.GetLength(1);
        List<Vector3> validSpawnPoints = new List<Vector3>();

        // Collect all valid MazeCell positions
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                Vector3 cellPosition = mazeGrid[x, y].transform.position;

                // Check if it's far enough from the player
                if (Vector3.Distance(cellPosition, playerPosition) >= minDistanceFromPlayer)
                {
                    validSpawnPoints.Add(cellPosition);
                }
            }
        }

        if (validSpawnPoints.Count == 0)
        {
            Debug.LogWarning("No valid spawn points found far enough from the player.");
            return Vector3.zero; 
        }
        
        return validSpawnPoints[Random.Range(0, validSpawnPoints.Count)];
    }

    
    public static string FormatTime(float totalTime)
    {
        int minutes = Mathf.FloorToInt(totalTime / 60); 
        int seconds = Mathf.FloorToInt(totalTime % 60); 
        int milliseconds = Mathf.FloorToInt((totalTime % 1) * 1000); 
        
        return $"{minutes}:{seconds:00}.{milliseconds:000}";
    }
    
    public static void SetLayerRecursively(GameObject obj, int newLayer)
    {
        obj.layer = newLayer;

        foreach (Transform child in obj.transform)
        {
            SetLayerRecursively(child.gameObject, newLayer);
        }
    }
}
