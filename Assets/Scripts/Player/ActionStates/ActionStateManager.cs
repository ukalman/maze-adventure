using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Animations.Rigging;
using UnityEngine.Serialization;

public class ActionStateManager : MonoBehaviour
{
    [HideInInspector] public ActionBaseState CurrentState { get; private set; }

    public ReloadState Reload = new ReloadState();
    public DefaultState Default = new DefaultState();

    public GameObject currentWeapon;
    [HideInInspector] public WeaponAmmo ammo;
    private AudioSource audioSource;

    [HideInInspector] public Animator anim;
    public MultiAimConstraint rightHandAim;
    public TwoBoneIKConstraint leftHandIK;
    
    void Start()
    {
        SwitchState(Default);
        ammo = currentWeapon.GetComponent<WeaponAmmo>();
        audioSource = currentWeapon.GetComponent<AudioSource>();
        anim = GetComponent<Animator>();
    }
    
    void Update()
    {
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
}
