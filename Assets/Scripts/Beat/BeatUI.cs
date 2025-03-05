using System.Collections;
using System.Collections.Generic;
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

    [SerializeField] private float _step;

    private void Awake()
    {
        _beatComp = GetComponent<BeatComponent>();
    }

    private void Start()
    {
        // BeatController.Instance.OnMusicStarted += OnMusicStarted;
        _beatComp.OnBeatAction += SpawnIcon;
    }

    private void OnDestroy()
    {
        // BeatController.Instance.OnMusicStarted -= OnMusicStarted;
        _beatComp.OnBeatAction -= SpawnIcon;
    }

    private void SpawnIcon()
    {
        // Criar ícones nos dois lados
        GameObject leftIcon = Instantiate(_iconPrefab, _leftSpawnPoint.position, Quaternion.identity, _container);
        GameObject rightIcon = Instantiate(_iconPrefab, _rightSpawnPoint.position, Quaternion.identity, _container);

        // Move os ícones em direção ao centro
        // StartCoroutine(MoveIcon(leftIcon, _leftSpawnPoint.position, _centerPoint.position));
        // StartCoroutine(MoveIcon(rightIcon, _rightSpawnPoint.position, _centerPoint.position));

        leftIcon.transform.DOMoveX(_centerPoint.position.x, _moveDuration).SetEase(Ease.Linear).OnComplete(() => Destroy(leftIcon));
        rightIcon.transform.DOMoveX(_centerPoint.position.x, _moveDuration).SetEase(Ease.Linear).OnComplete(() => Destroy(rightIcon));
    }

    private IEnumerator MoveIcon(GameObject icon, Vector3 startPos, Vector3 endPos)
    {
        float elapsedTime = 0;
        while (elapsedTime < _moveDuration)
        {
            icon.transform.position = Vector3.Lerp(startPos, endPos, elapsedTime / _moveDuration);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        icon.transform.position = endPos;

        // Remove o ícone após se encontrar no centro
        Destroy(icon);
    }
}
