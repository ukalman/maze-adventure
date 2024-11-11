using System.Collections;
using UnityEngine;

public class EnemyScreamingState : EnemyBaseState
{
    private bool animFinished;

    public override void EnterState(EnemyController controller)
    {
        Debug.Log("We're screaming!");
        animFinished = false; // Reset the flag
        if (controller.anim != null) 
        {
            controller.anim.SetBool("ZombieScreaming", true);
            controller.StartCoroutine(WaitForScreamingAnimation(controller));
        }
    }

    public override void UpdateState(EnemyController controller)
    {
        if (controller.isDead) 
        {
            ExitState(controller, controller.Death);
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

        if (controller.IsPlayerInAttackingDist()) 
        {
            ExitState(controller, controller.Attack);
        }
    }

    public override void ExitState(EnemyController controller, EnemyBaseState stateToSwitch)
    {
        if (controller.anim != null) 
            controller.anim.SetBool("ZombieScreaming", false);

        controller.SwitchState(stateToSwitch);
    }

    private IEnumerator WaitForScreamingAnimation(EnemyController controller)
    {
        // Wait until the animation has finished or the enemy dies
        while ((!animFinished && !controller.isDead) &&
               (controller.anim.GetCurrentAnimatorStateInfo(0).normalizedTime < 1.0f ||
                controller.anim.GetCurrentAnimatorStateInfo(0).IsName("ZombieScreaming")))
        {
            yield return null;
        }

        // Set animFinished to true only if the enemy is still alive
        if (!controller.isDead)
        {
            animFinished = true;
        }
    }
}