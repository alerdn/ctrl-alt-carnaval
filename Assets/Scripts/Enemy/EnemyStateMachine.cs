using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Pool;

public class EnemyStateMachine : StateMachine
{
    [field: SerializeField] public Animator Animator { get; private set; }
    [field: SerializeField] public CharacterController CharacterController { get; private set; }
    [field: SerializeField] public NavMeshAgent Agent { get; private set; }
    [field: SerializeField] public ForceReceiver ForceReceiver { get; private set; }
    [field: SerializeField] public BeatComponent BeatComponent { get; private set; }
    [field: SerializeField] public Transform ShootingPoint { get; private set; }
    [field: SerializeField] public Bullet BulletPrefab { get; private set; }
    [field: SerializeField] public int AttacksPerBeat { get; private set; }

    [field: SerializeField] public float AttackRange { get; private set; }
    [field: SerializeField] public float MovementSpeed { get; private set; }

    public PlayerStateMachine Player { get; private set; }
    public IObjectPool<Bullet> BulletPool { get; private set; }
    private int _maxPoolSize = 20;

    private void Start()
    {
        Player = PlayerStateMachine.Instance;

        Agent.updatePosition = false;
        Agent.updateRotation = false;

        BulletPool = new LinkedPool<Bullet>(OnCreateBullet, OnTakeFromPool, OnReturnToPool, OnDestroyBullet, true, _maxPoolSize);

        SwitchState(new EnemyChasingState(this));
    }

    #region Bullet Pool

    private Bullet OnCreateBullet()
    {
        Bullet bullet = Instantiate(BulletPrefab, ShootingPoint.position, Quaternion.identity);
        bullet.gameObject.SetActive(false);

        return bullet;
    }

    private void OnTakeFromPool(Bullet bullet)
    {
        bullet.gameObject.SetActive(true);
    }

    private void OnReturnToPool(Bullet bullet)
    {
        bullet.gameObject.SetActive(false);
    }

    private void OnDestroyBullet(Bullet bullet)
    {
        Destroy(bullet.gameObject);
    }

    #endregion

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, AttackRange);
    }
}