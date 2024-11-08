using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class ActionBaseState
{
    public abstract void EnterState(ActionStateManager actions);

    public abstract void UpdateState(ActionStateManager actions);
}
