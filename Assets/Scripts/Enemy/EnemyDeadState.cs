using Cysharp.Threading.Tasks;

public class EnemyDeadState : EnemyBaseState
{
    public EnemyDeadState(EnemyStateMachine stateMachine) : base(stateMachine)
    {
    }

    public override void Enter()
    {
        stateMachine.EXPPool.Get();
        stateMachine.Animator.gameObject.SetActive(false);
    }

    public override void Tick(float deltaTime)
    {
    }

    public override void Exit()
    {
    }

    private async UniTask Die()
    {
        await UniTask.Delay(1000);
        stateMachine.EnemyPool.Release(stateMachine);
    }
}