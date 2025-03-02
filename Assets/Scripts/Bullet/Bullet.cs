using System;
using UnityEngine;
using UnityEngine.Pool;

public class Bullet : MonoBehaviour
{
    [SerializeField] private float _lifeTime = 5f;
    [SerializeField] private float _shootForce = 15;
    [SerializeField] private string _targetTag;

    private Rigidbody _rb;
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

    public void Fire(Vector3 position, Quaternion rotation)
    {
        transform.SetPositionAndRotation(position, rotation);

        _rb.AddForce(transform.forward * _shootForce, ForceMode.Impulse);
    }

    public void SetPool(IObjectPool<Bullet> bulletPool)
    {
        _pool = bulletPool;
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
            Debug.Log($"{_targetTag} hit!");
        }

        if (isActiveAndEnabled) Release();
    }
}