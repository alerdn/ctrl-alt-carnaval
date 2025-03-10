using System;
using UnityEngine;

[CreateAssetMenu(fileName = "SOFloat", menuName = "SO/Float")]
public class SOFloat : ScriptableObject
{
    public event Action<float> OnValueChanged;

    public float Value
    {
        get => _value;
        set
        {
            _value = value;
            OnValueChanged?.Invoke(value);
        }
    }

    [SerializeField] private float _value;
    [SerializeField] private bool _resetOnEnable;

    private void OnEnable()
    {
        if (_resetOnEnable) Value = 0;
    }
}