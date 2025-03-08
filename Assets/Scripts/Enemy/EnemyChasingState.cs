using Cysharp.Threading.Tasks;
using UnityEngine;

public class EnemyChasingState : EnemyBaseState
{
    private readonly int LocomotionHash = Animator.StringToHash("Locomotion");
    private bool _recalculatePath;

    public EnemyChasingState(EnemyStateMachine stateMachine) : base(stateMachine)
    {
    }

    public override void Enter()
    {
        _recalculatePath = true;
        stateMachine.Animator.CrossFadeInFixedTime(LocomotionHash, .1f);
    }

    public override void Tick(float deltaTime)
    {
        if (IsInAttackRange())
        {
            stateMachine.SwitchState(new EnemyAttackingState(stateMachine));
        }

        _ = MoveToPlayer(deltaTime);
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

    private async UniTask MoveToPlayer(float deltaTime)
    {
        if (!_recalculatePath) return;

        _recalculatePath = false;
        if (stateMachine.Agent.isOnNavMesh)
        {
            stateMachine.Agent.destination = stateMachine.Player.transform.position;
            await UniTask.Delay(500);
            _recalculatePath = true;
        }
    }
}