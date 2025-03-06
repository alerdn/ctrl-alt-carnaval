using DG.Tweening;
using UnityEngine;

public class BeatUI : MonoBehaviour
{
    [SerializeField] private GameObject _iconPrefab;
    [SerializeField] private Transform _container;
    [SerializeField] private Transform _leftSpawnPoint;
    [SerializeField] private Transform _rightSpawnPoint;
    [SerializeField] private Transform _centerPoint;
    [SerializeField] private float _moveDuration = 3.0f;
    private BeatComponent _beatComp;

    private void Awake()
    {
        _beatComp = GetComponent<BeatComponent>();
    }

    private void Start()
    {
        _beatComp.OnBeatAction += SpawnIcon;
    }

    private void OnDestroy()
    {
        _beatComp.OnBeatAction -= SpawnIcon;
    }

    private void SpawnIcon()
    {
        // Criar ícones nos dois lados
        GameObject leftIcon = Instantiate(_iconPrefab, _leftSpawnPoint.position, Quaternion.identity, _container);
        GameObject rightIcon = Instantiate(_iconPrefab, _rightSpawnPoint.position, Quaternion.identity, _container);

        leftIcon.transform.DOMoveX(_centerPoint.position.x, _moveDuration).SetEase(Ease.Linear).OnComplete(() => Destroy(leftIcon));
        rightIcon.transform.DOMoveX(_centerPoint.position.x, _moveDuration).SetEase(Ease.Linear).OnComplete(() => Destroy(rightIcon));
    }
}
