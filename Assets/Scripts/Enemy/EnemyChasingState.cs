using UnityEngine;

public class EnemyChasingState : EnemyBaseState
{
    private readonly int LocomotionHash = Animator.StringToHash("Locomotion");

    public EnemyChasingState(EnemyStateMachine stateMachine) : base(stateMachine)
    {
    }

    public override void Enter()
    {
        stateMachine.Animator.CrossFadeInFixedTime(LocomotionHash, .1f);
    }

    public override void Tick(float deltaTime)
    {
        if (IsInAttackRange())
        {
            stateMachine.SwitchState(new EnemyAttackingState(stateMachine));
        }

        MoveToPlayer(deltaTime);
        FacePlayer();
    }

    public override void Exit()
    {
        if (stateMachine.Agent.isOnNavMesh)
        {
            stateMachine.Agent.ResetPath();
        }
        stateMachine.Agent.velocity = Vector3.zero;
    }

    private void MoveToPlayer(float deltaTime)
    {
        if (Time.frameCount % 10 != 0) return;

        if (stateMachine.Agent.isOnNavMesh)
        {
            stateMachine.Agent.destination = stateMachine.Player.transform.position;
        }
    }
}