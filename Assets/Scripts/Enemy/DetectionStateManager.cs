using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DetectionStateManager : MonoBehaviour
{
    [SerializeField] private float lookDistance = 25.0f, fov = 120.0f;
    [SerializeField] private Transform enemyEyes;
    
    [SerializeField] private float activateDistance = 35.0f;
    
    private Transform playerHead;
    private PlayerHealth playerHealth;
    
    private Transform originalEnemyEyes;
    private EnemyController controller;

    private float playerSeenTimer;// Timer to track how long the player has been "seen"
    private const float minChaseDuration = 5.0f;
    private float hearingDistance = 25.0f;
    public bool playerHeardRecently; 
    private float playerHeardDuration = 3.0f; // Duration to consider the player "heard"

    private bool isPaused;
    
    // Start is called before the first frame update
    void Start()
    {
        playerHead = GameManager.Instance.playerHead;
        playerHealth = GameManager.Instance.Player.GetComponent<PlayerHealth>();
        controller = GetComponent<EnemyController>();
        EventManager.Instance.OnPlayerFired += OnPlayerFired;
        
        EventManager.Instance.OnDroneCamActivated += OnDroneCamActivated;
        EventManager.Instance.OnDroneCamDeactivated += OnDroneCamDeactivated;
        EventManager.Instance.OnGamePaused += OnGamePaused;
        EventManager.Instance.OnGameContinued += OnGameContinued;
    }

    private void OnDestroy()
    {
        EventManager.Instance.OnPlayerFired -= OnPlayerFired;
        
        EventManager.Instance.OnDroneCamActivated -= OnDroneCamActivated;
        EventManager.Instance.OnDroneCamDeactivated -= OnDroneCamDeactivated;
        EventManager.Instance.OnGamePaused -= OnGamePaused;
        EventManager.Instance.OnGameContinued -= OnGameContinued;
    }

    // Update is called once per frame
    void Update()
    {
        if (!LevelManager.Instance.HasLevelStarted) return;
        
        if (LevelManager.Instance.playerDied) return;
        
        if (isPaused) return;

        if (controller.deathMarchActive) return;
        
        
        CheckIfPlayerInProximity();
    }

    private void FixedUpdate()
    {
        if (!LevelManager.Instance.HasLevelStarted) return;

        if (LevelManager.Instance.playerDied)
        {
            controller.playerSeen = false;
            return;
        }
        
        if (isPaused) return;

        if (controller.deathMarchActive)
        {
            controller.playerSeen = true;
            controller.isAwayFromPlayer = false;
            controller.distToPlayer = Vector3.Distance(transform.position, playerHead.position);
            return;
        }
        
        if (controller.isAwayFromPlayer) return;
        
        if (PlayerSeen() || playerHeardRecently)
        {
            // Reset the timer because the player is seen
            playerSeenTimer = Mathf.Max(playerSeenTimer, minChaseDuration);

            // Update enemy distance and state
            controller.distToPlayer = Vector3.Distance(transform.position, playerHead.position);
            controller.playerSeen = true;
        }
        else
        {
            // Decrease the timer
            playerSeenTimer -= Time.fixedDeltaTime;

            // If the timer is still active, keep chasing
            if (playerSeenTimer > 0)
            {
                // Update enemy distance to last known player position
                controller.distToPlayer = Vector3.Distance(transform.position, playerHead.position);
                controller.playerSeen = true;
            }
            else
            {
                // Timer expired, stop chasing
                controller.playerSeen = false;
            }
        }
        
        if (playerHeardRecently)
        {
            playerHeardDuration -= Time.fixedDeltaTime;
            if (playerHeardDuration <= 0)
            {
                playerHeardRecently = false; 
            }
        }
        
    }

    private void CheckIfPlayerInProximity()
    {
        if (Vector3.Distance(enemyEyes.position, playerHead.position) > activateDistance)
        {
            if (!controller.isAwayFromPlayer)
            {
                controller.isAwayFromPlayer = true;
                controller.DeactivateEnemyModules();
            }
        }

        else
        {
            if (controller.isAwayFromPlayer)
            {
                controller.isAwayFromPlayer = false;
                controller.ActivateEnemyModules();
            }
        }
        
        controller.isAwayFromPlayer = Vector3.Distance(enemyEyes.position, playerHead.position) > activateDistance;
    }

    private bool PlayerSeen()
    {
        if (playerHealth != null && playerHealth.isDead) return false;
         
        enemyEyes.LookAt(playerHead.position);
        if (Vector3.Distance(enemyEyes.position, playerHead.position) > lookDistance)
        {
            return false;
        }

        Vector3 dirToPlayer = (playerHead.position - enemyEyes.position).normalized;

        float angleToPlayer = Vector3.Angle(enemyEyes.parent.forward, dirToPlayer);

        
        if (controller.CurrentState != controller.Attack && GameManager.Instance.PlayerMovement.isCrouching && !GameManager.Instance.PlayerMovement.isFlashlightOn)
        {
            if (angleToPlayer > fov * 0.5f)
            {
                return false; // fov / 2 degrees for each side
            }
        }
        

        RaycastHit hit;
        if (Physics.Raycast(enemyEyes.position, enemyEyes.forward, out hit, lookDistance))
        {
            if (hit.transform == null)
            {
                return false;
            }

            if (hit.transform.CompareTag(playerHead.tag))
            {
                return true;
            }
            
        }

        return false;

    }
    
    private void OnPlayerFired()
    {
        if (!LevelManager.Instance.HasNexusCore)
        {
            if (Vector3.Distance(transform.position, playerHead.position) <= hearingDistance)
            {
                playerHeardRecently = true; 
                playerHeardDuration = 1.0f; 
                playerSeenTimer = Mathf.Max(playerSeenTimer, minChaseDuration); 
                controller.playerSeen = true; // Ensure the enemy knows the player is detected
            
            } 
        }
        
    }

    private void OnDroneCamActivated()
    {
        isPaused = true;
    }

    private void OnDroneCamDeactivated()
    {
        isPaused = false;
    }

    private void OnGamePaused()
    {
        isPaused = true;
    }

    private void OnGameContinued()
    {
        isPaused = false;
    }
    
}
