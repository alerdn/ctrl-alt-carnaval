using UnityEngine;

public class EnemyIdleState : EnemyBaseState
{
    private readonly int IdleHash = Animator.StringToHash("Idle");

    public EnemyIdleState(EnemyStateMachine stateMachine) : base(stateMachine)
    {
    }

    public override void Enter()
    {
        stateMachine.Animator.CrossFadeInFixedTime(IdleHash, .1f);
    }

    public override void Tick(float deltaTime)
    {
    }

    public override void Exit()
    {
    }
}