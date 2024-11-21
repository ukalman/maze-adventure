
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

    [SerializeField] private Material transparentMat1, transparentMat2;
    [SerializeField] private GameObject minimapTile;

    public bool isAwayFromPlayer;
    
    public bool isPaused;

    public GameObject zombieGroup;
    
    private void Start()
    {
        zombieGroup = transform.parent.gameObject;
        anim = GetComponent<Animator>();
        enemyAgent = GetComponent<NavMeshAgent>();
        enemyAudio = GetComponent<EnemyAudio>();
        health = GetComponent<EnemyHealth>();
        ragdollManager = GetComponent<RagdollManager>();
        SwitchState(Idle);
        isDead = health.isDead;
        playerHealth = GameManager.Instance.Player.GetComponent<PlayerHealth>();
        
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

    private void Update()
    {
        if (!LevelManager.Instance.HasLevelStarted) return;
        
        if (isPaused) return;
        
        Debug.Log("Distance to the player: " + Vector3.Distance(transform.position, GameManager.Instance.Player.transform.position));
        
        isDead = health.isDead;
        CurrentState.UpdateState(this);
        if (isDead && !isDeadDoubleCheck)
        {
            StartCoroutine(OnDeath());
        }
        
    }
    
    public void ActivateEnemyModules()
    {
        anim.enabled = true;
        enemyAudio.enabled = true;
    }

    public void DeactivateEnemyModules()
    {
        anim.enabled = false;
        enemyAudio.enabled = false;
    }
    
    public void SwitchState(EnemyBaseState stateToSwitch)
    {
        CurrentState = stateToSwitch;
        CurrentState.EnterState(this);
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
        Destroy(enemyAgent);

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

            Destroy(anim);
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

        EventManager.Instance.InvokeOnEnemyDestroy(zombieGroup);
        
        // Destroy the enemy object
        Destroy(gameObject);
    }


    private IEnumerator FadeAway()
    {
        SkinnedMeshRenderer skinnedMeshRenderer = GetComponentInChildren<SkinnedMeshRenderer>();
        if (skinnedMeshRenderer != null)
        {
            // Set the transparent materials
            skinnedMeshRenderer.materials = new Material[] { transparentMat1, transparentMat2 };

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
                foreach (var material in skinnedMeshRenderer.materials)
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
        if (enemyAgent != null) enemyAgent.isStopped = true;
        if (LevelManager.Instance.VeinsActivated) LevelManager.Instance.levelUIManager.RegisterTrackedObject(transform);
    }

    private void OnDroneCamDeactivated()
    {
        isPaused = false;
        if (anim != null) anim.speed = 1;
        if (enemyAgent != null) enemyAgent.isStopped = false;
        LevelManager.Instance.levelUIManager.UnregisterTrackedObject(transform);
    }

    private void OnGamePaused()
    {
        isPaused = true;
        anim.speed = 0;
        enemyAgent.isStopped = true;
    }

    private void OnGameContinued()
    {
        isPaused = false;
        anim.speed = 1;
        enemyAgent.isStopped = false;
    }

    
}
