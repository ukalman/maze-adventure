using System.Collections;
using UnityEngine;

public class EnemyScreamingState : EnemyBaseState
{
    private bool animFinished;
    private float rotationDuration = 1.5f; 
    public override void EnterState(EnemyController controller)
    {
        controller.State = "Scream";
        animFinished = false;
        controller.enemyAgent.enabled = false;
        controller.StartCoroutine(RotateTowardsPlayerAndStartScreaming(controller));
    }

    public override void UpdateState(EnemyController controller)
    {
        if (controller.isDead) 
        {
            ExitState(controller, controller.Death);
            return;
        }

        if (!controller.playerSeen || controller.playerHealth.isDead)
        {
            ExitState(controller,controller.Idle);
            return;
        }
        
        if (animFinished) 
        {
            if (controller.playerSeen) 
                ExitState(controller, controller.Run);
            else 
                ExitState(controller, controller.Idle);

            return;
        }

        if (controller.IsPlayerInAttackingDist() && controller.playerSeen) ExitState(controller, controller.Attack);
        
    }

    public override void ExitState(EnemyController controller, EnemyBaseState stateToSwitch)
    {
        if (controller.anim != null) 
            controller.anim.SetBool("ZombieScreaming", false);

        controller.SwitchState(stateToSwitch);
    }
    
    private IEnumerator RotateTowardsPlayerAndStartScreaming(EnemyController controller)
    {
        Vector3 directionToPlayer = (GameManager.Instance.Player.transform.position - controller.transform.position).normalized;
        
        float angleOffset = 10f; // how many degrees before fully facing the player
        
        directionToPlayer = Quaternion.AngleAxis(angleOffset, Vector3.up) * directionToPlayer;
        
        Quaternion targetRotation = Quaternion.LookRotation(directionToPlayer);
        
        float timeElapsed = 0f;
        Quaternion startRotation = controller.transform.rotation;
    
        while (timeElapsed < rotationDuration)
        {
            timeElapsed += Time.deltaTime;
            controller.transform.rotation = Quaternion.Slerp(startRotation, targetRotation, timeElapsed / rotationDuration);
            yield return null;
        }
        
        controller.transform.rotation = targetRotation;

        // Start the scream animation after rotation completes
        if (controller.anim != null) 
        {
            controller.anim.SetBool("ZombieScreaming", true);
            controller.StartCoroutine(WaitForScreamingAnimation(controller));
        }
    }
    
    private IEnumerator WaitForScreamingAnimation(EnemyController controller)
    {
        /*
        while ((!animFinished && !controller.isDead) &&
               (controller.anim.GetCurrentAnimatorStateInfo(0).normalizedTime < 1.0f ||
                controller.anim.GetCurrentAnimatorStateInfo(0).IsName("ZombieScreaming")))
        {
            yield return null;
        }
        */

        yield return new WaitForSeconds(2.5f);
        
        if (!controller.isDead)
        {
            animFinished = true;
        }
    }

    
}