
public class EnemyRunState : EnemyBaseState
{
    public override void EnterState(EnemyController controller)
    {
        if (controller.anim != null)
        {
            controller.anim.SetBool("ZombieRunning",true);
        }

        if (controller.enemyAgent != null)
        {
            controller.enemyAgent.isStopped = false;
            controller.enemyAgent.SetDestination(GameManager.Instance.Player.transform.position);
        }
        
    }

    public override void UpdateState(EnemyController controller)
    {
        if (controller.enemyAgent != null) controller.enemyAgent.SetDestination(GameManager.Instance.Player.transform.position);

        if (controller.isDead) ExitState(controller,controller.Death);
        
        
        if (!controller.playerSeen) ExitState(controller,controller.Idle);

        if (controller.IsPlayerInAttackingDist()) ExitState(controller, controller.Attack);
        
        //controller.transform.LookAt(GameManager.Instance.Player.transform.position);
        
    }

    public override void ExitState(EnemyController controller, EnemyBaseState stateToSwitch)
    {
        if (controller.anim != null) controller.anim.SetBool("ZombieRunning",false);
        controller.SwitchState(stateToSwitch);
    }
}
