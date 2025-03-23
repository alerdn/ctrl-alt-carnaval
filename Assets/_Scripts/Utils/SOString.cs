using System;
using UnityEngine;

[CreateAssetMenu(fileName = "SOString", menuName = "SO/String")]
public class SOString : ScriptableObject
{
    public event Action<string> OnValueChanged;

    public string Value
    {
        get => _value;
        set
        {
            _value = value;
            OnValueChanged?.Invoke(value);
        }
    }

    [SerializeField] private string _value;
    [SerializeField] private bool _resetOnEnable;

    private void OnEnable()
    {
        if (_resetOnEnable) Value = "";
    }
}