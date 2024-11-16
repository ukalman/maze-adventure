
using UnityEngine;

public class WalkState : MovementBaseState
{
    public override void EnterState(MovementStateManager movement)
    {
        movement.anim.SetBool("Walking",true);
    }

    public override void UpdateState(MovementStateManager movement)
    {
        if (Input.GetKey(KeyCode.LeftShift))
        {
            if (movement.currentSprintTime / movement.maxSprintTime >= movement.sprintThreshold) ExitState(movement, movement.Run);
            
        }
        else if (Input.GetKeyDown(KeyCode.LeftControl)) ExitState(movement, movement.Crouch);
        else if (movement.dir.magnitude < 0.1f) ExitState(movement, movement.Idle);

        if (movement.zInput < 0.0f) movement.currentMoveSpeed = movement.walkBackSpeed;
        else movement.currentMoveSpeed = movement.walkSpeed;
        
        if (Input.GetKeyDown(KeyCode.Space))
        {
            movement.PreviousState = this;
            ExitState(movement,movement.Jump);
        }
    }

    private void ExitState(MovementStateManager movement, MovementBaseState stateToSwitch)
    {
        movement.anim.SetBool("Walking", false);
        movement.SwitchState(stateToSwitch);
    }
    
}

