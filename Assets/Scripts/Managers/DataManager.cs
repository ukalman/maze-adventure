using UnityEngine;

public class DataManager : MonoBehaviour
{
    public static DataManager Instance { get; private set; }
    public GameDifficulty selectedDifficulty;

    public bool isEasyCompleted { get; private set; }
    public bool isModerateCompleted { get; private set; }
    
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
        isEasyCompleted = PlayerPrefs.GetInt("EasyCompleted", 0) == 1;
        isModerateCompleted = PlayerPrefs.GetInt("ModerateCompleted", 0) == 1;

        EventManager.Instance.OnDifficultySelected += OnDifficultySelected;
        EventManager.Instance.OnLevelCompleted += OnLevelCompleted;
    }

    private void OnDestroy()
    {
        EventManager.Instance.OnDifficultySelected -= OnDifficultySelected;
        EventManager.Instance.OnLevelCompleted -= OnLevelCompleted;
    }

    private void OnDifficultySelected(GameDifficulty difficulty)
    {
        selectedDifficulty = difficulty;
    }

    private void OnLevelCompleted(GameDifficulty difficulty)
    {
        if (difficulty == GameDifficulty.EASY)
        {
            isEasyCompleted = true;
            PlayerPrefs.SetInt("EasyCompleted", 1); 
        }
        else if (difficulty == GameDifficulty.MODERATE)
        {
            isModerateCompleted = true;
            PlayerPrefs.SetInt("ModerateCompleted", 1);
        }
        
        PlayerPrefs.Save();
    }
}