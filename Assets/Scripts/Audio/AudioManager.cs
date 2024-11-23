
using System;
using UnityEngine;
using UnityEngine.Serialization;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance { get; private set; }

    public float globalVolume = 1.0f;
    
    [Header("Audio Sources")]
    [SerializeField] private AudioSource musicAudioSource;
    [SerializeField] private AudioSource ambienceAudioSource;
    [SerializeField] private AudioSource footstepAudioSource;
    [SerializeField] private AudioSource interactionAudioSource;
    [SerializeField] private AudioSource lightAudioSource;
    
    [Header("Audio Clips")]
    [SerializeField] private AudioClip horrorAmbienceMusic;
    
    [SerializeField] private AudioClip footstepSound1;
    [SerializeField] private AudioClip footstepSound2;
    
    [SerializeField] private AudioClip ammoPickUpSound;
    [SerializeField] private AudioClip weaponPickUpSound;

    [SerializeField] private AudioClip flashlightSound;

    [SerializeField] private AudioClip activateDroneCamSound;
    [SerializeField] private AudioClip deactivateDroneCamSound;

    [SerializeField] private AudioClip droneAmbienceSound;

    [SerializeField] private AudioClip nexusVeinsSound;

    [SerializeField] private AudioClip facilityAlarmSound;
    
    [Header("Volumes")]
    [SerializeField] private float musicVolume = 0.5f;
    [SerializeField] private float ambienceVolume = 0.3f;
    [SerializeField] private float alarmVolume = 0.1f;

    private bool isDroneAmbiencePlaying;
    
    private void Awake()
    {
        /*
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject); 
        */
        if (Instance == null)
        {
            Instance = this;
        }
    }

    private void Start()
    {
        musicVolume = 1.0f;
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
        
    }

    public void PlayMusic(AudioClip audioClip, float volume)
    {
        musicAudioSource.clip = audioClip;
        musicAudioSource.volume = volume * globalVolume;
        musicAudioSource.Play();
        //musicAudioSource.PlayOneShot(audioClip, volume * globalVolume);
    }

    public void PlayAmbience(AudioClip audioClip, float volume )
    {
        ambienceAudioSource.clip = audioClip;
        ambienceAudioSource.volume = volume * globalVolume;
        ambienceAudioSource.Play();
    }
    
    public void PlayInteractionSound(AudioClip clip, float volume)
    {
        interactionAudioSource.PlayOneShot(clip, volume * globalVolume);
    }

    public void PlayFootstepSound(AudioClip clip, float volume)
    {
        footstepAudioSource.PlayOneShot(clip, volume * globalVolume);
    }

    public void PlayFlashlightSound()
    {
        lightAudioSource.PlayOneShot(flashlightSound, globalVolume);
    }
    
    // Animation Event
    public void OnFirstFootstep(float volume)
    {
        PlayFootstepSound(footstepSound1, volume);
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
        
        PlayInteractionSound(activateDroneCamSound, globalVolume);
        PlayAmbience(droneAmbienceSound, ambienceVolume);
    }

    private void OnDroneCamDeactivated()
    {
        PlayInteractionSound(deactivateDroneCamSound, globalVolume);
        ambienceAudioSource.Stop();
    }

    private void OnLightsTurnedOn()
    {
        PlayInteractionSound(nexusVeinsSound, globalVolume);
    }

    private void OnNexusCoreObtained()
    {
        PlayAmbience(facilityAlarmSound, alarmVolume);
    }

    private void OnLevelStarted()
    {
        isDroneAmbiencePlaying = false;
        PlayMusic(horrorAmbienceMusic, musicVolume);
    }

    public void OnSceneInitialized()
    {
        PlayAmbience(droneAmbienceSound, ambienceVolume);
        isDroneAmbiencePlaying = true;
    }
    
}
