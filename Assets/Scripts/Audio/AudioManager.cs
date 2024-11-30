
using System.Collections;
using UnityEngine;


public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance { get; private set; }
    
    [Header("Audio Sources")]
    [SerializeField] private AudioSource musicAudioSource;
    [SerializeField] private AudioSource ambienceAudioSource;
    [SerializeField] private AudioSource footstepAudioSource;
    [SerializeField] private AudioSource interactionAudioSource;
    [SerializeField] private AudioSource lightAudioSource;
    
    [Header("Audio Clips")]
    [SerializeField] private AudioClip horrorAmbienceMusic;

    [SerializeField] private AudioClip easyTensionMusic;
    [SerializeField] private AudioClip moderateTensionMusic;
    [SerializeField] private AudioClip hardTensionMusic;
    
    [SerializeField] private AudioClip footstepSound1;
    [SerializeField] private AudioClip footstepSound2;
    
    [SerializeField] private AudioClip ammoPickUpSound;
    [SerializeField] private AudioClip weaponPickUpSound;

    [SerializeField] private AudioClip flashlightSound;

    [SerializeField] private AudioClip sprayPaintSound;

    [SerializeField] private AudioClip activateDroneCamSound;
    [SerializeField] private AudioClip deactivateDroneCamSound;

    [SerializeField] private AudioClip droneAmbienceSound;

    [SerializeField] private AudioClip nexusVeinsSound;

    [SerializeField] private AudioClip facilityAlarmSound;

    [SerializeField] private AudioClip healingSound;

    [SerializeField] private AudioClip buttonClickSound;

    [SerializeField] private AudioClip doorOpeningSound;

    [SerializeField] private AudioClip nexusPoweringDownSound;

    [SerializeField] private AudioClip[] punchSounds;
    [SerializeField] private AudioClip killerPunchSound;

    [SerializeField] private AudioClip[] bulletHitFleshSounds;
    [SerializeField] private AudioClip[] bulletHitMetalSounds;

    [SerializeField] private AudioClip bulletShellSound;
    
    public float masterVolume, musicVolume, sfxVolume;
    
    private bool isDroneAmbiencePlaying;

    private bool escapeMusicPlaying;

    private bool isPaused;
    
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
    }

    private void Start()
    {
        masterVolume = DataManager.Instance.masterVolume;
        musicVolume = DataManager.Instance.musicVolume;
        sfxVolume = DataManager.Instance.sfxVolume;
        ResetAudioSourceVolumes();
        
        OnSceneInitialized();
        
        EventManager.Instance.OnAmmoCollected += OnAmmoCollected;
        EventManager.Instance.OnWeaponAcquired += OnWeaponAcquired;
        
        EventManager.Instance.OnFlashlightTurnedOn += PlayFlashlightSound;
        EventManager.Instance.OnFlashlightTurnedOff += PlayFlashlightSound;

        EventManager.Instance.OnDroneCamActivated += OnDroneCamActivated;
        EventManager.Instance.OnDroneCamDeactivated += OnDroneCamDeactivated;

        EventManager.Instance.OnLightsTurnedOn += OnLightsTurnedOn;

        EventManager.Instance.OnNexusCoreObtained += OnNexusCoreObtained;
        
        EventManager.Instance.OnLevelStarted += OnLevelStarted;

        EventManager.Instance.OnGamePaused += OnGamePaused;
        EventManager.Instance.OnGameContinued += OnGameContinued;

        EventManager.Instance.OnPlayerDied += FadeOutMusic;
        EventManager.Instance.OnCountdownEnded += FadeOutMusic;
        EventManager.Instance.OnMazeExit += OnMazeExit;
    }

    private void ResetAudioSourceVolumes()
    {
        musicAudioSource.volume = masterVolume * musicVolume;
        ambienceAudioSource.volume = masterVolume * sfxVolume;
        footstepAudioSource.volume = masterVolume * sfxVolume;
        interactionAudioSource.volume = masterVolume * sfxVolume;
        lightAudioSource.volume = masterVolume * sfxVolume;
    }

    private void OnDestroy()
    {
        EventManager.Instance.OnAmmoCollected -= OnAmmoCollected;
        EventManager.Instance.OnWeaponAcquired -= OnWeaponAcquired;
        
        EventManager.Instance.OnFlashlightTurnedOn -= PlayFlashlightSound;
        EventManager.Instance.OnFlashlightTurnedOff -= PlayFlashlightSound;
        
        EventManager.Instance.OnDroneCamActivated -= OnDroneCamActivated;
        EventManager.Instance.OnDroneCamDeactivated -= OnDroneCamDeactivated;
        
        EventManager.Instance.OnLightsTurnedOn -= OnLightsTurnedOn;
        
        EventManager.Instance.OnNexusCoreObtained -= OnNexusCoreObtained;

        EventManager.Instance.OnLevelStarted -= OnLevelStarted;
        
        EventManager.Instance.OnGamePaused -= OnGamePaused;
        EventManager.Instance.OnGameContinued -= OnGameContinued;
        
        EventManager.Instance.OnPlayerDied += FadeOutMusic;
        EventManager.Instance.OnCountdownEnded += FadeOutMusic;
        EventManager.Instance.OnMazeExit += OnMazeExit;
    }

    private void PlayMusic(AudioClip audioClip, float volume)
    {
        musicAudioSource.clip = audioClip;
        musicAudioSource.volume = volume * masterVolume * musicVolume;
        musicAudioSource.Play();
    }

    private void PlayAmbience(AudioClip audioClip, float volume)
    {
        ambienceAudioSource.clip = audioClip;
        ambienceAudioSource.volume = volume * masterVolume * sfxVolume;
        ambienceAudioSource.Play();
    }
    
    private void PlayInteractionSound(AudioClip clip, float volume)
    {
        interactionAudioSource.PlayOneShot(clip, volume * masterVolume * sfxVolume);
    }

    private void PlayFootstepSound(AudioClip clip, float volume)
    {
        footstepAudioSource.PlayOneShot(clip, volume * masterVolume * sfxVolume);
    }

    private void PlayFlashlightSound()
    {
        lightAudioSource.PlayOneShot(flashlightSound, 1.0f);
    }
    
    // Animation Event
    public void OnFirstFootstep(float volume)
    {
        PlayFootstepSound(footstepSound1, volume * 0.5f);
    }

    // Animation Event
    public void OnSecondFootstep(float volume)
    {
        PlayFootstepSound(footstepSound2, volume);
    }

    private void OnWeaponAcquired(WeaponName weaponName)
    {
        float volume = 0.4f;
        PlayInteractionSound(weaponPickUpSound, volume);
    }

    private void OnAmmoCollected(AmmoType ammoType, int amount)
    {
        float volume = 0.5f;
        PlayInteractionSound(ammoPickUpSound, volume);
    }

    private void OnDroneCamActivated()
    {
        if (isDroneAmbiencePlaying) return;
        
        PlayInteractionSound(activateDroneCamSound, 1.0f);
        PlayAmbience(droneAmbienceSound, 0.5f);
    }

    private void OnDroneCamDeactivated()
    {
        ambienceAudioSource.volume = masterVolume * sfxVolume;
        PlayInteractionSound(deactivateDroneCamSound, 1.0f);
        ambienceAudioSource.Stop();
    }

    private void OnLightsTurnedOn()
    {
        PlayInteractionSound(nexusVeinsSound, 1.0f);
    }

    private void OnNexusCoreObtained()
    {
        PlayAmbience(facilityAlarmSound, 1.0f);
        musicAudioSource.loop = false;
        escapeMusicPlaying = true;
        switch (LevelManager.Instance.GetGameDifficulty())
        {
            case GameDifficulty.EASY:
                StartCoroutine(SmoothTransition(easyTensionMusic, 1.0f));
                //PlayMusic(easyTensionMusic, 1.0f);
                break;
            case GameDifficulty.MODERATE:
                StartCoroutine(SmoothTransition(moderateTensionMusic, 1.0f));
                //PlayMusic(moderateTensionMusic, 1.0f);
                break;
            case GameDifficulty.HARD:
                StartCoroutine(SmoothTransition(hardTensionMusic, 1.0f));
                //PlayMusic(hardTensionMusic, 1.0f);
                break;
        }
    }

    private void OnLevelStarted()
    {
        EventManager.Instance.InvokeOnVolumeChanged();
        isDroneAmbiencePlaying = false;
        PlayMusic(horrorAmbienceMusic, 1.0f);
    }
    
    private void WarmUpTrack(AudioClip track)
    {
        AudioSource tempSource = gameObject.AddComponent<AudioSource>();
        tempSource.clip = track;
        tempSource.volume = 0f; 
        tempSource.Play();
        tempSource.Stop();
        Destroy(tempSource); 
    }

    private void OnSceneInitialized()
    {
        WarmUpTrack(horrorAmbienceMusic);
        switch (LevelManager.Instance.GetGameDifficulty())
        {
            case GameDifficulty.EASY:
                WarmUpTrack(easyTensionMusic);
                break;
            case GameDifficulty.MODERATE:
                WarmUpTrack(moderateTensionMusic);
                break;
            case GameDifficulty.HARD:
                WarmUpTrack(hardTensionMusic);
                break;
        }
        
        PlayAmbience(droneAmbienceSound, 0.5f);
        isDroneAmbiencePlaying = true;
    }

    private IEnumerator SmoothTransition(AudioClip newClip, float targetVolume)
    {
 
        if (musicAudioSource.isPlaying)
        {
            float initialVolume = musicAudioSource.volume;
            float fadeDuration = 1.0f; 
            float elapsedTime = 0f;

            while (elapsedTime < fadeDuration)
            {
                elapsedTime += Time.deltaTime;
                musicAudioSource.volume = Mathf.Lerp(initialVolume, 0f, elapsedTime / fadeDuration);
                yield return null;
            }

            musicAudioSource.Stop(); 
        }

        musicAudioSource.clip = newClip;
        musicAudioSource.volume = 0f;
        musicAudioSource.Play();
        
        float fadeInDuration = 1.0f;
        float elapsedFadeIn = 0f;

        while (elapsedFadeIn < fadeInDuration)
        {
            elapsedFadeIn += Time.deltaTime;
            musicAudioSource.volume = Mathf.Lerp(0f, targetVolume * masterVolume * musicVolume, elapsedFadeIn / fadeInDuration);
            yield return null;
        }

        musicAudioSource.volume = targetVolume * masterVolume * musicVolume; 
    }
    
    public void OnSprayPaintUsed()
    {
        PlayInteractionSound(sprayPaintSound, 1.0f);
    }

    public void OnHealed()
    {
        PlayInteractionSound(healingSound, 1.0f);
    }

    public void OnNexusCoreRemoving()
    {
        PlayInteractionSound(nexusPoweringDownSound, 1.0f);
    }

    public IEnumerator OnCoreRemovalCancelled()
    {
        float startVolume = interactionAudioSource.volume;
        yield return FadeOutAudioSourceVolume(interactionAudioSource, 1.0f, 0.0f);
        interactionAudioSource.Stop();
        interactionAudioSource.volume = startVolume;
    }

    public void OnMasterVolumeChanged(float value)
    {
        masterVolume = value;
        ResetAudioSourceVolumes();
    }

    public void OnMusicVolumeChanged(float value)
    {
        musicVolume = value;
        ResetAudioSourceVolumes();
    }

    public void OnSFXVolumeChanged(float value)
    {
        sfxVolume = value;
        ResetAudioSourceVolumes();
    }

    private void OnGamePaused()
    {
        isPaused = true;
        ambienceAudioSource.Pause();
        interactionAudioSource.Pause();
        footstepAudioSource.Pause();
        
        if (!escapeMusicPlaying) return;
        musicAudioSource.Pause();
    }
    
    public void PlayButtonSound()
    {
        interactionAudioSource.PlayOneShot(buttonClickSound);
    }

    private void OnGameContinued()
    {
        isPaused = false;
        ambienceAudioSource.UnPause();
        interactionAudioSource.UnPause();
        footstepAudioSource.UnPause();
        
        if (!escapeMusicPlaying) return;
        musicAudioSource.UnPause();
    }

    private void FadeOutMusic()
    {
        StartCoroutine(FadeOutAudioSourceVolume(musicAudioSource,3.0f, 0.0f));
    }
    
    private IEnumerator FadeOutAudioSourceVolume(AudioSource audioSource, float duration, float targetVolume)
    {
        float startVolume = audioSource.volume;
        float elapsedTime = 0f;

        while (elapsedTime < duration)
        {
            if (isPaused) yield return null;
            else
            {
                elapsedTime += Time.deltaTime;
                float newVolume = Mathf.Lerp(startVolume, targetVolume, elapsedTime / duration);
                audioSource.volume = newVolume;
                yield return null;  
            }
            
        }
        
        audioSource.volume = targetVolume;
    }
    
    private void OnMazeExit()
    {
        PlayInteractionSound(doorOpeningSound, 1.0f);
        FadeOutMusic();
        StartCoroutine(FadeOutAudioSourceVolume(ambienceAudioSource, 2.0f, 0.0f));
    }

    public void OnDamageTaken()
    {
        PlayInteractionSound(punchSounds[Random.Range(0,2)], 1.0f);
    }

    public void OnKilled()
    {
        PlayInteractionSound(killerPunchSound, 1.0f);
    }

    public void OnBulletHitFlesh()
    {
        PlayInteractionSound(bulletHitFleshSounds[Random.Range(0,2)], 0.4f);
    }

    public void OnBulletHitMetal()
    {
        PlayInteractionSound(bulletHitMetalSounds[Random.Range(0,2)], 0.3f);
    }

    public void OnWeaponFired()
    {
        PlayInteractionSound(bulletShellSound, 0.8f);
    }
    
}
