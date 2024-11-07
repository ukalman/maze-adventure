
using UnityEngine;

public class RunState : MovementBaseState
{
    public override void EnterState(MovementStateManager movement)
    {
        movement.anim.SetBool("Running",true);
    }

    public override void UpdateState(MovementStateManager movement)
    {
        if (Input.GetKeyUp(KeyCode.LeftShift)) ExitState(movement, movement.Walk);
        else if (movement.dir.magnitude < 0.1f) ExitState(movement, movement.Idle);
        
        if (movement.zInput < 0.0f) movement.currentMoveSpeed = movement.runBackSpeed;
        else movement.currentMoveSpeed = movement.runSpeed;
    }
    
    private void ExitState(MovementStateManager movement, MovementBaseState stateToSwitch)
    {
        movement.anim.SetBool("Running", false);
        movement.SwitchState(stateToSwitch);
    }
}
