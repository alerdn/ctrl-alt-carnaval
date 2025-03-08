using UnityEngine;

public enum PowerUp
{
    Damage10Percent,
    Health10Percent,
    Defence10Percent,
    DashImprovement,
}

[CreateAssetMenu(fileName = "PowerUpData", menuName = "PowerUp")]
public class PowerUPData : ScriptableObject
{
    public PowerUp Type;
    public string Title;
    public string Description;
}