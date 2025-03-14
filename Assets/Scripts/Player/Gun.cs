using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.Pool;

public class Gun : MonoBehaviour
{
    public int InitialDamage => _initialDamage;

    // Power Ups
    public bool FireTriple;
    public bool FireBack;
    public bool FireHeal;
    public bool FireMultiple;

    public DamageData Damage;

    [Header("Dialogue")]
    [SerializeField] private DialogueUI _chatUI;
    [SerializeField] private List<string> _successTexts;
    [SerializeField] private List<string> _failTexts;

    [Header("Settings")]
    [SerializeField] private Transform ShootingPoint;
    [SerializeField] private Bullet _bulletPrefab;
    [SerializeField] private int _initialDamage;
    [SerializeField] private Vector2 _damageRange;
    [SerializeField] private float _fireFailCooldown;
    [SerializeField] private float _range;
    [SerializeField] private float _followSpeed;
    [SerializeField] private LayerMask GroundMask;

    private PlayerStateMachine _player;
    private IObjectPool<Bullet> _bulletPool;
    private int _maxPoolSize = 20;

    private Camera _mainCamera;
    private float _startYPostion;
    private float _fireCooldown;

    private void Start()
    {
        _mainCamera = Camera.main;
        _startYPostion = transform.position.y;
        Damage = new DamageData() { Damage = _initialDamage, AttackPower = 1 };

        transform.DOMoveY(_startYPostion + .1f, 1f).From(_startYPostion - .1f).SetLoops(-1, LoopType.Yoyo);

        _bulletPool = new LinkedPool<Bullet>(OnCreateBullet, OnTakeFromPool, OnReturnToPool, OnDestroyBullet, true, _maxPoolSize);
    }

    private void OnDestroy()
    {
        _player.InputReader.FireEvent -= Fire;
        _player.InputReader.AimEvent -= Aim;
    }

    private void Update()
    {
        MoveToPlayer();
        // Aim();

        if (_fireCooldown > 0)
        {
            _fireCooldown -= Time.deltaTime;
        }
    }

    public void Init(PlayerStateMachine player)
    {
        _player = player;
        _player.InputReader.FireEvent += Fire;
        _player.InputReader.AimEvent += Aim;
    }

    public DamageData GetDamage()
    {
        int modifier = Mathf.RoundToInt(Random.Range(_damageRange.x, _damageRange.y));
        return new DamageData() { Damage = Damage.Damage + modifier, AttackPower = Damage.AttackPower };
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
        bullet.Init(GetDamage());
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

    private void Aim(Vector2 aimPosition, bool isGamepad)
    {
        if (Time.timeScale == 0) return;

        var (success, position) = GetAimPosition(aimPosition, isGamepad);
        if (success)
        {
            var direction = position - transform.position;
            direction.y = 0;

            transform.forward = direction;
        }
    }

    private (bool success, Vector3 position) GetAimPosition(Vector2 aimPosition, bool isGamepad)
    {
        if (isGamepad)
        {
            Cursor.visible = false;
            return (success: true, position: transform.position + new Vector3(aimPosition.x, 0, aimPosition.y) * 10);
        }
        else
        {
            Cursor.visible = true;
            var ray = _mainCamera.ScreenPointToRay(aimPosition);

            if (Physics.Raycast(ray, out var hitInfo, Mathf.Infinity, GroundMask))
            {
                return (success: true, position: hitInfo.point);
            }
            else
            {
                return (success: false, position: Vector3.zero);
            }
        }
    }

    private void Fire()
    {
        if (_fireCooldown > 0) return;

        bool withinBeatWindow = _player.IsWithinBeatWindow();
        if (!withinBeatWindow)
        {
            _fireCooldown = _fireFailCooldown;
            _chatUI.ShowChatText(_failTexts.GetRandom());
        }

        List<Bullet> bullets = new();

        Bullet bullet = _bulletPool.Get();
        bullet.Fire(withinBeatWindow, ShootingPoint.position, transform.rotation);

        bullets.Add(bullet);

        if (withinBeatWindow)
        {
            if (Random.Range(0, 10) <= 2) _chatUI.ShowChatText(_successTexts.GetRandom());

            if (FireTriple)
            {
                float angleOffset = 20f;

                for (int i = -1; i <= 1; i += 2)
                {
                    Bullet extraBullet = _bulletPool.Get();
                    Quaternion rotation = Quaternion.Euler(transform.eulerAngles + new Vector3(0, i * angleOffset, 0));
                    extraBullet.Fire(withinBeatWindow, ShootingPoint.position, rotation);

                    bullets.Add(extraBullet);
                }
            }

            if (FireBack)
            {
                Bullet extraBullet = _bulletPool.Get();
                Quaternion rotation = Quaternion.Euler(transform.eulerAngles + new Vector3(0, 180, 0));
                extraBullet.Fire(withinBeatWindow, ShootingPoint.position, rotation);

                bullets.Add(extraBullet);
            }

            if (FireMultiple)
            {
                List<Bullet> extraBullets = new List<Bullet>();
                for (int i = 0; i < bullets.Count; i++)
                {
                    Bullet extraBullet = _bulletPool.Get();
                    extraBullet.Fire(withinBeatWindow, ShootingPoint.position, bullets[i].transform.rotation, .6f);

                    extraBullets.Add(extraBullet);
                }

                bullets.AddRange(extraBullets);
            }

            if (FireHeal)
            {
                foreach (Bullet b in bullets)
                {
                    b.Heal = true;
                }
            }
        }

        AudioManager.Instance.PlayCue("PlayerAttack");
    }
}