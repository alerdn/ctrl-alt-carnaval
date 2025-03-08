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
    [field: SerializeField] public HitUI HitUI { get; private set; }
    [field: SerializeField] public EXPCollectable EXPCollectablePrefab { get; private set; }
    [field: SerializeField] public int ExpValue { get; private set; }
    [field: SerializeField] public int InitialDamage { get; private set; }
    [field: SerializeField] public float AttackRange { get; private set; }

    public DamageData Damage { get; private set; }
    public PlayerStateMachine Player { get; private set; }
    public IObjectPool<EXPCollectable> EXPPool { get; private set; }
    public IObjectPool<EnemyStateMachine> EnemyPool { get; private set; }

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

        EXPPool = new LinkedPool<EXPCollectable>(OnCreate, OnTakeFromPool, OnReturnToPool, OnDestroyItem, true);
    }

    public void Init(Vector3 position, int power)
    {
        PowerUp(power);
        transform.position = position;

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

    public void PowerUp(int power)
    {
        int maxHealth = Mathf.RoundToInt((float)Health.InitialMaxHealth * power * 1.5f);
        int defence = Mathf.RoundToInt((float)Health.InitialDefence * power * 2);

        Health.SetMaxHealth(maxHealth);
        Health.RestoreHealth();

        Health.SetDefence(defence);

        Damage = new DamageData { Damage = Mathf.Max(InitialDamage * power * 2, InitialDamage), AttackPower = 1 };
    }

    public void SetPool(IObjectPool<EnemyStateMachine> enemyPool)
    {
        EnemyPool = enemyPool;
    }

    public void SetEXP(float exp)
    {
        ExpValue = Mathf.Max(Mathf.RoundToInt(exp), 1);
    }

    private void HandleTakeDamage(int damage)
    {
        HitUI.ShowHitText(damage.ToString());

        _hitColorTween?.Kill();
        _hitColorTween = Renderer.material.DOColor(Color.red, "_Color", .1f).From(Color.white).SetLoops(2, LoopType.Yoyo);
    }

    private void HandleDie()
    {
        SwitchState(new EnemyDeadState(this));
    }

    #region Pool

    private EXPCollectable OnCreate()
    {
        EXPCollectable exp = Instantiate(EXPCollectablePrefab);
        exp.gameObject.SetActive(false);

        exp.SetPool(EXPPool);

        return exp;
    }

    private void OnTakeFromPool(EXPCollectable exp)
    {
        exp.gameObject.SetActive(true);
        exp.Init(ExpValue, transform.position);
    }

    private void OnReturnToPool(EXPCollectable exp)
    {
        exp.gameObject.SetActive(false);
    }

    private void OnDestroyItem(EXPCollectable exp)
    {
        Destroy(exp.gameObject);
    }

    #endregion

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, AttackRange);
    }
}