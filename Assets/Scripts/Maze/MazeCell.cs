using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class MazeCell : MonoBehaviour
{
    [SerializeField] private GameObject leftWall;
    [SerializeField] private GameObject rightWall;
    [SerializeField] private GameObject frontWall;
    [SerializeField] private GameObject backWall;
    [SerializeField] private GameObject unvisitedBlock;
    

    [SerializeField] private GameObject[] firstAidArray;
    [SerializeField] private GameObject[] circuitBreakerArray;
    
    public List<int> activeWalls = new List<int>();

    [SerializeField] private ReflectionProbe reflectionProbe;
    [SerializeField] private GameObject spotLight;
    
    public bool IsVisited { get; private set; }

    public bool hasFirstAid;
    public bool hasCircuitBreaker;
    
    private void Start()
    {
        activeWalls.Add(0);
        activeWalls.Add(1);
        activeWalls.Add(2);
        activeWalls.Add(3);
        
        for (int i = 0; i < firstAidArray.Length; i++)
        {
            firstAidArray[i].SetActive(false);
        }

        EventManager.Instance.OnLightsTurnedOn += OnLightsTurnedOn;

    }

    private void OnDestroy()
    {
        EventManager.Instance.OnLightsTurnedOn -= OnLightsTurnedOn;
    }

    public void Visit()
    {
        IsVisited = true;
        unvisitedBlock.SetActive(false);
    }

    public GameObject GetLeftWall()
    {
        return leftWall;
    }

    public GameObject GetRightWall()
    {
        return rightWall;
    }

    public void ClearLeftWall()
    {
        leftWall.SetActive(false);
        activeWalls.Remove(0);
    }
    
    public void ClearRightWall()
    {
        rightWall.SetActive(false);
        activeWalls.Remove(1);
    }
    
    public void ClearFrontWall()
    {
        frontWall.SetActive(false);
        activeWalls.Remove(2);
    }
    
    public void ClearBackWall()
    {
        backWall.SetActive(false);
        activeWalls.Remove(3);
    }

    public void ActivateFirstAidRandomly()
    {
        int index = Random.Range(0, activeWalls.Count);
        firstAidArray[activeWalls[index]].SetActive(true);

        List<GameObject> firstAidsToDestroy = new List<GameObject>();

        for (int i = 0; i < firstAidArray.Length; i++)
        {
            if (i != activeWalls[index])
            {
                firstAidsToDestroy.Add(firstAidArray[i]);
            }
        }

        for (int i = 0; i < 3; i++)
        {
            var firstAidToDestroy = firstAidsToDestroy[0];
            firstAidsToDestroy.RemoveAt(0);
            Destroy(firstAidToDestroy);
        }

        hasFirstAid = true;
    }

    public void ActivateCircuitBreakerRandomly()
    {
        int index = Random.Range(0, activeWalls.Count);
        circuitBreakerArray[activeWalls[index]].SetActive(true);
        
        List<GameObject> circuitBreakersToDestroy = new List<GameObject>();
        
        for (int i = 0; i < circuitBreakerArray.Length; i++)
        {
            if (i != activeWalls[index])
            {
                circuitBreakersToDestroy.Add(circuitBreakerArray[i]);
            }
        }
        
        for (int i = 0; i < 3; i++)
        {
            var circuitBreakerToDestroy = circuitBreakersToDestroy[0];
            circuitBreakersToDestroy.RemoveAt(0);
            Destroy(circuitBreakerToDestroy);
        }

        hasCircuitBreaker = true;
    }

    public void DestroyAllFirstAidKits()
    {
        List<GameObject> firstAidsToDestroy = new List<GameObject>(firstAidArray);
        
        for (int i = 0; i < 4; i++)
        {
            var firstAidToDestroy = firstAidsToDestroy[0];
            firstAidsToDestroy.RemoveAt(0);
            Destroy(firstAidToDestroy);
        }
    }

    public void DestroyAllCircuitBreakers()
    {
        List<GameObject> circuitBreakersToDestroy = new List<GameObject>(circuitBreakerArray);
        
        for (int i = 0; i < 4; i++)
        {
            var circuitBreakerToDestroy = circuitBreakersToDestroy[0];
            circuitBreakersToDestroy.RemoveAt(0);
            Destroy(circuitBreakerToDestroy);
        }
    }

    private void OnLightsTurnedOn()
    {
        if (spotLight != null) Destroy(spotLight);
    }
    
    
}
