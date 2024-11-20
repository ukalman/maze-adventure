
using System.Collections;
using UnityEngine;

public class EnemyDeathState : EnemyBaseState
{

    private bool animFinished;
    
    public override void EnterState(EnemyController controller)
    {
        if (LevelManager.Instance.activeCombatEnemies.Contains(controller))
        {
            LevelManager.Instance.activeCombatEnemies.Remove(controller);
        }
        
        controller.State = "Death";
        
        if (controller.enemyAgent != null)
        {
            controller.enemyAgent.enabled = true;
            controller.enemyAgent.isStopped = true;
        }
        if (controller.anim != null)
        {
            controller.anim.speed = 1.0f;
            controller.anim.SetTrigger("ZombieDeath");
            //controller.StartCoroutine(WaitForDeathAnimation(controller));
        }

        controller.StartCoroutine(controller.enemyAudio.PlaySound(EnemyAudioState.Death));
    }

    public override void UpdateState(EnemyController controller)
    {
       
    }

    public override void ExitState(EnemyController controller, EnemyBaseState stateToSwitch)
    {

    }

    private IEnumerator WaitForDeathAnimation(EnemyController controller)
    {
        yield return new WaitForSeconds(3.0f);
        animFinished = true;
        
    }
}
