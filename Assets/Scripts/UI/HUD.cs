using System;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class HUD : MonoBehaviour
{
    [SerializeField] private Image _centerImage;
    [SerializeField] private Gun _gun;
    [SerializeField] private Image _lifeBarAux;
    [SerializeField] private Image _lifeBar;
    [SerializeField] private Image _comboBar;
    [SerializeField] private TMP_Text _comboText;

    private Tween _lifeBarTween;
    [SerializeField] private float _comboPoints;
    private string[] _comboIcons = new string[] { "", "E", "D", "C", "B", "A", "S" };
    private int _currentComboIndex = 0;

    private void Start()
    {
        _comboPoints = 0;
        _comboBar.fillAmount = 0;
        _currentComboIndex = 0;
        _comboText.text = _comboIcons[_currentComboIndex];

        PlayerStateMachine.Instance.OnBeatAction += VerifyBeat;
        PlayerStateMachine.Instance.Health.OnHealthChanged += UpdateLifeBar;

        UpdateLifeBar(PlayerStateMachine.Instance.Health.CurrentHealth, PlayerStateMachine.Instance.Health.CurrentMaxHealth);
    }

    private void OnDestroy()
    {
        PlayerStateMachine.Instance.OnBeatAction -= VerifyBeat;
        PlayerStateMachine.Instance.Health.OnHealthChanged -= UpdateLifeBar;
    }

    private void UpdateLifeBar(int currentHP, int maxHP)
    {
        _lifeBar.fillAmount = (float)currentHP / maxHP;

        _lifeBarTween?.Kill();
        _lifeBarTween = _lifeBarAux.DOFillAmount((float)currentHP / maxHP, .5f).SetDelay(1f);
    }

    private void VerifyBeat(bool success)
    {
        if (success)
        {
            _comboPoints = Mathf.Min(_comboPoints + 1, 5);

            if (_comboPoints == 5)
            {
                _currentComboIndex = Mathf.Clamp(_currentComboIndex + 1, 0, _comboIcons.Length - 1);
                _comboText.text = _comboIcons[_currentComboIndex];

                _comboPoints = 0;
            }
        }
        else
        {
            _comboPoints = Mathf.Max(_comboPoints - 1, 0);

            if (_comboPoints == 0)
            {
                _currentComboIndex = Mathf.Clamp(_currentComboIndex - 1, 0, _comboIcons.Length - 1);
                _comboText.text = _comboIcons[_currentComboIndex];

                if (_currentComboIndex == 0)
                    _comboPoints = 0;
                else _comboPoints = 4;
            }

        }

        _comboBar.fillAmount = _comboPoints / 5f;
    }
}
