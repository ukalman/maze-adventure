
using UnityEngine;

public class EnemyRunState : EnemyBaseState
{

    private float timer;
    private float duration;
    private bool canMakeRunSound;
    private Transform playerTransform;

    
    public override void EnterState(EnemyController controller)
    {
        controller.State = "Run";
        playerTransform = GameManager.Instance.Player.transform;
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
            //controller.enemyAgent.SetDestination(GameManager.Instance.Player.transform.position);
            controller.enemyAgent.SetDestination(new Vector3(playerTransform.position.x, 0.1f, playerTransform.position.z));
        }

        
        duration = 2.0f;
        timer = 0.0f;
        
        if (controller.PreviousState == controller.Attack) canMakeRunSound = false;
        else canMakeRunSound = true;

    }

    public override void UpdateState(EnemyController controller)
    {

        timer += Time.deltaTime;

        if (timer >= duration && !canMakeRunSound)
        {
            canMakeRunSound = true;
            timer = 0.0f;
        }
        
        if (canMakeRunSound) controller.StartCoroutine(controller.enemyAudio.PlaySound(EnemyAudioState.Run));

        if (controller.enemyAgent != null)
        {
            //controller.enemyAgent.SetDestination(GameManager.Instance.Player.transform.position);
            controller.enemyAgent.SetDestination(new Vector3(playerTransform.position.x, 0.1f, playerTransform.position.z));
        }

        if (controller.isDead) ExitState(controller,controller.Death);


        if (!controller.playerSeen || controller.playerHealth.isDead)
        {
            Debug.Log("yes, this is the playerSeen : " + controller.playerSeen);
            ExitState(controller,controller.Idle);
        }

        if (controller.IsPlayerInAttackingDist() && controller.playerSeen) ExitState(controller, controller.Attack);
        
        //controller.transform.LookAt(GameManager.Instance.Player.transform.position);
        
    }

    public override void ExitState(EnemyController controller, EnemyBaseState stateToSwitch)
    {
        if (controller.anim != null) controller.anim.SetBool("ZombieRunning",false);
        controller.PreviousState = this;
        controller.SwitchState(stateToSwitch);
    }
}
