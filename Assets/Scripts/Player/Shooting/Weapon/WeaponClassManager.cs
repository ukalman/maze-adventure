using System;
using UnityEngine;
using UnityEngine.Animations.Rigging;

public class WeaponClassManager : MonoBehaviour
{
    [SerializeField] private TwoBoneIKConstraint leftHandIK;
    public Transform recoilFollowPosition;
    private ActionStateManager actions;

    public WeaponManager[] weapons;
    public int currentWeaponIndex;

    private void Awake()
    {
        currentWeaponIndex = 0;
        for (int i = 0; i < weapons.Length; i++)
        {
            weapons[i].gameObject.SetActive(i == 0);
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

    public void SetCurrentWeapon(WeaponManager weapon)
    {
        if (actions == null) actions = GetComponent<ActionStateManager>();
        leftHandIK.data.target = weapon.leftHandTarget;
        leftHandIK.data.hint = weapon.leftHandHint;
        actions.SetWeapon(weapon);
    }

    public void ChangeWeapon(float direction)
    {
        weapons[currentWeaponIndex].gameObject.SetActive(false);

        if (direction < 0.0f) // scrolling downwards
        {
            if (currentWeaponIndex == 0) currentWeaponIndex = weapons.Length - 1;
            else currentWeaponIndex--;
        }
        else
        {
            if (currentWeaponIndex == weapons.Length - 1) currentWeaponIndex = 0;
            else currentWeaponIndex++;
        }
        
        weapons[currentWeaponIndex].gameObject.SetActive(true);
        EventManager.Instance.InvokeOnWeaponChanged();
    }

    public void WeaponPutAway()
    {
        ChangeWeapon(actions.Default.scrollDirection);
    }

    public void WeaponPulledOut()
    {
        actions.SwitchState(actions.Default);
    }

    private void OnAmmoCollected(AmmoType ammoType, int amount)
    {
        switch (ammoType)
        {
            case AmmoType.M4_556:
                weapons[0].GetComponent<WeaponAmmo>().OnAmmoCollected(amount);
                break;
            case AmmoType.AK47_762:
                weapons[1].GetComponent<WeaponAmmo>().OnAmmoCollected(amount);
                break;
        }
    }

    private void OnWeaponAcquired(WeaponName weaponName)
    {
        LevelManager.Instance.collectedAK47 = true;
        actions.SwitchState(actions.Swap);
    }

}
