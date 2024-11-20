using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DetectionStateManager : MonoBehaviour
{
    [SerializeField] private float lookDistance = 20.0f, fov = 120.0f;
    [SerializeField] private Transform enemyEyes;
    private Transform playerHead;
    private PlayerHealth playerHealth;
    
    private Transform originalEnemyEyes;
    private EnemyController enemyController;

    private float playerSeenTimer;// Timer to track how long the player has been "seen"
    private const float minChaseDuration = 5.0f;
    private float hearingDistance = 15.0f;
    public bool playerHeardRecently; 
    private float playerHeardDuration = 1.0f; // Duration to consider the player "heard"

    private bool isPaused;
    
    // Start is called before the first frame update
    void Start()
    {
        playerHead = GameManager.Instance.playerHead;
        playerHealth = GameManager.Instance.Player.GetComponent<PlayerHealth>();
        enemyController = GetComponent<EnemyController>();
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
        
    }

    private void FixedUpdate()
    {
        if (!LevelManager.Instance.HasLevelStarted) return;
        
        if (isPaused) return;
        
        if (PlayerSeen() || playerHeardRecently)
        {
            // Reset the timer because the player is seen
            playerSeenTimer = Mathf.Max(playerSeenTimer, minChaseDuration);

            // Update enemy distance and state
            enemyController.distToPlayer = Vector3.Distance(transform.position, playerHead.position);
            enemyController.playerSeen = true;
        }
        else
        {
            // Decrease the timer
            playerSeenTimer -= Time.fixedDeltaTime;

            // If the timer is still active, keep chasing
            if (playerSeenTimer > 0)
            {
                // Update enemy distance to last known player position
                enemyController.distToPlayer = Vector3.Distance(transform.position, playerHead.position);
                enemyController.playerSeen = true;
            }
            else
            {
                // Timer expired, stop chasing
                enemyController.playerSeen = false;
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

    public bool PlayerSeen()
    {
        if (playerHealth != null && playerHealth.isDead) return false;
        if (LevelManager.Instance.HasNexusCore) return true;
         
        enemyEyes.LookAt(playerHead.position);
        if (Vector3.Distance(enemyEyes.position, playerHead.position) > lookDistance)
        {
            return false;
        }

        Vector3 dirToPlayer = (playerHead.position - enemyEyes.position).normalized;

        float angleToPlayer = Vector3.Angle(enemyEyes.parent.forward, dirToPlayer);

        
        if (enemyController.CurrentState != enemyController.Attack && GameManager.Instance.PlayerMovement.isCrouching && !GameManager.Instance.PlayerMovement.isFlashlightOn)
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
                Debug.DrawRay(enemyEyes.position, hit.point, Color.green);
                return true;
            }
            
        }

        return false;

    }
    
    private void OnPlayerFired()
    {
        if (Vector3.Distance(transform.position, playerHead.position) <= hearingDistance)
        {
            Debug.Log("Player heard by enemy!");
            playerHeardRecently = true; 
            playerHeardDuration = 1.0f; 
            playerSeenTimer = Mathf.Max(playerSeenTimer, minChaseDuration); 
            enemyController.playerSeen = true; // Ensure the enemy knows the player is detected
            
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
