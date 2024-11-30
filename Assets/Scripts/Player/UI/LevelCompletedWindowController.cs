using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class LevelCompletedWindowController : MonoBehaviour
{
    [SerializeField] private Image background;
    [SerializeField] private Image replayButtonBackground;
    [SerializeField] private Image nextLevelButtonBackground;
    [SerializeField] private Image mainMenuButtonBackground;
    [SerializeField] private Image finishGameButtonBackground;
    
    [SerializeField] private TMP_Text levelCompletedTitle; 
    [SerializeField] private TMP_Text difficultyText;
    [SerializeField] private TMP_Text totalTimeText;
    [SerializeField] private TMP_Text zombiesKilledText;
    [SerializeField] private TMP_Text replayButtonText;
    [SerializeField] private TMP_Text nextLevelButtonText;
    [SerializeField] private TMP_Text mainMenuButtonText;
    [SerializeField] private TMP_Text finishGameButtonText;
    
    
    [SerializeField] private Button replayButton;
    [SerializeField] private Button nextLevelButton;
    [SerializeField] private Button mainMenuButton;
    [SerializeField] private Button finishGameButton;

    public void SetPanel()
    {
        string formattedTotalTime = Extensions.FormatTime(GameManager.Instance.totalTime);
        totalTimeText.text = "TOTAL TIME: " + formattedTotalTime;

        string totalZombiesKilled = LevelManager.Instance.zombieKillCount.ToString();
        zombiesKilledText.text = "ZOMBIES KILLED: " + totalZombiesKilled;

        switch (LevelManager.Instance.GetGameDifficulty())
        {
            case GameDifficulty.EASY:
                difficultyText.text = "EASY";
                break;
            case GameDifficulty.MODERATE:
                difficultyText.text = "MODERATE";
                break;
            case GameDifficulty.HARD:
                difficultyText.text = "HARD";
                if (!DataManager.Instance.hasPlayerFinishedGame) finishGameButton.gameObject.SetActive(true);
                nextLevelButton.gameObject.SetActive(false);
                break;
        }
    }
    
    private void OnEnable()
    {
        SetPanel();
        StartCoroutine(FadeInUI());
    }
    
    private IEnumerator FadeInUI()
    {
        replayButton.interactable = false;
        nextLevelButton.interactable = false;
        mainMenuButton.interactable = false;
        finishGameButton.interactable = false;

        float fadeDuration = 3.0f;
        float elapsedTime = 0f;
        
        Color backgroundColor = background.color;
        Color replayButtonColor = replayButtonBackground.color;
        Color nextLevelButtonColor = nextLevelButtonBackground.color;
        Color mainMenuButtonColor = mainMenuButtonBackground.color;
        Color finishGameButtonColor = finishGameButtonBackground.color;
        Color titleColor = levelCompletedTitle.color;
        Color difficultyTextColor = difficultyText.color;
        Color totalTimeTextColor = totalTimeText.color;
        Color zombiesKilledTextColor = zombiesKilledText.color;
        Color replayTextColor = replayButtonText.color;
        Color nextLevelTextColor = nextLevelButtonText.color;
        Color mainMenuTextColor = mainMenuButtonText.color;
        Color finishGameTextColor = finishGameButtonText.color;
        
        backgroundColor.a = 0;
        replayButtonColor.a = 0;
        nextLevelButtonColor.a = 0;
        mainMenuButtonColor.a = 0;
        finishGameButtonColor.a = 0;
        titleColor.a = 0;
        difficultyTextColor.a = 0;
        totalTimeTextColor.a = 0;
        zombiesKilledTextColor.a = 0;
        replayTextColor.a = 0;
        nextLevelTextColor.a = 0;
        mainMenuTextColor.a = 0;
        finishGameTextColor.a = 0;
        
        background.color = backgroundColor;
        replayButtonBackground.color = replayButtonColor;
        nextLevelButtonBackground.color = nextLevelButtonColor;
        mainMenuButtonBackground.color = mainMenuButtonColor;
        finishGameButtonBackground.color = finishGameButtonColor;
        levelCompletedTitle.color = titleColor;
        difficultyText.color = difficultyTextColor;
        totalTimeText.color = totalTimeTextColor;
        zombiesKilledText.color = zombiesKilledTextColor;
        replayButtonText.color = replayTextColor;
        nextLevelButtonText.color = nextLevelTextColor;
        mainMenuButtonText.color = mainMenuTextColor;
        finishGameButtonText.color = finishGameTextColor;
        
        while (elapsedTime < fadeDuration)
        {
            elapsedTime += Time.deltaTime;
            float alpha = Mathf.Clamp01(elapsedTime / fadeDuration);

            backgroundColor.a = alpha;
            replayButtonColor.a = alpha;
            nextLevelButtonColor.a = alpha;
            mainMenuButtonColor.a = alpha;
            finishGameButtonColor.a = alpha;
            titleColor.a = alpha;
            difficultyTextColor.a = alpha;
            totalTimeTextColor.a = alpha;
            zombiesKilledTextColor.a = alpha;
            replayTextColor.a = alpha;
            nextLevelTextColor.a = alpha;
            mainMenuTextColor.a = alpha;
            finishGameTextColor.a = alpha;
            
            background.color = backgroundColor;
            replayButtonBackground.color = replayButtonColor;
            nextLevelButtonBackground.color = nextLevelButtonColor;
            mainMenuButtonBackground.color = mainMenuButtonColor;
            finishGameButtonBackground.color = finishGameButtonColor;
            levelCompletedTitle.color = titleColor;
            difficultyText.color = difficultyTextColor;
            totalTimeText.color = totalTimeTextColor;
            zombiesKilledText.color = zombiesKilledTextColor;
            replayButtonText.color = replayTextColor;
            nextLevelButtonText.color = nextLevelTextColor;
            mainMenuButtonText.color = mainMenuTextColor;
            finishGameButtonText.color = finishGameTextColor;

            yield return null;
        }
        
        backgroundColor.a = 1;
        replayButtonColor.a = 1;
        nextLevelButtonColor.a = 1;
        mainMenuButtonColor.a = 1;
        finishGameButtonColor.a = 1;
        titleColor.a = 1;
        difficultyTextColor.a = 1;
        totalTimeTextColor.a = 1;
        zombiesKilledTextColor.a = 1;
        replayTextColor.a = 1;
        nextLevelTextColor.a = 1;
        mainMenuTextColor.a = 1;
        finishGameTextColor.a = 1;
        
        background.color = backgroundColor;
        replayButtonBackground.color = replayButtonColor;
        nextLevelButtonBackground.color = nextLevelButtonColor;
        mainMenuButtonBackground.color = mainMenuButtonColor;
        finishGameButtonBackground.color = finishGameButtonColor;
        levelCompletedTitle.color = titleColor;
        difficultyText.color = difficultyTextColor;
        totalTimeText.color = totalTimeTextColor;
        zombiesKilledText.color = zombiesKilledTextColor;
        replayButtonText.color = replayTextColor;
        nextLevelButtonText.color = nextLevelTextColor;
        mainMenuButtonText.color = mainMenuTextColor;
        finishGameButtonText.color = finishGameTextColor;
        
        replayButton.interactable = true;
        nextLevelButton.interactable = true;
        mainMenuButton.interactable = true;
        finishGameButton.interactable = true;
        
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    public void OnReplayButtonClicked()
    {   
        SceneManager.Instance.ReloadCurrentScene();
    }

    public void OnNextLevelButtonClicked()
    {
        if (LevelManager.Instance.GetGameDifficulty() == GameDifficulty.EASY)
        {
            EventManager.Instance.InvokeOnDifficultySelected(GameDifficulty.MODERATE);
        }
        else if (LevelManager.Instance.GetGameDifficulty() == GameDifficulty.MODERATE)
        {
            EventManager.Instance.InvokeOnDifficultySelected(GameDifficulty.HARD);
        }
        
        SceneManager.Instance.LoadScene(2);
    }
    
    public void OnMainMenuClicked()
    {
        SceneManager.Instance.LoadScene(1);
    }

    public void OnFinishGameClicked()
    {
        if (!DataManager.Instance.hasPlayerFinishedGame) EventManager.Instance.InvokeOnGameFinished();
        SceneManager.Instance.LoadScene(3);
    }
}
