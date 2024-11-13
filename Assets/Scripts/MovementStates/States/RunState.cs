
using System.Collections;
using UnityEngine;

public class RunState : MovementBaseState
{
    
    public override void EnterState(MovementStateManager movement)
    {
        movement.StartSprinting();
        movement.anim.SetBool("Running", true);
    }

    public override void UpdateState(MovementStateManager movement)
    {
        if (Input.GetKeyUp(KeyCode.LeftShift) || movement.currentSprintTime <= 0)
        {
            ExitState(movement, movement.Walk);
            return;
        }

        if (movement.dir.magnitude < 0.1f) ExitState(movement, movement.Idle);
        
        if (movement.zInput < 0.0f) movement.currentMoveSpeed = movement.runBackSpeed;
        else movement.currentMoveSpeed = movement.runSpeed;
        
        if (Input.GetKeyDown(KeyCode.Space))
        {
            movement.PreviousState = this;
            ExitState(movement,movement.Jump);
        }
    }
    
    private void ExitState(MovementStateManager movement, MovementBaseState stateToSwitch)
    {
        movement.StopSprinting(); // Ensure sprinting stops on exit
        movement.anim.SetBool("Running", false);
        movement.SwitchState(stateToSwitch);
    }
    
}

