
using System;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance { get; private set; }

    [SerializeField] private AudioSource musicAudioSource;
    [SerializeField] private AudioSource footstepAudioSource;
    [SerializeField] private AudioSource interactionAudioSource;
    
    [SerializeField] private AudioClip footstepSound1;
    [SerializeField] private AudioClip footstepSound2;
    
    [SerializeField] private AudioClip ammoPickUpSound;
    [SerializeField] private AudioClip weaponPickUpSound;
    
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        EventManager.Instance.OnAmmoCollected += OnAmmoCollected;
        EventManager.Instance.OnWeaponAcquired += OnWeaponAcquired;
    }

    private void OnDestroy()
    {
        EventManager.Instance.OnAmmoCollected -= OnAmmoCollected;
        EventManager.Instance.OnWeaponAcquired -= OnWeaponAcquired;
    }

    public void PlayInteractionSound(AudioClip clip, float volume)
    {
        interactionAudioSource.PlayOneShot(clip, volume);
        //interactionAudioSource.volume = 1.0f;
    }

    public void PlayFootstepSound(AudioClip clip, float volume)
    {
        footstepAudioSource.PlayOneShot(clip, volume);
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
}
