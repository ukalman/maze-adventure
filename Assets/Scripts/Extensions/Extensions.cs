
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
}
