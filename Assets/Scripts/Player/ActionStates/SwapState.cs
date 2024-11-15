using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SwapState : ActionBaseState
{
    public override void EnterState(ActionStateManager actions)
    {
        actions.anim.SetTrigger("SwapWeapon");
        actions.leftHandIK.weight = 0.0f;
        actions.rightHandAim.weight = 0.0f;
        
        actions.currentStateName = "Swap";
    }

    public override void UpdateState(ActionStateManager actions)
    {
        
    }
}
