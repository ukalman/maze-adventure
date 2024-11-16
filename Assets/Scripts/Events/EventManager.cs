using System;
using UnityEngine;
using UnityEngine.Events;

public class EventManager : MonoBehaviour
{
    public static EventManager Instance { get; private set; }
    
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
    
    // convert all these UnityActions to delegates and events afterwards. ENCOURAGE BEING EXPLICIT!
    public event Action OnFirstAidUsed;
    public event Action OnWeaponChanged;
    
    public event Action OnAmmoAmountChanged;

    public event Action<AmmoType, int> OnAmmoCollected;

    public event Action<WeaponName> OnWeaponAcquired;

    public event Action OnMazeGenerated;

    public event Action<int> OnEnemyDied;
    
    public void InvokeOnFirstAidUsed()
    {
        OnFirstAidUsed?.Invoke();
    }

    public void InvokeOnWeaponChanged()
    {
        OnWeaponChanged?.Invoke();
    }

    public void InvokeOnAmmoChanged()
    {
        OnAmmoAmountChanged?.Invoke();
    }

    public void InvokeOnAmmoCollected(AmmoType ammoType, int amount)
    {
        OnAmmoCollected?.Invoke(ammoType, amount);
    }

    public void InvokeOnWeaponAcquired(WeaponName weaponName)
    {
        OnWeaponAcquired?.Invoke(weaponName);
    }

    public void InvokeOnMazeGenerated()
    {
        OnMazeGenerated?.Invoke();
    }

    public void InvokeOnEnemyDied(int groupID)
    {
        OnEnemyDied?.Invoke(groupID);
    }
    
}