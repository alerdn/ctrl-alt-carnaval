using Cysharp.Threading.Tasks;

public class EnemyAttackingState : EnemyBaseState
{
    public EnemyAttackingState(EnemyStateMachine stateMachine) : base(stateMachine)
    {
    }

    public override void Enter()
    {
        stateMachine.BeatComponent.OnBeatAction += Shoot;
    }

    public override void Tick(float deltaTime)
    {
        if (stateMachine.Player.Health.IsDead)
        {
            stateMachine.SwitchState(new EnemyIdleState(stateMachine));
        }

        if (!IsInAttackRange())
        {
            stateMachine.SwitchState(new EnemyChasingState(stateMachine));
        }

        FacePlayer();
    }

    public override void Exit()
    {
        stateMachine.BeatComponent.OnBeatAction -= Shoot;
    }

    private void Shoot()
    {
        Bullet bullet = stateMachine.BulletPool.Get();
        bullet.Fire(stateMachine.ShootingPoint.position, stateMachine.ShootingPoint.rotation);
    }
}