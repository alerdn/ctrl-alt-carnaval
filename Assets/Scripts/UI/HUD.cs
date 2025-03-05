using System;
using UnityEngine;
using UnityEngine.UI;

public class HUD : MonoBehaviour
{
    [SerializeField] private Image _centerImage;
    [SerializeField] private Gun _gun;
    [SerializeField] private Image _lifeBar;

    private void Start()
    {
        _gun.OnFireEvent += VerifyFire;
        PlayerStateMachine.Instance.Health.OnHealthChanged += UpdateLifeBar;

        UpdateLifeBar(PlayerStateMachine.Instance.Health.CurrentHealth, PlayerStateMachine.Instance.Health.CurrentMaxHealth);
    }

    private void OnDestroy()
    {
        _gun.OnFireEvent -= VerifyFire;
        PlayerStateMachine.Instance.Health.OnHealthChanged -= UpdateLifeBar;
    }

    private void UpdateLifeBar(int currentHP, int maxHP)
    {
        _lifeBar.fillAmount = (float)currentHP / maxHP;
    }

    private void VerifyFire(bool success)
    {
    }
}
