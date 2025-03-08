using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Pool;
using Random = UnityEngine.Random;

[Serializable]
public record WaveConfig
{
    public int WaveNumber;
    public int EnemiesCount;
    public int EnemiesSubWaveCount;
}

public class WaveManager : MonoBehaviour
{
    public UnityAction OnMinuteChanged;

    public TimeSpan CurrentTimeSpan
    {
        get => _timeSpan;
        private set
        {
            if (value.Minutes != _timeSpan.Minutes)
            {
                OnMinuteChanged?.Invoke();
            }

            _timeSpan = value;
        }
    }

    [SerializeField] private PlayerStateMachine _player;
    [SerializeField] private SOString _clock;
    [SerializeField] private List<WaveConfig> _waves;
    [SerializeField] private Transform _enemiesContainer;
    [SerializeField] private EnemyStateMachine _enemyPrefab;

    [Header("Spawn Points")]
    [SerializeField] private float _innerRadius = 1f; // Raio do círculo menor (excluído)
    [SerializeField] private float _outerRadius = 3f; // Raio do círculo maior
    [SerializeField] private Vector2 _xBoudries = new(-25f, 25f);
    [SerializeField] private Vector2 _yBoudries = new(-25f, 25f);
    [SerializeField] private int _maxEnemiesSpawned = 100;

    private List<EnemyStateMachine> _enemies = new();
    private IObjectPool<EnemyStateMachine> _enemyPool;
    private TimeSpan _timeSpan;

    private void Start()
    {
        CurrentTimeSpan = TimeSpan.FromSeconds(0);
        _enemyPool = new LinkedPool<EnemyStateMachine>(OnCreateEnemy, OnTakeFromPool, OnReturnToPool, OnDestroyEnemy, true);

        OnMinuteChanged += SpawnWave;
        SpawnWave();
    }

    private void OnDestroy()
    {
        OnMinuteChanged -= SpawnWave;
    }

    private void Update()
    {
        CurrentTimeSpan += TimeSpan.FromSeconds(Time.deltaTime);
        _clock.Value = $"{CurrentTimeSpan.Minutes:D2}:{CurrentTimeSpan.Seconds:D2}";
    }

    private void SpawnWave()
    {
        _ = SpawnWaveTask();
    }

    private async UniTask SpawnWaveTask()
    {
        await UniTask.Delay(1000);

        WaveConfig wave = _waves.FindLast(wave => CurrentTimeSpan.Minutes >= wave.WaveNumber);
        if (wave != null)
        {
            int enemiesPerSubWave = Mathf.CeilToInt((float)wave.EnemiesCount / wave.EnemiesSubWaveCount);
            for (int i = 0; i < wave.EnemiesSubWaveCount; i++)
            {
                for (int j = 0; j < enemiesPerSubWave; j++)
                {
                    if (_enemies.FindAll(enemy => enemy.isActiveAndEnabled).Count >= _maxEnemiesSpawned) continue;

                    var enemy = _enemyPool.Get();
                    if (!_enemies.Contains(enemy))
                    {
                        _enemies.Add(enemy);
                    }

                    await UniTask.WaitForEndOfFrame();
                }
                await UniTask.Delay(60000 / wave.EnemiesSubWaveCount);
            }
        }
    }

    private Vector3 GetRandomPointInRing()
    {
        // Escolhe um ângulo aleatório
        float angle = Random.Range(0f, Mathf.PI * 2);

        // Escolhe um raio aleatório entre innerRadius e outerRadius
        float radius = Mathf.Sqrt(Random.Range(Mathf.Pow(_innerRadius, 2), Mathf.Pow(_outerRadius, 2)));

        // Converte para coordenadas cartesianas
        Vector3 point = new(Mathf.Cos(angle) * radius, 0f, Mathf.Sin(angle) * radius);
        point += PlayerStateMachine.Instance.transform.position;

        point.x = Mathf.Clamp(point.x, _xBoudries.x, _xBoudries.y);
        point.z = Mathf.Clamp(point.z, _yBoudries.x, _yBoudries.y);

        return point;
    }

    #region Enemy Pool

    private EnemyStateMachine OnCreateEnemy()
    {
        EnemyStateMachine enemy = Instantiate(_enemyPrefab, _enemiesContainer);
        enemy.SetPool(_enemyPool);

        enemy.gameObject.SetActive(false);

        return enemy;
    }

    private void OnTakeFromPool(EnemyStateMachine enemy)
    {
        enemy.Animator.gameObject.SetActive(true);
        enemy.gameObject.SetActive(true);
        enemy.Init(GetRandomPointInRing(), CurrentTimeSpan.Minutes);
    }

    private void OnReturnToPool(EnemyStateMachine enemy)
    {
        enemy.gameObject.SetActive(false);
    }

    private void OnDestroyEnemy(EnemyStateMachine enemy)
    {
        Destroy(enemy.gameObject);
    }

    #endregion

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(_player.transform.position, _innerRadius);

        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(_player.transform.position, _outerRadius);

        Gizmos.color = Color.yellow;
        Gizmos.DrawLine(new Vector3(_xBoudries.x, 0f, _yBoudries.x), new Vector3(_xBoudries.x, 0f, _yBoudries.y));
        Gizmos.DrawLine(new Vector3(_xBoudries.x, 0f, _yBoudries.y), new Vector3(_xBoudries.y, 0f, _yBoudries.y));
        Gizmos.DrawLine(new Vector3(_xBoudries.y, 0f, _yBoudries.y), new Vector3(_xBoudries.y, 0f, _yBoudries.x));
        Gizmos.DrawLine(new Vector3(_xBoudries.y, 0f, _yBoudries.x), new Vector3(_xBoudries.x, 0f, _yBoudries.x));
    }
}