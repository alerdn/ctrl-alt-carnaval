using UnityEngine;

public class PlayerDashingState : PlayerBaseState
{
    private readonly int DashHash = Animator.StringToHash("Dash");

    private Vector3 _dodgingDirection;
    private float _remainingDodgeTime;

    public PlayerDashingState(PlayerStateMachine stateMachine, Vector3 dodgingDirection) : base(stateMachine)
    {
        _dodgingDirection = dodgingDirection;
    }

    public override void Enter()
    {
        stateMachine.DashCooldownTimeStamp = Time.time + stateMachine.DashCooldown;
        stateMachine.Health.SetInvulnerable(true);
        stateMachine.Controller.excludeLayers = LayerMask.GetMask("Enemy");

        _remainingDodgeTime = stateMachine.DodgeDuration;

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
            stateMachine.SwitchState(new PlayerFreeLookState(stateMachine));
        }
    }

    public override void Exit()
    {
        stateMachine.Health.SetInvulnerable(false);
        stateMachine.Controller.excludeLayers = LayerMask.GetMask("Nothing");
    }

    private Vector3 CalculateMovement()
    {
        Vector3 movement = new Vector3();
        Vector3 forward = stateMachine.MainCamera.transform.forward;
        Vector3 right = stateMachine.MainCamera.transform.right;

        forward.y = 0f;
        right.y = 0f;

        forward.Normalize();
        right.Normalize();

        movement += _dodgingDirection.y * stateMachine.DodgeLength * forward / stateMachine.DodgeDuration;
        movement += _dodgingDirection.x * stateMachine.DodgeLength * right / stateMachine.DodgeDuration;

        return movement;
    }
}