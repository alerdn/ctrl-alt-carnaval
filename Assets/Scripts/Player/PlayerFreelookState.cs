using UnityEngine;

public class PlayerFreeLookState : PlayerBaseState
{
    private readonly int FreeLookBlendTreeHash = Animator.StringToHash("FreeLookBlendTree");
    private readonly int FreeLookSpeedHash = Animator.StringToHash("FreeLookSpeed");
    private const float AnimatorDampTime = .075f;

    public PlayerFreeLookState(PlayerStateMachine stateMachine) : base(stateMachine)
    {
    }

    public override void Enter()
    {
        stateMachine.InputReader.DashEvent += Dash;

        stateMachine.Animator.CrossFadeInFixedTime(FreeLookBlendTreeHash, 0.1f);
    }

    public override void Tick(float deltaTime)
    {
        Vector3 movement = stateMachine.CalculeMovement();
        stateMachine.Move(stateMachine.FreeLookMovement * movement, deltaTime);

        if (stateMachine.InputReader.MovementValue == Vector2.zero)
        {
            stateMachine.Animator.SetFloat(FreeLookSpeedHash, 0f, AnimatorDampTime, deltaTime);
            return;
        }

        stateMachine.Animator.SetFloat(FreeLookSpeedHash, 1f, AnimatorDampTime, deltaTime);
        FaceMovementDirection(movement, deltaTime);
    }

    public override void Exit()
    {
        stateMachine.InputReader.DashEvent -= Dash;
    }

    private void Dash()
    {
        if (Time.time < stateMachine.DashCooldownTimeStamp) return;

        Vector2 dashDirection = stateMachine.InputReader.MovementValue;
        if (dashDirection == Vector2.zero) return;

        stateMachine.SwitchState(new PlayerDashingState(stateMachine, dashDirection));

    }
}