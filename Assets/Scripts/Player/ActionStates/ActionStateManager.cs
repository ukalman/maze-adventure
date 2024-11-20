using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Animations.Rigging;
using UnityEngine.Serialization;

public class ActionStateManager : MonoBehaviour
{
    public ActionBaseState CurrentState { get; private set; }
    public string currentStateName;
    
    public ReloadState Reload = new ReloadState();
    public DefaultState Default = new DefaultState();
    public SwapState Swap = new SwapState();

    [HideInInspector] public WeaponManager currentWeapon;
    [HideInInspector] public WeaponAmmo ammo;
    private AudioSource audioSource;

    [HideInInspector] public Animator anim;
    public MultiAimConstraint rightHandAim;
    public TwoBoneIKConstraint leftHandIK;

    public WeaponClassManager weaponClassManager;

    private bool isPaused;
    
    void Start()
    {
        SwitchState(Default);
        anim = GetComponent<Animator>();
        weaponClassManager = GetComponent<WeaponClassManager>();
        
        EventManager.Instance.OnDroneCamActivated += OnDroneCamActivated;
        EventManager.Instance.OnDroneCamDeactivated += OnDroneCamDeactivated;
        EventManager.Instance.OnGamePaused += OnGamePaused;
        EventManager.Instance.OnGameContinued += OnGameContinued;
    }

    private void OnDestroy()
    {
        EventManager.Instance.OnDroneCamActivated -= OnDroneCamActivated;
        EventManager.Instance.OnDroneCamDeactivated -= OnDroneCamDeactivated;
        EventManager.Instance.OnGamePaused -= OnGamePaused;
        EventManager.Instance.OnGameContinued -= OnGameContinued;
    }


    void Update()
    {
        if (!LevelManager.Instance.HasLevelStarted) return;
        
        if (isPaused) return;
        
        CurrentState.UpdateState(this);
    }

    public void SwitchState(ActionBaseState stateToSwitch)
    {
        CurrentState = stateToSwitch;
        CurrentState.EnterState(this);
    }

    public void WeaponReloaded()
    {
        ammo.Reload();
        SwitchState(Default);
    }

    public void MagOut()
    {   
        audioSource.PlayOneShot(ammo.magOutSound);
    }

    public void MagIn()
    {
        audioSource.PlayOneShot(ammo.magInSound);
    }

    public void ReleaseSlide()
    {
        audioSource.PlayOneShot(ammo.releaseSlideSound);
    }

    public void SetWeapon(WeaponManager weapon)
    {
        currentWeapon = weapon;
        audioSource = weapon.audioSource;
        ammo = weapon.ammo;
    }
    
    private void OnDroneCamActivated()
    {
        isPaused = true;
        anim.speed = 0;
    }

    private void OnDroneCamDeactivated()
    {
        isPaused = false;
        anim.speed = 1;
    }

    private void OnGamePaused()
    {
        isPaused = true;
        anim.speed = 0;
    }

    private void OnGameContinued()
    {
        isPaused = false;
        anim.speed = 1;
    }
}
