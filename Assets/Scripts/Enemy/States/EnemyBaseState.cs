
public abstract class EnemyBaseState
{
    public abstract void EnterState(EnemyController controller);

    public abstract void UpdateState(EnemyController controller);

    public abstract void ExitState(EnemyController controller, EnemyBaseState stateToSwitch);
}




