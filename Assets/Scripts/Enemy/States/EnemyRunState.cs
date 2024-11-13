
public class EnemyRunState : EnemyBaseState
{
    public override void EnterState(EnemyController controller)
    {
        
        if (controller.anim != null)
        {
            controller.anim.speed = 1.0f;
            controller.anim.SetBool("ZombieRunning",true);
        }

        if (controller.enemyAgent != null)
        {
            controller.enemyAgent.enabled = true;
            controller.enemyAgent.speed = controller.runSpeed;
            controller.enemyAgent.isStopped = false;
            controller.enemyAgent.SetDestination(GameManager.Instance.Player.transform.position);
        }
        
    }

    public override void UpdateState(EnemyController controller)
    {
        if (controller.enemyAgent != null) controller.enemyAgent.SetDestination(GameManager.Instance.Player.transform.position);

        if (controller.isDead) ExitState(controller,controller.Death);
        
        
        if (!controller.playerSeen || controller.playerHealth.isDead) ExitState(controller,controller.Idle);

        if (controller.IsPlayerInAttackingDist() && controller.playerSeen) ExitState(controller, controller.Attack);
        
        //controller.transform.LookAt(GameManager.Instance.Player.transform.position);
        
    }

    public override void ExitState(EnemyController controller, EnemyBaseState stateToSwitch)
    {
        if (controller.anim != null) controller.anim.SetBool("ZombieRunning",false);
        controller.SwitchState(stateToSwitch);
    }
}
