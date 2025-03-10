using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;

public class Bullet : MonoBehaviour
{
    [SerializeField] private float _lifeTime = 5f;
    [SerializeField] private float _shootForce = 15;
    [SerializeField] private int _maxHits = 5;
    [SerializeField] private string _targetTag;
    [SerializeField] private ParticleSystem _ps;

    private Rigidbody _rb;
    private DamageData _damage;
    private IObjectPool<Bullet> _pool;
    private float _currentLifeTime;
    private List<Collider> _collidersAlreadyHit = new();

    public bool Heal;

    private void Awake()
    {
        _rb = GetComponent<Rigidbody>();
    }

    private void Start()
    {
        _currentLifeTime = _lifeTime;
    }

    private void Update()
    {
        if (!isActiveAndEnabled) return;

        _currentLifeTime -= Time.deltaTime;

        if (_currentLifeTime <= 0f)
        {
            Release();
        }
    }

    public void SetPool(IObjectPool<Bullet> pool)
    {
        _pool = pool;
    }

    public void Init(DamageData damage)
    {
        _damage = damage;
        Heal = false;
    }

    public void Fire(bool onBeat, Vector3 position, Quaternion rotation, float shootForceModifier = 1)
    {
        var mainModule = _ps.main;
        if (onBeat)
        {
            mainModule.startLifetime = 1f;
            _damage.IsCritical = true;
        }
        else
        {
            mainModule.startLifetime = .25f;
        }

        transform.SetPositionAndRotation(position, rotation);

        _rb.AddForce(transform.forward * _shootForce * shootForceModifier, ForceMode.Impulse);
    }

    private void Release()
    {
        _currentLifeTime = _lifeTime;
        _rb.linearVelocity = Vector3.zero;
        _collidersAlreadyHit.Clear();

        _pool.Release(this);
    }

    private void OnTriggerEnter(Collider other)
    {
        bool maxHitsReached = _collidersAlreadyHit.Count >= _maxHits;
        bool alreadyHit = _collidersAlreadyHit.Contains(other);
        bool isTarget = other.CompareTag(_targetTag);

        if (!maxHitsReached && !alreadyHit && isTarget)
        {
            if (other.TryGetComponent(out Health health))
            {
                _collidersAlreadyHit.Add(other);
                health.TakeDamage(_damage);

                if (Heal)
                {
                    PlayerStateMachine.Instance.Health.RestoreHealth(Mathf.RoundToInt((float)PlayerStateMachine.Instance.Health.CurrentMaxHealth * .001f));
                }
            }
        }
    }
}