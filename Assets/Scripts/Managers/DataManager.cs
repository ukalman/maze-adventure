using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DataManager : MonoBehaviour
{
    public static DataManager Instance { get; private set; }
    public GameDifficulty difficulty;
    
    private void Awake()
    {
        
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject); 
    }

    private void Start()
    {
        EventManager.Instance.OnDifficultySelected += OnDifficultySelected;
    }

    private void OnDestroy()
    {
        EventManager.Instance.OnDifficultySelected -= OnDifficultySelected;
    }

    private void OnDifficultySelected(GameDifficulty difficulty)
    {
        this.difficulty = difficulty;
    }
}
