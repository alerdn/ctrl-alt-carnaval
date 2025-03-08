using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class PowerUpController : MonoBehaviour
{
    public List<PowerUPData> AllPowerUpsData => _allPowerUpsData;

    [SerializeField] private Transform _frame;
    [SerializeField] private InputReader _inputReader;
    [SerializeField] private List<PowerUpUI> _powerUpsUI;

    [Header("Debug")]
    [SerializeField] private List<PowerUPData> _allPowerUpsData;

    PlayerStateMachine _player;

    private void Start()
    {
        _player = PlayerStateMachine.Instance;
        ExperienceManager.Instance.OnLevelUp += Show;

        _allPowerUpsData = Resources.LoadAll<PowerUPData>("PowerUps").ToList();

        foreach (var powerUpUI in _powerUpsUI)
        {
            powerUpUI.OnSelect += OnSelect;
        }

        Hide();
    }

    private void OnDestroy()
    {
        ExperienceManager.Instance.OnLevelUp -= Show;

        foreach (var powerUpUI in _powerUpsUI)
        {
            powerUpUI.OnSelect -= OnSelect;
        }
    }

    public void Show()
    {
        _inputReader.SetControllerMode(ControllerMode.None);
        Time.timeScale = 0;

        _frame.gameObject.SetActive(true);

        foreach (var powerUpUI in _powerUpsUI)
        {
            powerUpUI.Init(_allPowerUpsData.GetRandom());
        }
    }

    private void Hide()
    {
        Time.timeScale = 1;
        _frame.gameObject.SetActive(false);
    }

    private void OnSelect(PowerUPData powerUp)
    {
        _inputReader.SetControllerMode(ControllerMode.Gameplay);

        switch (powerUp.Type)
        {
            case PowerUp.Damage10Percent:
                _player.Gun.SetDamage(Mathf.RoundToInt((float)_player.Gun.Damage * 1.1f));
                Debug.Log($"Damage increased to {_player.Gun.Damage}");
                break;
            case PowerUp.Health10Percent:
                _player.Health.SetMaxHealth(Mathf.RoundToInt((float)_player.Health.CurrentMaxHealth * 1.1f));
                _player.Health.RestoreHealth(Mathf.RoundToInt(_player.Health.CurrentMaxHealth * .1f));
                Debug.Log($"Health increased to {_player.Health.CurrentHealth}");
                break;
        }

        Hide();
    }
}