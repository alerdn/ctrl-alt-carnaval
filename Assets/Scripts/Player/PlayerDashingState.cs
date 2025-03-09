using System.Linq;
using UnityEngine;

public class PlayerDashingState : PlayerBaseState
{
    private readonly int DashHash = Animator.StringToHash("Dash");

    private Vector3 _dodgingDirection;
    private float _remainingDodgeTime;
    private bool _isWithinBeatWindow;

    public PlayerDashingState(PlayerStateMachine stateMachine, Vector3 dodgingDirection) : base(stateMachine)
    {
        _dodgingDirection = dodgingDirection;
    }

    public override void Enter()
    {
        stateMachine.DashCooldownTimeStamp = Time.time + stateMachine.DashCooldown;
        stateMachine.Health.SetInvulnerable(true);

        _remainingDodgeTime = stateMachine.DashDuration;

        _isWithinBeatWindow = stateMachine.IsWithinBeatWindow();
        if (_isWithinBeatWindow)
        {
            stateMachine.DashCooldownTimeStamp = Time.time + stateMachine.DashCooldown / 2f;
        }

        stateMachine.Animator.CrossFadeInFixedTime(DashHash, .1f);
        AudioManager.Instance.PlayCue("Dash");
    }

    public override void Tick(float deltaTime)
    {
        Vector3 movement = CalculateMovement();

        stateMachine.Move(movement, deltaTime);

        _remainingDodgeTime -= deltaTime;
        if (_remainingDodgeTime <= 0)
        {
            if (_isWithinBeatWindow)
            {
                if (stateMachine.DashExplosion)
                {
                    stateMachine.DashExplosionPS.Play();

                    Physics.OverlapSphere(stateMachine.transform.position, stateMachine.DashExplosionRadius, LayerMask.GetMask("Enemy")).ToList()
                        .ForEach(collider =>
                        {
                            if (collider.TryGetComponent<EnemyStateMachine>(out var enemy))
                            {
                                DamageData damage = stateMachine.Gun.GetDamage();
                                damage.IsCritical = true;
                                enemy.Health.TakeDamage(damage);
                            }
                        });
                }

                if (stateMachine.DashProtection)
                {
                    stateMachine.Health.Shield = 100;
                }

                if (stateMachine.DashBombastic)
                {
                    Object.Instantiate(stateMachine.Bomb, stateMachine.transform.position, Quaternion.identity);
                }
            }

            stateMachine.SwitchState(new PlayerFreeLookState(stateMachine));
        }
    }

    public override void Exit()
    {
        stateMachine.Health.SetInvulnerable(false);
    }

    private Vector3 CalculateMovement()
    {
        Vector3 movement = new Vector3();
        Vector3 forward = stateMachine.MainCameraTransform.forward;
        Vector3 right = stateMachine.MainCameraTransform.right;

        forward.y = 0f;
        right.y = 0f;

        forward.Normalize();
        right.Normalize();

        movement += _dodgingDirection.y * stateMachine.DashLength * forward / stateMachine.DashDuration;
        movement += _dodgingDirection.x * stateMachine.DashLength * right / stateMachine.DashDuration;

        return movement;
    }
}