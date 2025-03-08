using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.Pool;

public class HitElement : MonoBehaviour
{
    [SerializeField] private CanvasGroup _canvasGroup;
    [SerializeField] private TMP_Text _text;

    private IObjectPool<HitElement> _hitElementPool;

    public void SetPool(IObjectPool<HitElement> hitElementPool)
    {
        _hitElementPool = hitElementPool;
    }

    public void Init(string text)
    {
        gameObject.SetActive(true);
        _text.text = text;
        
        _canvasGroup.transform.DOPunchScale(Vector3.one * .5f, .1f);
        _canvasGroup.DOFade(1f, .1f).From(0f).SetEase(Ease.InFlash).OnComplete(() =>
        {
            _canvasGroup.DOFade(0f, .25f).SetDelay(.5f).OnComplete(() =>
            {
                _hitElementPool.Release(this);
            });
        });
    }
}