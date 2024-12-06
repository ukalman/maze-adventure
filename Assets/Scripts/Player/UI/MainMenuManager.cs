using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class MainMenuManager : MonoBehaviour
{
    [SerializeField] private Camera guiCam;

    [SerializeField] private GameObject mainMenuContainer;
    [SerializeField] private GameObject background;
    [SerializeField] private GameObject sceneModels;
    [SerializeField] private GameObject loadingScreen;

    [SerializeField] private AudioSource musicAudioSource;
    [SerializeField] private AudioSource sfxAudioSource;

    [SerializeField] private AudioClip mainMenuAmbience_1;
    [SerializeField] private AudioClip mainMenuAmbience_2;
    [SerializeField] private AudioClip brokenMainMenuMusic;

    [SerializeField] private AudioClip buttonClickSound;
    [SerializeField] private AudioClip readyButtonSound;

    [SerializeField] private AudioClip futuristicWhooshSound;

    [SerializeField] private Button creditsButton;
    [SerializeField] private Button easyButton;
    [SerializeField] private Button moderateButton;
    [SerializeField] private Button hardButton;

    [SerializeField] private Text creditsText;
    [SerializeField] private Text moderateText;
    [SerializeField] private Text hardText;

    [SerializeField] private Text easyRecordText;
    [SerializeField] private Text moderateRecordText;
    [SerializeField] private Text hardRecordText;

    [SerializeField] private Slider masterVolumeSlider;
    [SerializeField] private Slider musicVolumeSlider;
    [SerializeField] private Slider sfxVolumeSlider;
    [SerializeField] private Slider loadingSlider;
    
    void Start()
    {
        InitializeSliders(); 
        UpdateButtonInteractability();
        SetupBestTimes();
    }

    private void InitializeSliders()
    {
        SceneManager.Instance.loadingSlider = loadingSlider;
        masterVolumeSlider.value = DataManager.Instance.masterVolume;
        musicVolumeSlider.value = DataManager.Instance.musicVolume;
        sfxVolumeSlider.value = DataManager.Instance.sfxVolume;
        
        masterVolumeSlider.onValueChanged.AddListener(OnMasterVolumeChanged);
        musicVolumeSlider.onValueChanged.AddListener(OnMusicVolumeChanged);
        sfxVolumeSlider.onValueChanged.AddListener(OnSFXVolumeChanged);
    }
    
    private void UpdateButtonInteractability()
    {
        creditsButton.interactable = DataManager.Instance.hasPlayerFinishedGame;
        Color creditsTextColor = creditsText.color;
        float alphaCredits = DataManager.Instance.hasPlayerFinishedGame ? 1.0f : 0.05f;
        creditsTextColor.a = alphaCredits;
        creditsText.color = creditsTextColor;
        
        easyButton.interactable = true;
        
        moderateButton.interactable = DataManager.Instance.isEasyCompleted;
        Color moderateTextColor = moderateText.color;
        float alphaModerate = DataManager.Instance.isEasyCompleted ? 1.0f : 0.05f;
        moderateTextColor.a = alphaModerate;
        moderateText.color = moderateTextColor;
        
        hardButton.interactable = DataManager.Instance.isModerateCompleted;
        Color hardTextColor = hardText.color;
        float alphaHard = DataManager.Instance.isModerateCompleted ? 1.0f : 0.05f;
        hardTextColor.a = alphaHard;
        hardText.color = hardTextColor;
    }

    private void SetupBestTimes()
    {
        if (DataManager.Instance.bestTimeEasy < float.MaxValue)
        {
            easyRecordText.text = $"BEST TIME: {FormatTime(DataManager.Instance.bestTimeEasy)}";
        }
        else
        {
            easyRecordText.text = "NO RECORD YET";
        }

        if (DataManager.Instance.bestTimeModerate < float.MaxValue)
        {
            moderateRecordText.text = $"BEST TIME: {FormatTime(DataManager.Instance.bestTimeModerate)}";
        }
        else
        {
            moderateRecordText.text = "NO RECORD YET";
        }

        if (DataManager.Instance.bestTimeHard < float.MaxValue)
        {
            hardRecordText.text = $"BEST TIME: {FormatTime(DataManager.Instance.bestTimeHard)}";
        }
        else
        {
            hardRecordText.text = "NO RECORD YET";
        }

    }
    
    private string FormatTime(float time)
    {
        int minutes = Mathf.FloorToInt(time / 60);
        int seconds = Mathf.FloorToInt(time % 60);
        return $"{minutes}:{seconds:D2}";
    }

    public void OnCreditsClicked()
    {
        SceneManager.Instance.LoadScene(3);
    }

    public void OnLaunchClicked()
    {
        sfxAudioSource.PlayOneShot(readyButtonSound);
        StartCoroutine(FadeOutMusicVolume(3f, 0.1f));
        mainMenuContainer.SetActive(false);
        StartCoroutine(LoadLevelCoroutine());
    }

    private IEnumerator FadeOutMusicVolume(float duration, float targetVolume)
    {
        float startVolume = musicAudioSource.volume;
        float elapsedTime = 0f;

        while (elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;
            float newVolume = Mathf.Lerp(startVolume, targetVolume, elapsedTime / duration);
            musicAudioSource.volume = newVolume;
            yield return null; 
        }
        
        musicAudioSource.volume = targetVolume;
    }

    private IEnumerator LoadLevelCoroutine()
    {
        yield return StartCoroutine(CameraZoomCoroutine());
        StartCoroutine(SceneManager.Instance.LoadLevelAsync(2));
    }

    private IEnumerator CameraZoomCoroutine()
    {
        yield return new WaitForSeconds(1.0f);
        sfxAudioSource.PlayOneShot(futuristicWhooshSound);

        yield return new WaitForSeconds(0.3f);
        float timer = 0.0f;
        float whooshInterval = 0.3f;

        while (timer <= whooshInterval)
        {
            timer += Time.deltaTime;
            if (guiCam.fieldOfView >= 0.5f) guiCam.fieldOfView -= 0.4f;
            yield return null;
        }

        background.SetActive(false);
        Destroy(sceneModels);
        loadingScreen.SetActive(true);
    }

    public void PlayButtonSound()
    {
        sfxAudioSource.PlayOneShot(buttonClickSound);
    }
    
    private void OnMasterVolumeChanged(float value)
    {
        DataManager.Instance.masterVolume = value;
        DataManager.Instance.SaveVolumeSettings();
        ApplyMasterVolume(value); 
    }

    private void OnMusicVolumeChanged(float value)
    {
        DataManager.Instance.musicVolume = value;
        DataManager.Instance.SaveVolumeSettings();
        musicAudioSource.volume = value * 0.4f; 
    }

    private void OnSFXVolumeChanged(float value)
    {
        DataManager.Instance.sfxVolume = value;
        DataManager.Instance.SaveVolumeSettings();
        sfxAudioSource.volume = value; 
    }
    
    private void ApplyMasterVolume(float value)
    {
        musicAudioSource.volume = DataManager.Instance.musicVolume * value;
        sfxAudioSource.volume = DataManager.Instance.sfxVolume * value;
    }

    public void OnQuitClicked()
    {
        Application.Quit();
    }

    public void OnEasyClicked()
    {
        DataManager.Instance.OnDifficultySelected(GameDifficulty.EASY);
    }

    public void OnModerateClicked()
    {
        DataManager.Instance.OnDifficultySelected(GameDifficulty.MODERATE); 
    }

    public void OnHardClicked()
    {
        DataManager.Instance.OnDifficultySelected(GameDifficulty.HARD);
    }
    
}
