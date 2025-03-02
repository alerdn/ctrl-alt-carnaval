using System;
using UnityEngine;

public class PlayerStateMachine : StateMachine
{
    [SerializeField] public InputReader InputReader { get; private set; }

    [field: SerializeField] public CharacterController Controller { get; private set; }
    [field: SerializeField] public Animator Animator { get; private set; }
    [field: SerializeField] public float FreeLookMovement { get; private set; }
    [field: SerializeField] public float RotationDamping { get; private set; }
    [field: SerializeField] public ForceReceiver ForceReceiver { get; private set; }

    public Camera MainCamera { get; private set; }

    private void Start()
    {
        MainCamera = Camera.main;
    }

    public float GetNormalizedTime(string tag)
    {
        AnimatorStateInfo currentInfo = Animator.GetCurrentAnimatorStateInfo(0);
        AnimatorStateInfo nextInfo = Animator.GetNextAnimatorStateInfo(0);

        if (Animator.IsInTransition(0) && nextInfo.IsTag(tag))
        {
            return nextInfo.normalizedTime;
        }
        else if (!Animator.IsInTransition(0) && currentInfo.IsTag(tag))
        {
            return currentInfo.normalizedTime;
        }
        else
        {
            return 0f;
        }
    }

    public Vector3 CalculeMovement()
    {
        Vector3 forward = MainCamera.transform.forward;
        Vector3 right = MainCamera.transform.right;

        forward.y = 0f;
        right.y = 0f;

        forward.Normalize();
        right.Normalize();

        return forward * InputReader.MovementValue.y
            + right * InputReader.MovementValue.x;
    }

    public void Move(float deltaTime)
    {
        Move(Vector3.zero, deltaTime);
    }

    public void Move(Vector3 motion, float deltaTime)
    {
        Controller.Move((motion + ForceReceiver.Movement) * deltaTime);
    }
}