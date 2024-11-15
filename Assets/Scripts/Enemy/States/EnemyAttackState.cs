
using UnityEngine;

public class EnemyAttackState : EnemyBaseState
{
    public override void EnterState(EnemyController controller)
    {
        if (controller.enemyAgent != null)
        {
            controller.enemyAgent.enabled = true;
            controller.enemyAgent.isStopped = true;
        }
        if (controller.anim != null)
        {
            controller.anim.speed = 1.5f;
            controller.anim.SetBool("ZombieAttacking",true);
        }
    }

    public override void UpdateState(EnemyController controller)
    {
        if (controller.isDead) ExitState(controller,controller.Death);
        if (!controller.playerSeen || controller.playerHealth.isDead) ExitState(controller,controller.Idle);
        if (!controller.IsPlayerInAttackingDist() && controller.playerSeen) ExitState(controller, controller.Run);
        if (controller.IsPlayerInAttackingDist() && Random.Range(1,100) < 50) controller.StartCoroutine(controller.enemyAudio.PlaySound(EnemyAudioState.Attack));
        
        controller.transform.LookAt(GameManager.Instance.Player.transform.position);
        
    }

    public override void ExitState(EnemyController controller, EnemyBaseState stateToSwitch)
    {
        if (controller.anim != null) controller.anim.SetBool("ZombieAttacking",false);
        //controller.enemyAudio.StopSound();
        controller.PreviousState = this;
        controller.SwitchState(stateToSwitch);
    }
}
