using DG.Tweening;
using UnityEngine;
using UnityEngine.Pool;

public class BeatUI : MonoBehaviour
{
    [SerializeField] private GameObject _iconPrefab;
    [SerializeField] private Transform _container;
    [SerializeField] private Transform _leftSpawnPoint;
    [SerializeField] private Transform _rightSpawnPoint;
    [SerializeField] private Transform _centerPoint;
    [SerializeField] private float _moveDuration = 3.0f;

    private BeatComponent _beatComp;
    private IObjectPool<GameObject> _iconPool;

    private void Awake()
    {
        _beatComp = GetComponent<BeatComponent>();
    }

    private void Start()
    {
        _beatComp.OnBeatAction += SpawnIcon;

        _iconPool = new LinkedPool<GameObject>(OnCreate, OnTakeFromPool, OnReturnToPool, OnDestroyItem, true, 20);
    }

    private void OnDestroy()
    {
        _beatComp.OnBeatAction -= SpawnIcon;
    }

    private void SpawnIcon()
    {
        // Criar ícones nos dois lados
        GameObject leftIcon = _iconPool.Get();
        leftIcon.transform.SetPositionAndRotation(_leftSpawnPoint.position, Quaternion.identity);

        GameObject rightIcon = _iconPool.Get();
        rightIcon.transform.SetPositionAndRotation(_rightSpawnPoint.position, Quaternion.identity);

        leftIcon.transform.DOMoveX(_centerPoint.position.x, _moveDuration).SetEase(Ease.Linear).OnComplete(() => Destroy(leftIcon));
        rightIcon.transform.DOMoveX(_centerPoint.position.x, _moveDuration).SetEase(Ease.Linear).OnComplete(() => Destroy(rightIcon));
    }

    #region Pool

    private GameObject OnCreate()
    {
        GameObject icon = Instantiate(_iconPrefab, _container);
        icon.SetActive(false);

        return icon;
    }

    private void OnTakeFromPool(GameObject icon)
    {
        icon.SetActive(true);
    }

    private void OnReturnToPool(GameObject icon)
    {
        icon.SetActive(false);
    }

    private void OnDestroyItem(GameObject icon)
    {
        Destroy(icon);
    }

    #endregion
}
