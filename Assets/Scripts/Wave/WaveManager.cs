using System;
using System.Collections;
using System.Collections.Generic;
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
    public float TotalEXP;
}

[Serializable]
public record EnemyData
{
    public EnemyStateMachine Enemy;
    public int MaxAmount;
    public int CurrentAmount;
}

[Serializable]
public record SpecialEnemyData
{
    public EnemyStateMachine Enemy;
    public int WaveNumber;
    public int MaxAmount;
    public int CurrentAmount;
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
    [SerializeField] private int _lastWave;
    [SerializeField] private List<WaveConfig> _waves;
    [SerializeField] private Transform _enemiesContainer;
    [SerializeField] private List<EnemyData> _enemiesData;
    [SerializeField] private List<SpecialEnemyData> _specialEnemiesData;

    [Header("Spawn Points")]
    [SerializeField] private float _innerRadius = 1f;
    [SerializeField] private float _outerRadius = 3f;
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
        if (CurrentTimeSpan.Minutes >= _lastWave)
        {
            if (_enemies.Find(enemy => enemy.isActiveAndEnabled) == null)
                _clock.Value = "VOCE VENCEU!";
            else _clock.Value = "HORA DA VERDADE!";
        }
        else
        {
            _clock.Value = $"{CurrentTimeSpan.Minutes:D2}:{CurrentTimeSpan.Seconds:D2}";
        }
    }

    private void SpawnWave()
    {
        if (CurrentTimeSpan.Minutes >= _lastWave) return;

        foreach (var enemy in _enemies)
        {
            if (enemy.isActiveAndEnabled)
            {
                enemy.PowerUp(CurrentTimeSpan.Minutes);
            }
        }

        StartCoroutine(SpawnWaveTask());
    }

    private IEnumerator SpawnWaveTask()
    {
        yield return new WaitForSeconds(1f);

        int maxEnemiesAmountToSpawn = Mathf.Max(_maxEnemiesSpawned - _enemies.FindAll(enemy => enemy.isActiveAndEnabled).Count, 0);

        WaveConfig wave = _waves.FindLast(wave => CurrentTimeSpan.Minutes >= wave.WaveNumber);
        if (wave != null)
        {
            int enemiesPerSubWave = Mathf.CeilToInt((float)wave.EnemiesCount / wave.EnemiesSubWaveCount);
            for (int i = 0; i < wave.EnemiesSubWaveCount; i++)
            {
                for (int j = 0; j < enemiesPerSubWave; j++)
                {
                    if (maxEnemiesAmountToSpawn == 0)
                    {
                        break;
                    }

                    var enemy = _enemyPool.Get();
                    maxEnemiesAmountToSpawn--;

                    enemy.SetEXP(wave.TotalEXP / (float)wave.EnemiesCount);

                    if (!_enemies.Contains(enemy))
                    {
                        _enemies.Add(enemy);
                    }

                    yield return new WaitForEndOfFrame();
                }
                yield return new WaitForSeconds(60f / wave.EnemiesSubWaveCount);
            }
        }

        var specialList = _specialEnemiesData.FindAll(data =>
        {
            if (CurrentTimeSpan.Minutes != data.WaveNumber) return false;
            if (data.CurrentAmount >= data.MaxAmount) return false;

            data.CurrentAmount++;
            return true;
        });

        foreach (var special in specialList)
        {
            EnemyStateMachine enemy = Instantiate(special.Enemy, _enemiesContainer);
            enemy.Init(GetRandomPointInRing(), 1);

            yield return new WaitForEndOfFrame();
        }

        if (CurrentTimeSpan.Minutes >= _lastWave && _enemies.Find(enemy => enemy.isActiveAndEnabled) == null)
        {
            PlayerStateMachine.Instance.Win();
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
        var prefab = _enemiesData.GetRandom().Enemy;
        EnemyStateMachine enemy = Instantiate(prefab, _enemiesContainer);
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