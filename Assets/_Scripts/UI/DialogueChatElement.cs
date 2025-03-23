using DG.Tweening;
using TMPro;
using UnityEngine;

public class DialogueChatElement : DialogueElement
{
    [SerializeField] private CanvasGroup _canvasGroup;
    [SerializeField] private TMP_Text _text;

    public void Init(string text)
    {
        gameObject.SetActive(true);
        _text.text = text;

        _canvasGroup.transform.DOPunchScale(Vector3.one * .5f, .1f);
        _canvasGroup.DOFade(1f, .1f).From(0f).SetEase(Ease.InFlash).OnComplete(() =>
        {
            _canvasGroup.DOFade(0f, .25f).SetDelay(.45f).OnComplete(() =>
            {
                elementPool.Release(this);
            });
        });
    }
}