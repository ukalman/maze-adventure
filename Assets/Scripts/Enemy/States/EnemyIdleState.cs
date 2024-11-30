using UnityEngine;
using UnityEngine.AI;

public class EnemyIdleState : EnemyBaseState
{
    private float idleTimer;
    private float wanderInterval;
    private bool isWandering;
    private Vector3 wanderTarget;
    private float distanceTolerance = 0.7f;

    private float soundIdleTimer;
    private float soundPlayInterval;

    private float ranTimer;
    private float waitScreamAfterRunDuration;
    private bool canScream;
    
    public override void EnterState(EnemyController controller)
    {
        if (LevelManager.Instance.activeCombatEnemies.Contains(controller))
        {
            LevelManager.Instance.activeCombatEnemies.Remove(controller);
        }
        controller.State = "Idle";
        if (controller.enemyAgent != null)
        {
            controller.enemyAgent.enabled = true;
            controller.enemyAgent.isStopped = true;
            controller.enemyAgent.speed = controller.walkSpeed;
        }

        if (controller.anim != null)
        {
            
            controller.anim.SetBool("ZombieScreaming", false);;
            /*
            controller.anim.SetBool("ZombieRunning", false);
            controller.anim.SetBool("ZombieWalking", false);
            controller.anim.SetBool("ZombieAttacking", false);
            */
            controller.anim.speed = 1.0f;
        }
        
        idleTimer = 0.0f;
        soundIdleTimer = 0.0f;
        wanderInterval = Random.Range(5.0f, 10.0f); 
        soundPlayInterval = Random.Range(10.0f, 15.0f);
        isWandering = false;

        ranTimer = 0.0f;
        waitScreamAfterRunDuration = 5.0f;

        if (controller.PreviousState != controller.Run) canScream = true;
        else canScream = false;


    }

    public override void UpdateState(EnemyController controller)
    {
        
        if (controller.isDead) 
        {
            StopWandering(controller);
            controller.SwitchState(controller.Death);
            return;
        }

        ranTimer += Time.deltaTime;
        if (!canScream && ranTimer >= waitScreamAfterRunDuration) canScream = true;
        

        if (controller.playerSeen)
        {
            StopWandering(controller);
            // 50% chance to scream or run towards the player
            if (Random.Range(0, 100) < 50 && canScream) controller.SwitchState(controller.Scream);
            else controller.SwitchState(controller.Run);
            return;
        }

        if (controller.IsPlayerInAttackingDist()) 
        {
            StopWandering(controller);
            controller.SwitchState(controller.Attack);
            return;
        }

        soundIdleTimer += Time.deltaTime;
        
        // Sound logic
        if (soundIdleTimer >= soundPlayInterval)
        {
            controller.StartCoroutine(controller.enemyAudio.PlaySound(EnemyAudioState.Idle));
            soundPlayInterval = Random.Range(10.0f, 20.0f); 
            soundIdleTimer = 0.0f;
        }

        // Wandering  logic
        idleTimer += Time.deltaTime;
        
        if (idleTimer >= wanderInterval)
        {
            if (isWandering)
            {
                // Stop wandering after reaching the destination or after a set time
                if (!controller.enemyAgent.pathPending &&
                    controller.enemyAgent.remainingDistance <= controller.enemyAgent.stoppingDistance + distanceTolerance)
                {
                    StopWandering(controller);
                }
            }
            else
            {
                StartWandering(controller);
            }
        }
        
        if (isWandering && controller.enemyAgent.remainingDistance <= controller.enemyAgent.stoppingDistance + distanceTolerance) StopWandering(controller);
    }

    public override void ExitState(EnemyController controller, EnemyBaseState stateToSwitch)
    {
        if (controller.enemyAgent != null)
        {
            //controller.enemyAgent.isStopped = true;
            StopWandering(controller);
        }
    }

    private void StartWandering(EnemyController controller)
    {
        isWandering = true;
        idleTimer = 0.0f;
        wanderInterval = Random.Range(5.0f, 10.0f); // Set a new interval for idling after wandering

        // Set the destination to a random position within the NavMesh
        wanderTarget = Extensions.GetRandomPointOnNavMesh(controller.transform.position, 10.0f); // Adjust range as needed
        
        if (wanderTarget != Vector3.zero)
        {
            controller.enemyAgent.isStopped = false;
            controller.enemyAgent.SetDestination(wanderTarget);
            
            if (controller.anim != null)
            {
                controller.anim.SetBool("ZombieWalking", true);
            }
        }
    }

    private void StopWandering(EnemyController controller)
    {
        isWandering = false;
        idleTimer = 0.0f;
        wanderInterval = Random.Range(5.0f, 10.0f); // Reset the interval to idle again

        if (controller.enemyAgent != null && controller.enemyAgent.isActiveAndEnabled) controller.enemyAgent.isStopped = true; 
        if (controller.anim != null) controller.anim.SetBool("ZombieWalking", false);
       
    }
    
}
