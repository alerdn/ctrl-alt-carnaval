using UnityEngine;
using UnityEngine.Pool;

public class DialogueUI : MonoBehaviour
{
    [SerializeField] private Transform _canvas;
    [SerializeField] private DialogueHitElement _hitTextPrefab;
    [SerializeField] private DialogueChatElement _chatTextPrefab;

    private IObjectPool<DialogueElement> _hitPool;
    private IObjectPool<DialogueElement> _chatPool;
    private int _maxPoolSize = 50;

    private void Start()
    {
        if (_hitTextPrefab)
            _hitPool = new LinkedPool<DialogueElement>(OnCreateHitElement, OnTakeFromPool, OnReturnToPool, OnDestroyElement, true, _maxPoolSize);
        if (_chatTextPrefab)
            _chatPool = new LinkedPool<DialogueElement>(OnCreateChatElement, OnTakeFromPool, OnReturnToPool, OnDestroyElement, true, _maxPoolSize);
    }

    public void ShowHitText(DamageData data)
    {
        if (!_hitTextPrefab) return;

        DialogueHitElement hit = _hitPool.Get() as DialogueHitElement;
        hit.Init(data);
    }

    public void ShowChatText(string text)
    {
        if (!_chatTextPrefab) return;

        DialogueChatElement chat = _chatPool.Get() as DialogueChatElement;
        chat.Init(text);
    }

    #region Pool

    private DialogueElement OnCreateHitElement()
    {
        DialogueHitElement hit = Instantiate(_hitTextPrefab, _canvas);
        hit.gameObject.SetActive(false);

        hit.SetPool(_hitPool);

        return hit;
    }

    private DialogueElement OnCreateChatElement()
    {
        DialogueChatElement chat = Instantiate(_chatTextPrefab, _canvas);
        chat.gameObject.SetActive(false);

        chat.SetPool(_chatPool);

        return chat;
    }

    private void OnTakeFromPool(DialogueElement element)
    {
        element.gameObject.SetActive(false);
    }

    private void OnReturnToPool(DialogueElement element)
    {
        element.gameObject.SetActive(false);
    }

    private void OnDestroyElement(DialogueElement element)
    {
        Destroy(element.gameObject);
    }

    #endregion
}