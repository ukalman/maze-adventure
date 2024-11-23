using System;
using UnityEngine;
using UnityEngine.Events;

public class EventManager : MonoBehaviour
{
    public static EventManager Instance { get; private set; }
    
    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject); 
    }

    public event Action<GameDifficulty> OnDifficultySelected;

    public event Action<GameDifficulty> OnLevelCompleted;
    
    public event Action OnFirstAidUsed;
    public event Action OnWeaponChanged;
    
    public event Action OnAmmoAmountChanged;

    public event Action<AmmoType, int> OnAmmoCollected;

    public event Action<WeaponName> OnWeaponAcquired;

    public event Action OnMazeGenerated;

    public event Action OnEnemyKilled;
    
    public event Action<GameObject> OnEnemyDestroy;

    public event Action<AmmoType> OnExtraAmmoDepleted;

    public event Action OnRequirementsBeforeNavMeshSpawned;

    public event Action OnNavMeshBaked;

    public event Action OnPlayerFired;

    public event Action OnMazeExit;

    public event Action OnDroneCamActivated;

    public event Action OnDroneCamDeactivated;

    public event Action OnGamePaused;

    public event Action OnGameContinued;

    public event Action OnLightsTurnedOn;

    public event Action OnFlashlightTurnedOn;
    
    public event Action OnFlashlightTurnedOff;

    public event Action OnNexusCoreObtained;

    public event Action OnLevelInstantiated;

    public event Action OnLevelStarted;

    public void InvokeOnDifficultySelected(GameDifficulty difficulty)
    {
        OnDifficultySelected?.Invoke(difficulty);
    }

    public void InvokeOnLevelCompleted(GameDifficulty difficulty)
    {
        OnLevelCompleted?.Invoke(difficulty);
    }

    public void OnEasyDifficultySelected()
    {
        InvokeOnDifficultySelected(GameDifficulty.EASY);
    }
    
    public void OnModerateDifficultySelected()
    {
        InvokeOnDifficultySelected(GameDifficulty.MODERATE);
    }
    
    public void OnHardDifficultySelected()
    {
        InvokeOnDifficultySelected(GameDifficulty.HARD);
    }
    
    public void InvokeOnLevelInstantiated()
    {
        OnLevelInstantiated?.Invoke();
    }

    public void InvokeOnLevelStarted()
    {
        OnLevelStarted?.Invoke();
    }

   
    
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

    public void InvokeOnEnemyKilled()
    {
        OnEnemyKilled?.Invoke();
    }
    
    public void InvokeOnEnemyDestroy(GameObject zombieGroup)
    {
        OnEnemyDestroy?.Invoke(zombieGroup);
    }

    public void InvokeOnExtraAmmoDepleted(AmmoType type)
    {
        OnExtraAmmoDepleted?.Invoke(type);
    }

    public void InvokeOnRequirementsBeforeNavMeshSpawned()
    {
        OnRequirementsBeforeNavMeshSpawned?.Invoke();
    }

    public void InvokeOnNavMeshBaked()
    {
        OnNavMeshBaked?.Invoke();
    }

    public void InvokeOnPlayerFired()
    {
        OnPlayerFired?.Invoke();
    }

    public void InvokeOnMazeExit()
    {
        OnMazeExit?.Invoke();
    }

    public void InvokeOnDroneCamActivated()
    {
        OnDroneCamActivated?.Invoke();
    }
    
    public void InvokeOnDroneCamDeactivated()
    {
        OnDroneCamDeactivated?.Invoke();
    }

    public void InvokeOnGamePaused()
    {
        OnGamePaused?.Invoke();
    }

    public void InvokeOnGameContinued()
    {
        OnGameContinued?.Invoke();
    }

    public void InvokeOnLightsTurnedOn()
    {
        OnLightsTurnedOn?.Invoke();
    }

    public void InvokeOnFlashlightTurnedOn()
    {
        OnFlashlightTurnedOn?.Invoke();
    }
    
    public void InvokeOnFlashlightTurnedOff()
    {
        OnFlashlightTurnedOff?.Invoke();
    }

    public void InvokeOnNexusCoreObtained()
    {
        OnNexusCoreObtained?.Invoke();
    }

    
    
    
}