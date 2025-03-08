using UnityEngine;

public enum PowerUp
{
    Damage10Percent,
    Health10Percent,
}

[CreateAssetMenu(fileName = "PowerUpData", menuName = "PowerUp")]
public class PowerUPData : ScriptableObject
{
    public PowerUp Type;
    public string Title;
    public string Description;
}