using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class WeaponManager : MonoBehaviour
{
    public string weaponName;
    public Sprite weaponSprite;
    
    [Header("Fire Rate")]
    [SerializeField] private float fireRate;
    [SerializeField] private bool semiAuto;
    private float fireRateTimer;

    [Header("Bullet Properties")] 
    [SerializeField] private GameObject bullet;
    [SerializeField] private Transform barrelPos;
    [SerializeField] private float bulletVelocity;
    [SerializeField] private int bulletsPerShot;
    public float damage = 20.0f;
    private AimStateManager aim;

    [SerializeField] private AudioClip[] gunShotSounds;
    [HideInInspector] public AudioSource audioSource;
    [HideInInspector] public WeaponAmmo ammo;
    private WeaponBloom bloom;
    private WeaponRecoil recoil;
    
    private ActionStateManager actions;

    private Light muzzleFlashLight;
    private ParticleSystem muzzleFlashParticles;
    private float lightIntensity;
    [SerializeField] private float lightReturnSpeed = 20.0f;

    public float enemyKickbackForce = 100.0f;

    public Transform leftHandTarget, leftHandHint;
    private WeaponClassManager weaponClass;

    private bool isPaused;

    [SerializeField] private GameObject flashLight;
    
    void Start()
    {
        aim = GetComponentInParent<AimStateManager>();
        bloom = GetComponent<WeaponBloom>();
        actions = GetComponentInParent<ActionStateManager>();
        muzzleFlashLight = GetComponentInChildren<Light>();
        lightIntensity = muzzleFlashLight.intensity;
        muzzleFlashLight.intensity = 0.0f;
        muzzleFlashParticles = GetComponentInChildren<ParticleSystem>();
        fireRateTimer = fireRate;

        audioSource.volume = 0f;
        audioSource.PlayOneShot(gunShotSounds[0]);
        audioSource.PlayOneShot(gunShotSounds[1]);
        
        EventManager.Instance.OnDroneCamActivated += OnDroneCamActivated;
        EventManager.Instance.OnDroneCamDeactivated += OnDroneCamDeactivated;
        EventManager.Instance.OnGamePaused += OnGamePaused;
        EventManager.Instance.OnGameContinued += OnGameContinued;
        
        EventManager.Instance.OnLightsTurnedOn += OnLightsTurnedOn;

        EventManager.Instance.OnVolumeChanged += ResetVolume;
        
        muzzleFlashParticles.Play();
        muzzleFlashParticles.Stop();
    }

    private void OnDestroy()
    {
        EventManager.Instance.OnDroneCamActivated -= OnDroneCamActivated;
        EventManager.Instance.OnDroneCamDeactivated -= OnDroneCamDeactivated;
        EventManager.Instance.OnGamePaused -= OnGamePaused;
        EventManager.Instance.OnGameContinued -= OnGameContinued;
        
        EventManager.Instance.OnLightsTurnedOn -= OnLightsTurnedOn;
        
        EventManager.Instance.OnVolumeChanged -= ResetVolume;
    }

    private void OnEnable()
    {
        if (weaponClass == null)
        {
            weaponClass = GetComponentInParent<WeaponClassManager>();
            ammo = GetComponent<WeaponAmmo>();
            audioSource = GetComponent<AudioSource>();
            recoil = GetComponent<WeaponRecoil>();
            recoil.recoilFollowPos = weaponClass.recoilFollowPosition;
        }
        weaponClass.SetCurrentWeapon(this);
    }

    void Update()
    {
        if (!LevelManager.Instance.HasLevelStarted) return;
        
        if (LevelManager.Instance.playerDied) return;
        
        if (LevelManager.Instance.LevelWon) return;
        
        if (isPaused) return;

        if (!LevelManager.Instance.VeinsActivated) CheckFlashlight();
        
        if (ShouldFire()) Fire();
        muzzleFlashLight.intensity = Mathf.Lerp(muzzleFlashLight.intensity, 0.0f, lightReturnSpeed * Time.deltaTime);
    }

    private bool ShouldFire()
    {
        fireRateTimer += Time.deltaTime;
        if (fireRateTimer < fireRate) return false;
        if (ammo.currentAmmo == 0) return false;
        if (actions.CurrentState == actions.Reload) return false;
        if (actions.CurrentState == actions.Swap) return false;
        if (semiAuto && Input.GetKeyDown(KeyCode.Mouse0)) return true;
        if (!semiAuto && Input.GetKey(KeyCode.Mouse0)) return true;
        return false;
    }

    private void Fire()
    {
        fireRateTimer = 0;
        barrelPos.LookAt(aim.actualAimPos);
        barrelPos.localEulerAngles = bloom.BloomAngle(barrelPos);
        PlayGunshotSound();
        recoil.TriggerRecoil();
        TriggerMuzzleFlash();
        ammo.currentAmmo--;
        EventManager.Instance.InvokeOnAmmoChanged();
        for (int i = 0; i < bulletsPerShot; i++)
        {
            GameObject bulletGO = Instantiate(bullet, barrelPos.position, barrelPos.rotation);
            
            Bullet bulletScript = bulletGO.GetComponent<Bullet>();
            bulletScript.weapon = this;
            bulletScript.dir = barrelPos.transform.forward;
            
            Rigidbody rb = bulletGO.GetComponent<Rigidbody>();
            rb.AddForce(barrelPos.forward * bulletVelocity, ForceMode.Impulse);
        }
        EventManager.Instance.InvokeOnPlayerFired();
    }

    private void PlayGunshotSound()
    {
        ResetVolume();
        int randomIndex = Random.Range(0, gunShotSounds.Length);
        //float volume = 0.7f;
        //audioSource.PlayOneShot(gunShotSounds[randomIndex], volume);
        audioSource.PlayOneShot(gunShotSounds[randomIndex]);
        
        AudioManager.Instance.OnWeaponFired();
    }

    private void ResetVolume()
    {
        if (audioSource != null && AudioManager.Instance != null) audioSource.volume = AudioManager.Instance.masterVolume * AudioManager.Instance.sfxVolume * 1.3f;
    }

    private void TriggerMuzzleFlash()
    {
        muzzleFlashParticles.Play();
        muzzleFlashLight.intensity = lightIntensity;
    }
    
    private void OnDroneCamActivated()
    {
        isPaused = true;
        audioSource.Pause();
    }

    private void OnDroneCamDeactivated()
    {
        isPaused = false;
        audioSource.UnPause();
    }

    private void OnGamePaused()
    {
        isPaused = true;
        audioSource.Pause();
    }

    private void OnGameContinued()
    {
        isPaused = false;
        audioSource.UnPause();
    }

    private void CheckFlashlight()
    {
        if (Input.GetKeyDown(KeyCode.F))
        {
            if (flashLight.activeSelf)
            {
                EventManager.Instance.InvokeOnFlashlightTurnedOff();
                flashLight.SetActive(false);
            }
            else
            {
                EventManager.Instance.InvokeOnFlashlightTurnedOn();
                flashLight.SetActive(true);
            }
        }
    }

    private void OnLightsTurnedOn()
    {
        if(flashLight.activeSelf) flashLight.SetActive(false);
    }
    
}
