using UnityEngine;

public class EnemyAttackingState : EnemyBaseState
{
    public EnemyAttackingState(EnemyStateMachine stateMachine) : base(stateMachine)
    {
    }

    public override void Enter()
    {
        stateMachine.BeatComponent.OnBeatAction += Attack;
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
        stateMachine.BeatComponent.OnBeatAction -= Attack;
    }

    private void Attack()
    {
        if (Time.timeScale == 0) return;

        AudioManager.Instance.PlayCue("EnemyAttack");
        stateMachine.Animator.CrossFadeInFixedTime("Attack", 0.1f);
        if (IsInAttackRange())
        {
            stateMachine.Player.Health.TakeDamage(stateMachine.Damage);
        }
    }
}