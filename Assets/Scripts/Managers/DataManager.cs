using UnityEngine;

public class DataManager : MonoBehaviour
{
    public static DataManager Instance { get; private set; }
    public GameDifficulty selectedDifficulty;

    public bool isEasyCompleted { get; private set; }
    public bool isModerateCompleted { get; private set; }
    public bool hasPlayerFinishedGame { get; private set; } // Tracks if the game is fully completed

    public float masterVolume = 1.0f, musicVolume = 1.0f, sfxVolume = 1.0f;

    public float bestTimeEasy { get; private set; }
    public float bestTimeModerate { get; private set; }
    public float bestTimeHard { get; private set; }

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);

        // Load volume settings
        masterVolume = PlayerPrefs.GetFloat("MasterVolume", 1.0f);
        musicVolume = PlayerPrefs.GetFloat("MusicVolume", 1.0f);
        sfxVolume = PlayerPrefs.GetFloat("SFXVolume", 1.0f);

        // Load best times
        bestTimeEasy = PlayerPrefs.GetFloat("BestTimeEasy", float.MaxValue);
        bestTimeModerate = PlayerPrefs.GetFloat("BestTimeModerate", float.MaxValue);
        bestTimeHard = PlayerPrefs.GetFloat("BestTimeHard", float.MaxValue);

        // Load game completion status
        hasPlayerFinishedGame = PlayerPrefs.GetInt("HasPlayerFinishedGame", 0) == 1;
    }

    private void Start()
    {
        isEasyCompleted = PlayerPrefs.GetInt("EasyCompleted", 0) == 1;
        isModerateCompleted = PlayerPrefs.GetInt("ModerateCompleted", 0) == 1;

        EventManager.Instance.OnDifficultySelected += OnDifficultySelected;
        EventManager.Instance.OnLevelCompleted += OnLevelCompleted;
        EventManager.Instance.OnGameFinished += OnGameFinished;
    }

    private void OnDestroy()
    {
        EventManager.Instance.OnDifficultySelected -= OnDifficultySelected;
        EventManager.Instance.OnLevelCompleted -= OnLevelCompleted;
        EventManager.Instance.OnGameFinished -= OnGameFinished;
    }

    private void OnDifficultySelected(GameDifficulty difficulty)
    {
        selectedDifficulty = difficulty;
    }

    private void OnLevelCompleted(GameDifficulty difficulty)
    {
        float totalTime = GameManager.Instance.totalTime;

        switch (difficulty)
        {
            case GameDifficulty.EASY:
                if (!isEasyCompleted)
                {
                    isEasyCompleted = true;
                    PlayerPrefs.SetInt("EasyCompleted", 1);
                }

                // Update best time for Easy
                if (totalTime < bestTimeEasy)
                {
                    bestTimeEasy = totalTime;
                    PlayerPrefs.SetFloat("BestTimeEasy", bestTimeEasy);
                }
                break;

            case GameDifficulty.MODERATE:
                if (!isModerateCompleted)
                {
                    isModerateCompleted = true;
                    PlayerPrefs.SetInt("ModerateCompleted", 1);
                }

                // Update best time for Moderate
                if (totalTime < bestTimeModerate)
                {
                    bestTimeModerate = totalTime;
                    PlayerPrefs.SetFloat("BestTimeModerate", bestTimeModerate);
                }
                break;

            case GameDifficulty.HARD:
                // Update best time for Hard
                if (totalTime < bestTimeHard)
                {
                    bestTimeHard = totalTime;
                    PlayerPrefs.SetFloat("BestTimeHard", bestTimeHard);
                }

               
                break;
        }

        PlayerPrefs.Save();
    }

    private void OnGameFinished()
    {
        if (!hasPlayerFinishedGame)
        {
            hasPlayerFinishedGame = true;
            PlayerPrefs.SetInt("HasPlayerFinishedGame", 1);
        }
        PlayerPrefs.Save();
    }
    
    public void SaveVolumeSettings()
    {
        PlayerPrefs.SetFloat("MasterVolume", masterVolume);
        PlayerPrefs.SetFloat("MusicVolume", musicVolume);
        PlayerPrefs.SetFloat("SFXVolume", sfxVolume);
        PlayerPrefs.Save();
    }
}
