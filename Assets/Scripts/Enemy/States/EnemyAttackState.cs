
using UnityEngine;

public class EnemyAttackState : EnemyBaseState
{
    public override void EnterState(EnemyController controller)
    {
        if (controller.enemyAgent != null) controller.enemyAgent.isStopped = true;
        if (controller.anim != null) controller.anim.SetBool("ZombieAttacking",true);
    }

    public override void UpdateState(EnemyController controller)
    {
        if (controller.isDead) ExitState(controller,controller.Death);
        if (!controller.playerSeen)
        {
            ExitState(controller,controller.Idle);
        }
        if (!controller.IsPlayerInAttackingDist())
        {
            ExitState(controller, controller.Run);
        }
        
        controller.transform.LookAt(GameManager.Instance.Player.transform.position);
        
    }

    public override void ExitState(EnemyController controller, EnemyBaseState stateToSwitch)
    {
        if (controller.anim != null) controller.anim.SetBool("ZombieAttacking",false);
        controller.SwitchState(stateToSwitch);
    }
}
