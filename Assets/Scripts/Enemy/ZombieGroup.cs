using System.Collections;
using UnityEngine;

public class ZombieGroup : MonoBehaviour
{
    [SerializeField] private EnemyController[] zombies;

    private bool[] activityStatus;

    private float distFromPlayer = 30.0f;

    private float elapsedTime;
    private float waitingTime = 30.0f;

    private bool deathMarch;
    private bool isZombieWakeupRoutineRunning;

    private bool isPaused;
    
    // Start is called before the first frame update
    void Start()
    {
        activityStatus = new bool[zombies.Length];

        EventManager.Instance.OnGamePaused += OnGamePaused;
        EventManager.Instance.OnGamePaused += OnGameContinued;
        EventManager.Instance.OnNexusCoreObtained += OnNexusCoreObtained;
        EventManager.Instance.OnMazeExit += StopDeathMarch;
        EventManager.Instance.OnPlayerDied += StopDeathMarch;
        EventManager.Instance.OnCountdownEnded += StopDeathMarch;
    }

    private void OnDestroy()
    {
        EventManager.Instance.OnGamePaused -= OnGamePaused;
        EventManager.Instance.OnGamePaused -= OnGameContinued;
        EventManager.Instance.OnNexusCoreObtained -= OnNexusCoreObtained;
        EventManager.Instance.OnMazeExit -= StopDeathMarch;
        EventManager.Instance.OnPlayerDied -= StopDeathMarch;
        EventManager.Instance.OnCountdownEnded -= StopDeathMarch;
    }

    private void SetIndices()
    {
        for (int i = 0; i < zombies.Length; i++)
        {
            zombies[i].SetGroupIndex(i);
            activityStatus[i] = true;
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private IEnumerator ZombieWakeUpRoutine()
    {
        while (deathMarch)
        {
            while (isPaused)
            {
                yield return null;
            }

            yield return new WaitForSeconds(1.0f);
            elapsedTime += 1.0f;
            
            if (elapsedTime >= waitingTime)
            {
                elapsedTime = 0.0f;
                WakeZombiesUp();
            }
        }
    }
    
    private void SetZombieActive(int index)
    {
        activityStatus[index] = true;
        zombies[index].gameObject.SetActive(true);
        zombies[index].SwitchState(zombies[index].Idle);
    }
    
    public void SetZombieInactive(int index)
    {
        activityStatus[index] = false;
        zombies[index].gameObject.SetActive(false);
    }

    private void WakeZombieUp(int index)
    { 
        zombies[index].ResetEnemy();
        SetZombieActive(index);
        
    }

    private void ResetTransform(int index)
    {
        Vector3 spawnPoint = Extensions.GetSpawnPointFarFromPlayer(
            GameManager.Instance.MazeGrid, 
            GameManager.Instance.Player.transform.position, 
            distFromPlayer // Minimum distance from player
        );

        zombies[index].transform.position = spawnPoint;
        zombies[index].transform.rotation = Quaternion.identity;
    }

    private void OnGamePaused()
    {
        isPaused = true;
    }

    private void OnGameContinued()
    {
        isPaused = false;
    }
    
    private void OnNexusCoreObtained()
    {
        deathMarch = true;
        WakeZombiesUp();

        StartCoroutine(ZombieWakeUpRoutine());
    }
    
    private void WakeZombiesUp()
    {
        for (int i = 0; i < zombies.Length; i++)
        {
            if (!activityStatus[i])
            {
                // Reset the position of zombie, reset its modules and set it active
                ResetTransform(i);
                WakeZombieUp(i);
            }
        }
    }

    private void StopDeathMarch()
    {
        deathMarch = false;
    }
}
