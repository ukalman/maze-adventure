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

    public List<int> activeWalls = new List<int>();
    
    public bool IsVisited { get; private set; }

    public bool hasFirstAid;
    
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
        
    }

    public void Visit()
    {
        IsVisited = true;
        unvisitedBlock.SetActive(false);
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
        int index = Random.Range(0, activeWalls.Count - 1);
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
}
