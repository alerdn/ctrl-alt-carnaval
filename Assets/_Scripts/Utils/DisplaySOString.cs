using UnityEngine;
using TMPro;

public class DisplaySOString : MonoBehaviour
{
    [SerializeField] private SOString _soString;
    [SerializeField] private TMP_Text _text;

    private void Start()
    {
        _soString.OnValueChanged += OnValueChanged;
        OnValueChanged(_soString.Value);
    }

    private void OnDestroy()
    {
        _soString.OnValueChanged -= OnValueChanged;
    }

    private void OnValueChanged(string value)
    {
        _text.text = value;
    }
}