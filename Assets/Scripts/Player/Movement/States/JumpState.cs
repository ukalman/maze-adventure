
using UnityEngine;

public class JumpState : MovementBaseState
{
    public override void EnterState(MovementStateManager movement)
    {
        if (movement.PreviousState == movement.Idle) movement.anim.SetTrigger("IdleJump");
        else if (movement.PreviousState == movement.Walk || movement.PreviousState == movement.Run) movement.anim.SetTrigger("RunJump");
    }

    public override void UpdateState(MovementStateManager movement)
    {
        if (movement.jumped && movement.IsGrounded())
        {
            movement.jumped = false;
            if (movement.xInput == 0.0f && movement.zInput == 0.0f) movement.SwitchState(movement.Idle);
            else if (Input.GetKey(KeyCode.LeftShift)) movement.SwitchState(movement.Run);
            else movement.SwitchState(movement.Walk);
        }
    }
}
