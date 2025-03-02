using System.Collections;
using UnityEngine;
using Cysharp.Threading.Tasks;
public class EnemyAttackingState : EnemyBaseState
{
    public EnemyAttackingState(EnemyStateMachine stateMachine) : base(stateMachine)
    {
    }

    public override void Enter()
    {
        stateMachine.BeatComponent.OnBeatEvent += Shoot;
    }

    public override void Tick(float deltaTime)
    {
        if (!IsInAttackRange())
        {
            stateMachine.SwitchState(new EnemyChasingState(stateMachine));
        }

        FacePlayer();
    }

    public override void Exit()
    {
        stateMachine.BeatComponent.OnBeatEvent -= Shoot;
    }

    private async void Shoot()
    {
        for (int i = 0; i < stateMachine.AttacksPerBeat; i++)
        {
            Bullet bullet = stateMachine.BulletPool.Get();
            bullet.SetPool(stateMachine.BulletPool);

            bullet.Fire(stateMachine.ShootingPoint.position, stateMachine.ShootingPoint.rotation);
            await UniTask.Delay(100);
        }
    }
}