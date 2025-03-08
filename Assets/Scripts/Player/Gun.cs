using DG.Tweening;
using UnityEngine;
using UnityEngine.Pool;

public class Gun : MonoBehaviour
{
    [SerializeField] private Transform ShootingPoint;
    [SerializeField] private Bullet _bulletPrefab;
    [SerializeField] private int _initialDamage;
    [SerializeField] private Vector2 _damageRange;
    [SerializeField] private float _range;
    [SerializeField] private float _followSpeed;
    [SerializeField] private LayerMask GroundMask;

    private int _damage;
    private PlayerStateMachine _player;
    private IObjectPool<Bullet> _bulletPool;
    private int _maxPoolSize = 20;

    private Camera _mainCamera;
    private float _startYPostion;

    private void Start()
    {
        _mainCamera = Camera.main;
        _startYPostion = transform.position.y;
        _damage = _initialDamage;

        transform.DOMoveY(_startYPostion + .1f, 1f).From(_startYPostion - .1f).SetLoops(-1, LoopType.Yoyo);

        _bulletPool = new LinkedPool<Bullet>(OnCreateBullet, OnTakeFromPool, OnReturnToPool, OnDestroyBullet, true, _maxPoolSize);
    }

    private void OnDestroy()
    {
        _player.InputReader.FireEvent -= Fire;
    }

    private void Update()
    {
        MoveToPlayer();
        Aim();
    }

    public void Init(PlayerStateMachine player)
    {
        _player = player;
        _player.InputReader.FireEvent += Fire;
    }

    public void SetDamage(int power)
    {
        _damage = Mathf.Max(_initialDamage * power, _initialDamage);
    }

    #region Bullet Pool

    private Bullet OnCreateBullet()
    {
        Bullet bullet = Instantiate(_bulletPrefab, ShootingPoint.position, Quaternion.identity);
        bullet.gameObject.SetActive(false);

        bullet.SetPool(_bulletPool);

        return bullet;
    }

    private void OnTakeFromPool(Bullet bullet)
    {
        int modifier = Mathf.RoundToInt(Random.Range(_damageRange.x, _damageRange.y));
        bullet.Init(_damage + modifier);
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
        if (Time.timeScale == 0) return;

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
        var ray = _mainCamera.ScreenPointToRay(_player.InputReader.MousePosition);

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
        bool withinBeatWindow = _player.IsWithinBeatWindow();

        Bullet bullet = _bulletPool.Get();
        bullet.Fire(withinBeatWindow, ShootingPoint.position, transform.rotation);

        AudioManager.Instance.PlayCue("PlayerAttack");
    }
}