
using UnityEngine;

public class IdleState : MovementBaseState
{
    public override void EnterState(MovementStateManager movement)
    {
        
    }

    public override void UpdateState(MovementStateManager movement)
    {
        if (movement.dir.magnitude > 0.1f)
        {
            if (Input.GetKey(KeyCode.LeftShift))
            {
                if (movement.currentSprintTime / movement.maxSprintTime >= movement.sprintThreshold) movement.SwitchState(movement.Run);
                
            }
            else
            {
                movement.SwitchState(movement.Walk);
            }
        }

        if (Input.GetKeyDown(KeyCode.LeftControl))
        {
            movement.SwitchState(movement.Crouch);
        }

        if (Input.GetKeyDown(KeyCode.Space))
        {
            movement.PreviousState = this;
            movement.SwitchState(movement.Jump);
        }
    }
    
}