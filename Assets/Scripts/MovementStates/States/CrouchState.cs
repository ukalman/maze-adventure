
using UnityEngine;

public class CrouchState : MovementBaseState
{
    public override void EnterState(MovementStateManager movement)
    {
        movement.anim.SetBool("Crouching",true);
    }

    public override void UpdateState(MovementStateManager movement)
    {
        if (Input.GetKey(KeyCode.LeftShift)) ExitState(movement, movement.Run);
        if (Input.GetKeyDown(KeyCode.LeftControl))
        {
            if (movement.dir.magnitude < 0.1f) ExitState(movement, movement.Idle);
            else ExitState(movement, movement.Walk);
        }
        
        if (movement.zInput < 0.0f) movement.currentMoveSpeed = movement.crouchBackSpeed;
        else movement.currentMoveSpeed = movement.crouchSpeed;
    }
    
    private void ExitState(MovementStateManager movement, MovementBaseState stateToSwitch)
    {
        movement.anim.SetBool("Crouching", false);
        movement.SwitchState(stateToSwitch);
    }
}
