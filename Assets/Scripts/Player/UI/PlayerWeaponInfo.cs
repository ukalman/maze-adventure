using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PlayerWeaponInfo : MonoBehaviour
{
    [SerializeField] private Image weaponImage;
    [SerializeField] private TMP_Text weaponName, weaponAmmo;

    private WeaponClassManager weaponClassManager;
    private WeaponManager currentWeapon;
    private WeaponAmmo currentWeaponAmmo;
    
    private StringBuilder ammoStringBuilder = new StringBuilder();
    
    void Start()
    {
        EventManager.Instance.OnWeaponChanged += OnWeaponChanged;
        EventManager.Instance.OnAmmoAmountChanged += UpdateAmmoText;
        
        weaponClassManager = GameManager.Instance.Player.GetComponent<WeaponClassManager>();

        if (weaponClassManager != null)
        {
            currentWeapon = weaponClassManager.weapons[weaponClassManager.currentWeaponIndex];

            if (currentWeapon != null)
            {
                currentWeaponAmmo = currentWeapon.GetComponent<WeaponAmmo>();
                if (currentWeaponAmmo != null)
                {
                    SetWeaponInfo();
                }
            }
            
            
        }
        
        UpdateAmmoText();
        
    }

    private void OnDestroy()
    {
        EventManager.Instance.OnWeaponChanged -= OnWeaponChanged;
        EventManager.Instance.OnAmmoAmountChanged -= UpdateAmmoText;
    }

    
    private void OnWeaponChanged()
    {
        currentWeapon = weaponClassManager.weapons[weaponClassManager.currentWeaponIndex];
        currentWeaponAmmo = currentWeapon.GetComponent<WeaponAmmo>();
        
        SetWeaponInfo();
        UpdateAmmoText();
    }

    private void SetWeaponInfo()
    {
        //var weapon = weaponClassManager.weapons[this.weaponClassManager.currentWeaponIndex];
        
        weaponImage.sprite = currentWeapon.weaponSprite;
        weaponName.text = currentWeapon.weaponName;
        
        //UpdateAmmoText();
    }

    private void UpdateAmmoText()
    {
        ammoStringBuilder.Clear();
        ammoStringBuilder.Append(currentWeaponAmmo.currentAmmo);
        ammoStringBuilder.Append(" / ");
        ammoStringBuilder.Append(currentWeaponAmmo.extraAmmo);

        weaponAmmo.text = ammoStringBuilder.ToString();
    }
}
