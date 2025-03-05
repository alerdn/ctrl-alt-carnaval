using DG.Tweening;
using UnityEngine;

public class PlayerStateMachine : StateMachine
{
    public static PlayerStateMachine Instance { get; private set; }

    [field: SerializeField] public InputReader InputReader { get; private set; }

    [field: SerializeField] public CharacterController Controller { get; private set; }
    [field: SerializeField] public Animator Animator { get; private set; }
    [field: SerializeField] public Health Health { get; private set; }
    [field: SerializeField] public Renderer Renderer { get; private set; }

    [field: SerializeField] public float FreeLookMovement { get; private set; }
    [field: SerializeField] public float RotationDamping { get; private set; }
    [field: SerializeField] public ForceReceiver ForceReceiver { get; private set; }

    private Tween _hitColorTween;

    public Camera MainCamera { get; private set; }

    private void Awake()
    {
        Instance = this;
    }

    private void OnEnable()
    {
        Health.OnTakeDamage += HandleTakeDamage;
        Health.OnDie += HandleDie;
    }

    private void OnDisable()
    {
        Health.OnTakeDamage -= HandleTakeDamage;
        Health.OnDie -= HandleDie;
    }

    private void Start()
    {
        MainCamera = Camera.main;
        InputReader.SetControllerMode(ControllerMode.Gameplay);

        SwitchState(new PlayerFreeLookState(this));
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