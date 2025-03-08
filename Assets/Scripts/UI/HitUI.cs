using UnityEngine;
using UnityEngine.Pool;

public class HitUI : MonoBehaviour
{
    [SerializeField] private Transform _canvas;
    [SerializeField] private HitElement _hitTextPrefab;

    private IObjectPool<HitElement> _hitPool;
    private int _maxPoolSize = 50;

    private void Start()
    {
        _hitPool = new LinkedPool<HitElement>(OnCreateHitText, OnTakeFromPool, OnReturnToPool, OnDestroyHitText, true, _maxPoolSize);
    }

    public void ShowHitText(string text)
    {
        HitElement hit = _hitPool.Get();
        hit.Init(text);
    }

    #region Pool

    private HitElement OnCreateHitText()
    {
        HitElement hit = Instantiate(_hitTextPrefab, _canvas);
        hit.gameObject.SetActive(false);

        hit.SetPool(_hitPool);

        return hit;
    }

    private void OnTakeFromPool(HitElement hit)
    {
        hit.gameObject.SetActive(false);
    }

    private void OnReturnToPool(HitElement hit)
    {
        hit.gameObject.SetActive(false);
    }

    private void OnDestroyHitText(HitElement hit)
    {
        Destroy(hit.gameObject);
    }

    #endregion
}