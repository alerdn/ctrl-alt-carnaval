using DG.Tweening;
using UnityEngine;

public class BeatUI : MonoBehaviour
{
    [SerializeField] private GameObject _iconPrefab; // Prefab do ícone que se moverá
    [SerializeField] private Transform _container;
    [SerializeField] private Transform _leftSpawnPoint; // Ponto de spawn da esquerda
    [SerializeField] private Transform _rightSpawnPoint; // Ponto de spawn da direita
    [SerializeField] private Transform _centerPoint; // Ponto onde os ícones se encontram
    [SerializeField] private float _moveDuration = 3.0f; // Tempo que leva para o ícone chegar ao centro
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
