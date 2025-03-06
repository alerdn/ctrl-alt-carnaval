public class EnemyDeadState : EnemyBaseState
{
    public EnemyDeadState(EnemyStateMachine stateMachine) : base(stateMachine)
    {
    }

    public override void Enter()
    {
        //TODO: Drop XP
        stateMachine.EnemyPool.Release(stateMachine);
    }

    public override void Tick(float deltaTime)
    {
    }

    public override void Exit()
    {
    }
}