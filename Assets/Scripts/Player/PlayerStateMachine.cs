using System;
using DG.Tweening;
using UnityEngine;
using UnityEngine.Events;

public class PlayerStateMachine : StateMachine
{
    public static PlayerStateMachine Instance { get; private set; }

    public UnityAction<bool> OnBeatAction;

    [field: SerializeField] public InputReader InputReader { get; private set; }

    [field: SerializeField] public CharacterController Controller { get; private set; }
    [field: SerializeField] public Animator Animator { get; private set; }
    [field: SerializeField] public Health Health { get; private set; }
    [field: SerializeField] public Renderer Renderer { get; private set; }
    [field: SerializeField] public ForceReceiver ForceReceiver { get; private set; }
    [field: SerializeField] public BeatComponent BeatComp { get; private set; }
    [field: SerializeField] public float BeatThreshold { get; private set; } = .1f;

    [field: SerializeField] public float FreeLookMovement { get; private set; }
    [field: SerializeField] public float RotationDamping { get; private set; }
    [field: SerializeField] public float DodgeDuration { get; private set; }
    [field: SerializeField] public float DodgeLength { get; private set; }
    [field: SerializeField] public float DashCooldown { get; private set; }

    private Tween _hitColorTween;

    public Camera MainCamera { get; private set; }
    public float DashCooldownTimeStamp;
    private float _lastBeatTime;

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        MainCamera = Camera.main;
        InputReader.SetControllerMode(ControllerMode.Gameplay);

        Health.OnTakeDamage += HandleTakeDamage;
        Health.OnDie += HandleDie;
        BeatComp.OnBeatAction += HandleBeat;

        SwitchState(new PlayerFreeLookState(this));
    }

    private void OnDestroy()
    {
        Health.OnTakeDamage -= HandleTakeDamage;
        Health.OnDie -= HandleDie;
        BeatComp.OnBeatAction -= HandleBeat;
    }

    public bool IsWithinBeatWindow()
    {
        float currentTime = Time.time;
        float nextBeatTime = _lastBeatTime + .5f;

        bool preTime = currentTime >= nextBeatTime - BeatThreshold;
        bool postTime = currentTime <= _lastBeatTime + BeatThreshold;

        bool success = preTime || postTime;
        OnBeatAction?.Invoke(success);

        return success;
    }

    private void HandleTakeDamage()
    {
        _hitColorTween?.Kill();
        _hitColorTween = Renderer.material.DOColor(Color.red, "_Color", .1f).SetLoops(2, LoopType.Yoyo);
    }

    private void HandleDie()
    {
        SwitchState(new PlayerDeadState(this));
    }

    private void HandleBeat()
    {
        _lastBeatTime = Time.time;
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