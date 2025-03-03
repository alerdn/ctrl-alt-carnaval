using UnityEngine;

public class EnemyDeadState : EnemyBaseState
{
    public EnemyDeadState(EnemyStateMachine stateMachine) : base(stateMachine)
    {
    }

    public override void Enter()
    {
        Object.Destroy(stateMachine.gameObject);
    }

    public override void Tick(float deltaTime)
    {
    }

    public override void Exit()
    {
    }
}