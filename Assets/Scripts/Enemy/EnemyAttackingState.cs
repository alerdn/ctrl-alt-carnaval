using DG.Tweening;
using UnityEngine;

public class EnemyAttackingState : EnemyBaseState
{
    private readonly int IdleHash = Animator.StringToHash("Idle");

    private bool _isCharging;
    private bool _hasCharged;
    private float _chargingCooldown;
    private Vector3 _playerPosition;

    private bool _isPraying;

    public EnemyAttackingState(EnemyStateMachine stateMachine) : base(stateMachine)
    {
        _isCharging = true;
        _hasCharged = false;
        _chargingCooldown = 2f;
        _playerPosition = stateMachine.Player.transform.position;

        _isPraying = false;
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

        switch (stateMachine.Type)
        {
            case EnemyType.Chaser:
                if (!IsInAttackRange())
                {
                    stateMachine.SwitchState(new EnemyChasingState(stateMachine));
                }
                break;
            case EnemyType.Charger:
                if (_isCharging)
                {
                    MoveTo(_playerPosition);

                    if (IsInImpactRange())
                    {
                        _isCharging = false;
                        stateMachine.Player.Health.TakeDamage(stateMachine.Damage);
                    }
                }
                else
                {
                    _chargingCooldown -= deltaTime;
                    if (_chargingCooldown <= 0)
                    {
                        stateMachine.SwitchState(new EnemyChasingState(stateMachine));
                    }
                }
                break;
            case EnemyType.Healer:
                if (!IsInAttackRange())
                {
                    stateMachine.SwitchState(new EnemyChasingState(stateMachine));
                }
                break;
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

        switch (stateMachine.Type)
        {
            case EnemyType.Chaser:
                ChaserAttack();
                break;
            case EnemyType.Charger:
                ChargerAttack();
                break;
            case EnemyType.Healer:
                HealerAttack();
                break;
        }
    }

    private void ChaserAttack()
    {
        AudioManager.Instance.PlayCue("EnemyAttack");
        stateMachine.Animator.CrossFadeInFixedTime("Attack", 0.1f);
        if (IsInAttackRange())
        {
            stateMachine.Player.Health.TakeDamage(stateMachine.Damage);
        }
    }

    private void ChargerAttack()
    {
        if (_hasCharged) return;
        _hasCharged = true;

        AudioManager.Instance.PlayCue("EnemyAttack");
        DOTween.To(() => stateMachine.Agent.speed, x => stateMachine.Agent.speed = x, 150, .25f).SetLoops(2, LoopType.Yoyo).OnComplete(() =>
        {
            stateMachine.Animator.CrossFadeInFixedTime(IdleHash, .1f);
            _isCharging = false;
        });
    }

    private void HealerAttack()
    {
        if (!_isPraying)
        {
            _isPraying = true;
            stateMachine.Animator.CrossFadeInFixedTime("Praying", 0.1f);
        }

        Collider[] allies = Physics.OverlapSphere(stateMachine.transform.position, stateMachine.AttackRange, LayerMask.GetMask("Enemy"));
        foreach (var ally in allies)
        {
            if (ally.TryGetComponent(out EnemyStateMachine enemy))
            {
                if (enemy.Type == EnemyType.Healer) continue;

                enemy.Health.RestoreHealth(Mathf.RoundToInt((float)enemy.Health.CurrentMaxHealth * .5f));
                enemy.HealEffect.Play();
            }
        }
    }
}