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
        if(ShouldFire()) Fire();
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
    }

    private void PlayGunshotSound()
    {
        int randomIndex = Random.Range(0, gunShotSounds.Length);
        float volume = 0.7f;
        audioSource.PlayOneShot(gunShotSounds[randomIndex], volume);
    }

    private void TriggerMuzzleFlash()
    {
        muzzleFlashParticles.Play();
        muzzleFlashLight.intensity = lightIntensity;
    }
    
}
