using TMPro;
using UnityEngine;
using UnityEngine.Events;

public class PowerUpUI : MonoBehaviour
{
    public UnityAction<PowerUPData> OnSelect;

    [SerializeField] private PowerUPData _data;
    [SerializeField] private TMP_Text _title;
    [SerializeField] private TMP_Text _description;

    public void Init(PowerUPData data)
    {
        _data = data;
        _title.text = _data.Title;
        _description.text = _data.Description;
    }

    public void OnClick_Select()
    {
        OnSelect?.Invoke(_data);
    }
}