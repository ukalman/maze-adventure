using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponManager : MonoBehaviour
{
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
    private AudioSource audioSource;

    private WeaponAmmo ammo;
    private WeaponBloom bloom;
    private WeaponRecoil recoil;
    
    private ActionStateManager actions;

    private Light muzzleFlashLight;
    private ParticleSystem muzzleFlashParticles;
    private float lightIntensity;
    [SerializeField] private float lightReturnSpeed = 20.0f;
    
    void Start()
    {
        recoil = GetComponent<WeaponRecoil>();
        audioSource = GetComponent<AudioSource>();
        aim = GetComponentInParent<AimStateManager>();
        ammo = GetComponent<WeaponAmmo>();
        bloom = GetComponent<WeaponBloom>();
        actions = GetComponentInParent<ActionStateManager>();
        muzzleFlashLight = GetComponentInChildren<Light>();
        lightIntensity = muzzleFlashLight.intensity;
        muzzleFlashLight.intensity = 0.0f;
        muzzleFlashParticles = GetComponentInChildren<ParticleSystem>();
        fireRateTimer = fireRate;
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
        for (int i = 0; i < bulletsPerShot; i++)
        {
            GameObject bulletGO = Instantiate(bullet, barrelPos.position, barrelPos.rotation);
            
            Bullet bulletScript = bulletGO.GetComponent<Bullet>();
            bulletScript.weapon = this;
            
            Rigidbody rb = bulletGO.GetComponent<Rigidbody>();
            rb.AddForce(barrelPos.forward * bulletVelocity, ForceMode.Impulse);
        }
    }

    private void PlayGunshotSound()
    {
        int randomIndex = Random.Range(0, gunShotSounds.Length);
        audioSource.PlayOneShot(gunShotSounds[randomIndex]);
    }

    private void TriggerMuzzleFlash()
    {
        muzzleFlashParticles.Play();
        muzzleFlashLight.intensity = lightIntensity;
    }
    
}
