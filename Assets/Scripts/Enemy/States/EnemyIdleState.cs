
using UnityEngine;

public class EnemyIdleState : EnemyBaseState
{
    public override void EnterState(EnemyController controller)
    {
        if (controller.enemyAgent != null) controller.enemyAgent.isStopped = true;
    }

    public override void UpdateState(EnemyController controller)
    {
        if (controller.isDead) controller.SwitchState(controller.Death);
        if (controller.playerSeen)
        {
            if (Random.Range(1,100) < 50)
            {
                controller.SwitchState(controller.Scream);
            }
            else controller.SwitchState(controller.Run);
        }
        if (controller.IsPlayerInAttackingDist()) controller.SwitchState(controller.Attack);
    }

    public override void ExitState(EnemyController controller, EnemyBaseState stateToSwitch)
    {
      
    }
}
