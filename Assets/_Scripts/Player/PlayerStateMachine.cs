using System;
using DG.Tweening;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Pool;

public class PlayerStateMachine : StateMachine
{
    public static PlayerStateMachine Instance { get; private set; }

    public UnityAction<bool> OnBeatAction;

    [field: SerializeField] public InputReader InputReader { get; private set; }

    [field: SerializeField] public Gun Gun { get; private set; }
    [field: SerializeField] public CharacterController Controller { get; private set; }
    [field: SerializeField] public Animator Animator { get; private set; }
    [field: SerializeField] public Health Health { get; private set; }
    [field: SerializeField] public Renderer Renderer { get; private set; }
    [field: SerializeField] public ForceReceiver ForceReceiver { get; private set; }
    [field: SerializeField] public BeatComponent BeatComp { get; private set; }
    [field: SerializeField] public float BeatThreshold { get; private set; } = .1f;

    [field: SerializeField] public float FreeLookMovement { get; private set; }
    [field: SerializeField] public float RotationDamping { get; private set; }
    [field: SerializeField] public float DashDuration { get; private set; }
    [field: SerializeField] public float DashLength { get; private set; }
    [field: SerializeField] public float DashCooldown { get; private set; }
    [field: SerializeField] public float DashAbilityCooldown { get; private set; }
    [field: SerializeField] public float DashExplosionRadius { get; private set; }
    [field: SerializeField] public ParticleSystem DashExplosionPS { get; private set; }
    [field: SerializeField] public Bomb BombPrefab { get; private set; }

    private Tween _hitColorTween;

    public Transform MainCameraTransform { get; private set; }
    public IObjectPool<Bomb> BombPool { get; private set; }

    public float DashCooldownTimeStamp;
    public float DashAbilityCooldownTimeStamp;

    // Power Ups
    public bool DashExplosion;
    public bool DashProtection;
    public bool DashBombastic;

    private float _lastBeatTime;

    private void Awake()
    {
        Instance = this;
        MainCameraTransform = Camera.main.transform;
    }

    private void Start()
    {
        InputReader.SetControllerMode(ControllerMode.Gameplay);

        Gun.Init(this);
        BombPool = new LinkedPool<Bomb>(OnCreate, OnTakeFromPool, OnReturnToPool, OnDestroyItem, true, 20);

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

    public void ImproveDash(float amount)
    {
        DashLength += amount;
    }

    private void HandleTakeDamage(DamageData damage)
    {
        _hitColorTween?.Kill();
        _hitColorTween = Renderer.material.DOColor(Color.red, "_Color", .1f).From(Color.white).SetLoops(2, LoopType.Yoyo);
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
        Vector3 forward = MainCameraTransform.forward;
        Vector3 right = MainCameraTransform.right;

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

    public void PowerUp(int level)
    {
        Health.SetMaxHealth(Mathf.RoundToInt((float)Health.InitialMaxHealth * (float)level));
        Health.RestoreHealth(Mathf.RoundToInt((float)Health.CurrentMaxHealth * .1f));

        Gun.Damage.AttackPower = level;
        Health.SetDefence(level);
    }

    public void Win()
    {
        SwitchState(new PlayerWinState(this));
    }

    #region Pool

    private Bomb OnCreate()
    {
        Bomb bomb = Instantiate(BombPrefab);
        bomb.gameObject.SetActive(false);

        return bomb;
    }

    private void OnTakeFromPool(Bomb bomb)
    {
        bomb.transform.position = transform.position;
        bomb.gameObject.SetActive(true);
    }

    private void OnReturnToPool(Bomb bomb)
    {
        bomb.gameObject.SetActive(false);
    }

    private void OnDestroyItem(Bomb bomb)
    {
        Destroy(bomb.gameObject);
    }

    #endregion
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, DashExplosionRadius);
    }
}