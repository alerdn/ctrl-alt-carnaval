using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using NaughtyAttributes;
using Unity.VisualScripting;
using UnityEngine;

public class PowerUpController : MonoBehaviour
{
    public List<PowerUPData> AllPowerUpsData => _allPowerUpsData;

    [SerializeField] private Transform _frame;
    [SerializeField] private Transform _foreground;
    [SerializeField] private Ease _introEase;
    [SerializeField] private InputReader _inputReader;
    [SerializeField] private List<PowerUpUI> _powerUpsUI;
    [SerializeField] private int _specialPowerUpRate;

    [Header("Debug")]
    [SerializeField] private List<PowerUPData> _allPowerUpsData;

    PlayerStateMachine _player;

    private void Start()
    {
        _player = PlayerStateMachine.Instance;
        ExperienceManager.Instance.OnLevelUp += Show;

        _allPowerUpsData = Resources.LoadAll<PowerUPData>("PowerUps").ToList();
        foreach (PowerUPData powerUp in _allPowerUpsData)
        {
            powerUp.IsActive = false;
        }

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

    public void Show(int level)
    {
        AudioManager.Instance.PlayCue("LevelUp");

        _inputReader.SetControllerMode(ControllerMode.None);
        Time.timeScale = 0;

        _frame.gameObject.SetActive(true);
        _foreground.DOLocalMoveX(0, .75f).From(1920).SetDelay(.5f).SetEase(_introEase).SetUpdate(true);

        List<PowerUPData> powerUps;
        if (level % _specialPowerUpRate == 0)
        {
            powerUps = _allPowerUpsData.FindAll(p => p.IsSpecial == true && p.IsActive == false);
        }
        else
        {
            powerUps = _allPowerUpsData.FindAll(p => p.IsSpecial == false);
        }

        foreach (var powerUpUI in _powerUpsUI)
        {
            if (powerUps.Count == 0)
            {
                powerUps = _allPowerUpsData.FindAll(p => p.IsSpecial == false);
            }
            powerUpUI.Init(powerUps.GetRandom());
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

        powerUp.IsActive = true;

        switch (powerUp.Type)
        {
            case PowerUp.Damage10Percent:
                _player.Gun.Damage = new DamageData()
                {
                    Damage = Mathf.RoundToInt((float)_player.Gun.Damage.Damage * 1.1f),
                    AttackPower = Mathf.RoundToInt((float)_player.Gun.Damage.AttackPower * 1.1f)
                };
                Debug.Log($"Damage increased to {_player.Gun.Damage} and attack to {_player.Gun.Damage.AttackPower}");
                break;
            case PowerUp.Health10Percent:
                _player.Health.SetMaxHealth(Mathf.RoundToInt((float)_player.Health.CurrentMaxHealth * 1.1f));
                _player.Health.RestoreHealth(Mathf.RoundToInt(_player.Health.CurrentMaxHealth * .1f));
                Debug.Log($"Health increased to {_player.Health.CurrentHealth}");
                break;
            case PowerUp.Defence10Percent:
                _player.Health.SetDefence(Mathf.RoundToInt((float)_player.Health.CurrentDefence * 1.1f));
                Debug.Log($"Defence increased to {_player.Health.CurrentDefence}");
                break;
            case PowerUp.DashImprovement:
                _player.ImproveDash(.25f);
                Debug.Log($"Dash improved to {_player.DashLength}");
                break;


            case PowerUp.DashExplosion:
                _player.DashExplosion = true;
                Debug.Log("Dash Explosivo habilitado");
                break;
            case PowerUp.DashProtection:
                _player.DashProtection = true;
                Debug.Log("Dash Protection habilitado");
                break;
            case PowerUp.FireTriple:
                _player.Gun.FireTriple = true;
                Debug.Log("Fire Triple habilitado");
                break;
        }

        Hide();
    }
}