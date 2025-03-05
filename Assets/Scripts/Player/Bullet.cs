using System;
using UnityEngine;
using UnityEngine.Pool;

public class Bullet : MonoBehaviour
{
    [SerializeField] private float _lifeTime = 5f;
    [SerializeField] private float _shootForce = 15;
    [SerializeField] private string _targetTag;
    [SerializeField] private ParticleSystem _ps;

    private Rigidbody _rb;
    private int _damage;
    private IObjectPool<Bullet> _pool;
    private float _currentLifeTime;

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

    public void Init(int damage, IObjectPool<Bullet> bulletPool)
    {
        _damage = damage;
        _pool = bulletPool;
    }

    public void Fire(bool onBeat, Vector3 position, Quaternion rotation)
    {
        var mainModule = _ps.main;
        if (onBeat)
        {
            mainModule.startLifetime = 1f;
            _damage *= 2;
        }
        else
        {
            mainModule.startLifetime = .25f;
        }

        transform.SetPositionAndRotation(position, rotation);

        _rb.AddForce(transform.forward * _shootForce, ForceMode.Impulse);
    }

    private void Release()
    {
        _currentLifeTime = _lifeTime;
        _rb.linearVelocity = Vector3.zero;

        _pool.Release(this);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag(_targetTag))
        {
            if (other.TryGetComponent(out Health health))
            {
                health.TakeDamage(_damage);
            }
        }

        if (isActiveAndEnabled) Release();
    }
}