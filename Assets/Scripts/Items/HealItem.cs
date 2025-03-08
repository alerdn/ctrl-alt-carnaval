using UnityEngine;

public class HealItem : Collectable
{
    [SerializeField] private int _healAmount;

    public override void Collect()
    {
        AudioManager.Instance.PlayCue("Heal");

        PlayerStateMachine.Instance.Health.RestoreHealth(_healAmount);
        Destroy(gameObject);
    }
}