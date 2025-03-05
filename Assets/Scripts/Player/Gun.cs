using System;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using UnityEngine;
using UnityEngine.Pool;

public class Gun : BeatReactive
{
    public event Action<bool> OnFireEvent;

    [SerializeField] private PlayerStateMachine _player;
    [SerializeField] private Transform ShootingPoint;
    [SerializeField] private Bullet _bulletPrefab;
    [SerializeField] private int _damage;
    [SerializeField] private float _range;
    [SerializeField] private float _followSpeed;
    [SerializeField] private LayerMask GroundMask;
    [SerializeField] private float _beatThreshold;

    [Header("Debug")]
    [SerializeField] private bool _withinBeatWindow;

    private IObjectPool<Bullet> _bulletPool;
    private int _maxPoolSize = 20;

    private Camera _mainCamera;
    private float _startYPostion;
    private float _lastBeatTime;

    private void Start()
    {
        _mainCamera = Camera.main;
        _startYPostion = transform.position.y;

        transform.DOMoveY(_startYPostion + .1f, 1f).From(_startYPostion - .1f).SetLoops(-1, LoopType.Yoyo);

        _player.InputReader.OnFireEvent += Fire;

        _bulletPool = new LinkedPool<Bullet>(OnCreateBullet, OnTakeFromPool, OnReturnToPool, OnDestroyBullet, true, _maxPoolSize);
    }

    private void OnDestroy()
    {
        _player.InputReader.OnFireEvent -= Fire;
    }

    private void Update()
    {
        MoveToPlayer();
        Aim();
    }

    #region Bullet Pool

    private Bullet OnCreateBullet()
    {
        Bullet bullet = Instantiate(_bulletPrefab, ShootingPoint.position, Quaternion.identity);
        bullet.gameObject.SetActive(false);

        bullet.Init(_damage, _bulletPool);

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

    #region Beat

    public override void OnBeat()
    {
        _lastBeatTime = Time.time;
    }

    #endregion

    private void MoveToPlayer()
    {
        float distance = Vector3.Distance(transform.position, _player.transform.position);
        if (distance > _range)
        {
            Vector3 targetPosition = new Vector3(_player.transform.position.x, transform.position.y, _player.transform.position.z);
            transform.position = Vector3.MoveTowards(transform.position, targetPosition, distance * _followSpeed * Time.deltaTime);
        }
    }

    private void Aim()
    {
        var (success, position) = GetMousePosition();
        if (success)
        {
            var direction = position - transform.position;
            direction.y = 0;

            transform.forward = direction;
        }
    }

    private (bool success, Vector3 position) GetMousePosition()
    {
        var ray = _mainCamera.ScreenPointToRay(Input.mousePosition);

        if (Physics.Raycast(ray, out var hitInfo, Mathf.Infinity, GroundMask))
        {
            return (success: true, position: hitInfo.point);
        }
        else
        {
            return (success: false, position: Vector3.zero);
        }
    }

    private void Fire()
    {
        bool preTime = Time.time >= _lastBeatTime + 1 - _beatThreshold;
        bool postTime = Time.time <= _lastBeatTime + _beatThreshold;

        _withinBeatWindow = preTime || postTime;
        OnFireEvent?.Invoke(_withinBeatWindow);

        Bullet bullet = _bulletPool.Get();
        bullet.Fire(_withinBeatWindow, ShootingPoint.position, transform.rotation);
        AudioManager.Instance.PlayCue("PlayerAttack");
    }
}