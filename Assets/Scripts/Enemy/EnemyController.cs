
using System;
using System.Collections;
using UnityEngine;
using UnityEngine.AI;


public class EnemyController : MonoBehaviour
{
    public float runSpeed = 3.5f;
    public float walkSpeed = 1.0f;
    public float distToPlayer;
    public float attackDist = 2.5f;
    private float attackDamage = 15.0f;
    
    public bool playerSeen;
    public bool isDead;
    private bool isDeadDoubleCheck;

    public PlayerHealth playerHealth;
    
    #region EnemyStates

    public string State;
    
    public EnemyBaseState CurrentState { get; private set; }
    public EnemyBaseState PreviousState;
    
    public EnemyIdleState Idle = new EnemyIdleState();
    public EnemyRunState Run = new EnemyRunState();
    public EnemyAttackState Attack = new EnemyAttackState();
    public EnemyDeathState Death = new EnemyDeathState();
    public EnemyScreamingState Scream = new EnemyScreamingState();
    
    #endregion
    
    [HideInInspector] public Animator anim;
    [HideInInspector] public EnemyAudio enemyAudio;
    [HideInInspector] public NavMeshAgent enemyAgent;
    
    private EnemyHealth health;
    public RagdollManager ragdollManager;

    [SerializeField] private Material originalMat1, originalMat2;
    [SerializeField] private Material transparentMat1, transparentMat2;
    private SkinnedMeshRenderer renderer;
    [SerializeField] private GameObject minimapTile;

    public bool isAwayFromPlayer;
    
    public bool isPaused;

    public bool deathMarchActive;
    
    public ZombieGroup zombieGroup;
    
    [SerializeField] private int groupIndex;
    
    
    private void Start()
    {
        zombieGroup = transform.parent.GetComponent<ZombieGroup>();
        renderer = GetComponentInChildren<SkinnedMeshRenderer>();
        anim = GetComponent<Animator>();
        enemyAgent = GetComponent<NavMeshAgent>();
        enemyAudio = GetComponent<EnemyAudio>();
        health = GetComponent<EnemyHealth>();
        ragdollManager = GetComponent<RagdollManager>();
        SwitchState(Idle);
        isDead = health.isDead;
        playerHealth = GameManager.Instance.Player.GetComponent<PlayerHealth>();

        switch (LevelManager.Instance.GetGameDifficulty())
        {
            case GameDifficulty.EASY:
                attackDamage = 15.0f;
                break;
            case GameDifficulty.MODERATE:
                attackDamage = 20.0f;
                break;
            case GameDifficulty.HARD:
                attackDamage = 25.0f;
                break;
        }
        
        EventManager.Instance.OnDroneCamActivated += OnDroneCamActivated;
        EventManager.Instance.OnDroneCamDeactivated += OnDroneCamDeactivated;
        EventManager.Instance.OnGamePaused += OnGamePaused;
        EventManager.Instance.OnGameContinued += OnGameContinued;

        EventManager.Instance.OnCountdownEnded += OnCountdownEnded;
        
        EventManager.Instance.OnNexusCoreObtained += OnNexusCoreObtained;
        
        EventManager.Instance.OnMazeExit += OnMazeExit;
    }

    private void OnDestroy()
    {
        EventManager.Instance.OnDroneCamActivated -= OnDroneCamActivated;
        EventManager.Instance.OnDroneCamDeactivated -= OnDroneCamDeactivated;
        EventManager.Instance.OnGamePaused -= OnGamePaused;
        EventManager.Instance.OnGameContinued -= OnGameContinued;
        
        EventManager.Instance.OnCountdownEnded -= OnCountdownEnded;
        
         
        EventManager.Instance.OnNexusCoreObtained -= OnNexusCoreObtained;
        
        EventManager.Instance.OnMazeExit -= OnMazeExit;
    }

    private void Update()
    {
        if (!LevelManager.Instance.HasLevelStarted) return;
        
        if (isPaused) return;
        
        isDead = health.isDead;
        
        if (isAwayFromPlayer) return;

        if (LevelManager.Instance.HasNexusCore && !deathMarchActive) deathMarchActive = true;
        
        CurrentState.UpdateState(this);
        if (isDead && !isDeadDoubleCheck)
        {
            StartCoroutine(OnDeath());
        }
        
    }
    
    public void ActivateEnemyModules()
    {
        enemyAudio.enabled = true;
        enemyAgent.enabled = true;
    }

    public void DeactivateEnemyModules()
    {
        enemyAudio.enabled = false;
        enemyAgent.enabled = false;
    }
    
    public void SwitchState(EnemyBaseState stateToSwitch)
    {
        CurrentState = stateToSwitch;
        CurrentState.EnterState(this);
    }

    public void SetGroupIndex(int index)
    {
        groupIndex = index;
    }

