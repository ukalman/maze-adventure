

using UnityEngine;

public class DefaultState : ActionBaseState
{
    public override void EnterState(ActionStateManager actions)
    {

    }

    public override void UpdateState(ActionStateManager actions)
    {
        actions.rightHandAim.weight = Mathf.Lerp(actions.rightHandAim.weight, 1.0f, 10.0f * Time.deltaTime);
        actions.leftHandIK.weight = Mathf.Lerp(actions.leftHandIK.weight, 1.0f, 10.0f * Time.deltaTime);
        
        if (Input.GetKeyDown(KeyCode.R) && CanReload(actions))
        {
            actions.SwitchState(actions.Reload);
        }
    }

    private bool CanReload(ActionStateManager action)
    {
        if (action.ammo.currentAmmo == action.ammo.clipSize) return false; 
        if (action.ammo.extraAmmo == 0) return false;
        return true;
    }
}
