using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class AgentNavigation : MonoBehaviour
{
    [SerializeField] private Vector3 desiredDestination;
    void Start()
    {
        //GetComponent<NavMeshAgent>().destination = desiredDestination;
        MazeGenerator.onMazeGenerated += SetDestination;
    }

    private void OnDestroy()
    {
        MazeGenerator.onMazeGenerated -= SetDestination;
    }

    private void SetDestination()
    {
        GetComponent<NavMeshAgent>().destination = desiredDestination;
    }
    
    void Update()
    {
        
    }
}