    public int GetGroupIndex()
    {
        return groupIndex;
    }

    public void ResetEnemy()
    {
       health.ResetModule();
       minimapTile.layer = LayerMask.NameToLayer("MinimapTile");
     
       minimapTile.SetActive(true);
       
       isDead = false;
       isDeadDoubleCheck = false;
       
       ragdollManager.DeactivateRagdoll();
       
       enemyAgent.enabled = true;
       anim.enabled = true;
    }
    
    public bool IsPlayerInAttackingDist()
    {
        if (playerHealth != null)
        {
            if (distToPlayer != 0.0f && !playerHealth.isDead)
            {
                return distToPlayer <= attackDist;
            }
        }
        
        return false;
    }

    public void AttackPlayer()
    {
        if (playerHealth != null)
        {
            if (playerHealth.isDead) return;
            playerHealth.TakeDamage(attackDamage);
        }
        
    }

    private IEnumerator OnDeath()
    {
        EventManager.Instance.InvokeOnEnemyKilled();
        //minimapTile.SetActive(false);
        isDead = false;
        isDeadDoubleCheck = true;

        // Destroy the enemy's NavMesh agent
        enemyAgent.enabled = false;
        //Destroy(enemyAgent);

        // Handle animations and ragdoll
        if (anim != null)
        {
            float waitTime = 3.0f;
            float elapsedTime = 0f;

            while (elapsedTime < waitTime)
            {
                if (!isPaused)
                {
                    elapsedTime += Time.deltaTime;
                }
                yield return null; // Waits for the game to unpause if paused
            }

            anim.enabled = false;
            ragdollManager.TriggerRagdoll();
        }

        // Wait before deactivating ragdoll
        float ragdollWaitTime = 2.0f;
        float ragdollElapsedTime = 0f;

        while (ragdollElapsedTime < ragdollWaitTime)
        {
            if (!isPaused)
            {
                ragdollElapsedTime += Time.deltaTime;
            }
            yield return null;
        }

        ragdollManager.DeactivateRagdoll();

        // Fade away the object
        yield return StartCoroutine(FadeAway());
        
        // Set the zombie inactive
        zombieGroup.SetZombieInactive(groupIndex);
        
        if (renderer != null) renderer.materials = new Material[] { originalMat1, originalMat2 };
        
        
        // Destroy the enemy object
        //Destroy(gameObject);
    }


    private IEnumerator FadeAway()
    {
        if (renderer != null)
        {
            // Set the transparent materials
            renderer.materials = new Material[] { transparentMat1, transparentMat2 };

            // Fade duration
            float fadeDuration = 3.0f;
            float elapsedTime = 0f;

            // Begin fading out by adjusting the alpha over time
            while (elapsedTime < fadeDuration)
            {
                // Pause handling
                while (isPaused)
                {
                    yield return null; // Wait until the game is unpaused
                }

                // Increment elapsed time
                elapsedTime += Time.deltaTime;
                float newOpacity = Mathf.Lerp(1.0f, 0, elapsedTime / fadeDuration);

                // Update the alpha for each material
                foreach (var material in renderer.materials)
                {
                    Color color = material.color;
                    color.a = newOpacity;
                    material.color = color;
                }

                yield return null;
            }
        }
    }


    private void OnDroneCamActivated()
    {
        isPaused = true;
        if (anim != null) anim.speed = 0;
        if (enemyAgent != null && enemyAgent.isActiveAndEnabled) enemyAgent.isStopped = true;
        if (LevelManager.Instance.VeinsActivated && !health.isDead) LevelManager.Instance.levelUIManager.RegisterTrackedObject(transform);
    }

    private void OnDroneCamDeactivated()
    {
        isPaused = false;
        if (anim != null) anim.speed = 1;
        if (enemyAgent != null && enemyAgent.isActiveAndEnabled) enemyAgent.isStopped = false;
        LevelManager.Instance.levelUIManager.UnregisterTrackedObject(transform);
    }

    private void OnGamePaused()
    {
        isPaused = true;
        if (anim != null && anim.isActiveAndEnabled) anim.speed = 0;
        if (enemyAgent != null && enemyAgent.isActiveAndEnabled) enemyAgent.isStopped = true;
    }

    private void OnGameContinued()
    {
        isPaused = false;
        if (anim != null && anim.isActiveAndEnabled) anim.speed = 1;
        if (enemyAgent != null && enemyAgent.isActiveAndEnabled) enemyAgent.isStopped = false;
    }

    private void OnCountdownEnded()
    {
        gameObject.SetActive(false);
    }

    private void OnNexusCoreObtained()
    {
        playerSeen = true;
        isAwayFromPlayer = false;
        deathMarchActive = true;
    }

    private void OnMazeExit()
    {
        gameObject.SetActive(false);
    }

    
}
