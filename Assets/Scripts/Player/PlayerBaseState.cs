using UnityEngine;

public abstract class PlayerBaseState : State
{
    protected readonly int FreeLookSpeedHash = Animator.StringToHash("FreeLookSpeed");
    protected const float AnimatorDampTime = .075f;

    protected PlayerStateMachine stateMachine;

    public PlayerBaseState(PlayerStateMachine stateMachine)
    {
        this.stateMachine = stateMachine;
    }

    protected void FaceMovementDirection(Vector3 movement, float deltaTime)
    {
        stateMachine.transform.rotation = Quaternion.Lerp(
            stateMachine.transform.rotation,
            Quaternion.LookRotation(movement),
            deltaTime * stateMachine.RotationDamping
        );
    }
}