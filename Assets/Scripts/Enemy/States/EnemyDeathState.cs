
using UnityEngine;

public class EnemyDeathState : EnemyBaseState
{
    public override void EnterState(EnemyController controller)
    {
        Debug.Log("Yes, we're in death state");
        if (controller.enemyAgent != null) controller.enemyAgent.isStopped = true;
        if (controller.anim != null) controller.anim.SetTrigger("ZombieDeath");  
    }

    public override void UpdateState(EnemyController controller)
    {

    }

    public override void ExitState(EnemyController controller, EnemyBaseState stateToSwitch)
    {

    }
}
