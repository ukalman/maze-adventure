

using UnityEngine;

public class DefaultState : ActionBaseState
{
    public float scrollDirection;
    
    public override void EnterState(ActionStateManager actions)
    {
        actions.currentStateName = "Default";
    }

    public override void UpdateState(ActionStateManager actions)
    {
        //actions.rightHandAim.weight = Mathf.Lerp(actions.rightHandAim.weight, 1.0f, 10.0f * Time.deltaTime);
        if (actions.leftHandIK.weight == 0.0f) actions.leftHandIK.weight = 1.0f;
        if (actions.rightHandAim.weight == 0.0f) actions.rightHandAim.weight = 1.0f;
        //actions.leftHandIK.weight = Mathf.Lerp(actions.leftHandIK.weight, 1.0f, 10.0f * Time.deltaTime);
        
        if (Input.GetKeyDown(KeyCode.R) && CanReload(actions))
        {
            actions.SwitchState(actions.Reload);
        }
        // TODO change this later, not suitable for new weapons
        else if (Input.mouseScrollDelta.y != 0.0f && LevelManager.Instance.CollectedAK47)
        {
            scrollDirection = Input.mouseScrollDelta.y;
            actions.SwitchState(actions.Swap);
        }
    }

    private bool CanReload(ActionStateManager action)
    {
        if (action.ammo.currentAmmo == action.ammo.clipSize) return false; 
        if (action.ammo.extraAmmo == 0) return false;
        return true;
    }
}
