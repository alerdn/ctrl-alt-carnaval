using UnityEngine;

public class HealItem : Collectable
{
    [SerializeField] private int _healAmount;

    public override void Collect()
    {
        AudioManager.Instance.PlayCue("Heal");

        PlayerStateMachine.Instance.Health.RestoreHealth(Mathf.RoundToInt((float)PlayerStateMachine.Instance.Health.CurrentMaxHealth * _healAmount / 100f));
        Destroy(gameObject);
    }
}