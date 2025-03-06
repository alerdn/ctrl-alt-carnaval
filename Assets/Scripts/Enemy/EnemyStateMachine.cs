using System;
using DG.Tweening;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Pool;

public class EnemyStateMachine : StateMachine
{
    [field: SerializeField] public Animator Animator { get; private set; }
    [field: SerializeField] public NavMeshAgent Agent { get; private set; }
    [field: SerializeField] public Health Health { get; private set; }
    [field: SerializeField] public Renderer Renderer { get; private set; }
    [field: SerializeField] public BeatComponent BeatComponent { get; private set; }
    [field: SerializeField] public int Damage { get; private set; }

    [field: SerializeField] public float AttackRange { get; private set; }
    [field: SerializeField] public float MovementSpeed { get; private set; }

    public PlayerStateMachine Player { get; private set; }
    public IObjectPool<EnemyStateMachine> EnemyPool { get; private set; }

    private int _power;
    private int _initialDamage;
    private Tween _hitColorTween;

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
        Player = PlayerStateMachine.Instance;

        Agent.updatePosition = true;
        Agent.updateRotation = false;

        _initialDamage = Damage;
    }

    public void Init(Vector3 position, int power)
    {
        _power = power;
        Health.SetMaxHealth(power);
        Health.RestoreHealth();

        Damage = GetDamage(power);

        // Resetando posição
        // CharacterController.enabled = false;
        transform.position = position;
        // CharacterController.enabled = true;

        // Resetando agent
        Agent.enabled = false;
        Agent.enabled = true;
        if (Agent.isOnNavMesh)
        {
            Agent.ResetPath();
        }
        Agent.velocity = Vector3.zero;

        SwitchState(new EnemyChasingState(this));
    }

    public void SetPool(IObjectPool<EnemyStateMachine> enemyPool)
    {
        EnemyPool = enemyPool;
    }

    private int GetDamage(int power)
    {
        //TODO: Pensar em uma fórmula melhor
        return Mathf.Max(_initialDamage * power, _initialDamage);
    }

    private void HandleTakeDamage()
    {
        _hitColorTween?.Kill();
        _hitColorTween = Renderer.material.DOColor(Color.red, "_Color", .1f).From(Color.white).SetLoops(2, LoopType.Yoyo);
    }

    private void HandleDie()
    {
        SwitchState(new EnemyDeadState(this));
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, AttackRange);
    }
}