using System;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class HUD : MonoBehaviour
{
    [SerializeField] private Image _centerImage;

    [Header("HP")]
    [SerializeField] private Image _lifeBarAux;
    [SerializeField] private Image _lifeBar;

    [Header("xp")]
    [SerializeField] private Image _expBar;
    [SerializeField] private SOFloat _exp;

    [Header("Combo")]
    [SerializeField] private Image _comboBar;
    [SerializeField] private TMP_Text _comboIcon;
    [SerializeField] private TMP_Text _comboText;

    private Gun _gun;
    private Tween _lifeBarTween;
    [SerializeField] private float _comboPoints;
    private string[] _comboIcons = new string[] { "", "E", "D", "C", "B", "A", "S" };
    private string[] _comboTexts = new string[] { "", "stiloso", "emais!", "abuloso!!", "ota pra\nquebrar!", "rrasando!!", "ensacional!!!" };
    private int _currentComboIndex = 0;
    private float _lastComboTime;

    private void Start()
    {
        _comboPoints = 0;
        _comboBar.fillAmount = 0;
        _currentComboIndex = 0;
        _comboIcon.text = _comboIcons[_currentComboIndex];
        _comboText.text = _comboTexts[_currentComboIndex];

        _gun = PlayerStateMachine.Instance.Gun;
        PlayerStateMachine.Instance.OnBeatAction += VerifyBeat;
        PlayerStateMachine.Instance.Health.OnHealthChanged += UpdateLifeBar;
        _exp.OnValueChanged += UpdateEXPBar;

        UpdateLifeBar(PlayerStateMachine.Instance.Health.CurrentHealth, PlayerStateMachine.Instance.Health.CurrentMaxHealth);
        UpdateEXPBar(_exp.Value);
    }

    private void OnDestroy()
    {
        PlayerStateMachine.Instance.OnBeatAction -= VerifyBeat;
        PlayerStateMachine.Instance.Health.OnHealthChanged -= UpdateLifeBar;
        _exp.OnValueChanged -= UpdateEXPBar;
    }

    private void Update()
    {
        if (Time.time - _lastComboTime > 2.5f)
        {
            _comboPoints = Mathf.Max(_comboPoints - 1, 0);
            UpdateComboBar();
        }
    }

    private void UpdateLifeBar(int currentHP, int maxHP)
    {
        _lifeBar.fillAmount = (float)currentHP / maxHP;

        _lifeBarTween?.Kill();
        _lifeBarTween = _lifeBarAux.DOFillAmount((float)currentHP / maxHP, .5f).SetDelay(1f);
    }

    private void UpdateEXPBar(float exp)
    {
        _expBar.fillAmount = exp / (float)ExperienceManager.Instance.ExperienceToNextLevel;
    }

    private void VerifyBeat(bool success)
    {
        if (success)
        {
            _comboPoints = Mathf.Min(_comboPoints + 1, 5);
            UpdateComboBar();
        }
        else
        {
            _comboPoints = Mathf.Max(_comboPoints - 1, 0);
            UpdateComboBar();
        }
    }

    private void UpdateComboBar()
    {
        if (_comboPoints == 5)
        {
            _currentComboIndex = Mathf.Clamp(_currentComboIndex + 1, 0, _comboIcons.Length - 1);
            _comboPoints = 0;
        }

        else if (_comboPoints == 0)
        {
            _currentComboIndex = Mathf.Clamp(_currentComboIndex - 1, 0, _comboIcons.Length - 1);
            _comboIcon.text = _comboIcons[_currentComboIndex];

            if (_currentComboIndex == 0)
                _comboPoints = 0;
            else _comboPoints = 4;
        }

        _comboIcon.text = _comboIcons[_currentComboIndex];
        _comboText.text = _comboTexts[_currentComboIndex];
        _comboBar.fillAmount = _comboPoints / 5f;

        _lastComboTime = Time.time;
    }
}
