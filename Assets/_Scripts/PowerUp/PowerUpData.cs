using UnityEngine;

public enum PowerUp
{
    Damage10Percent,
    Health10Percent,
    Defence10Percent,
    DashImprovement,
    DashExplosion,
    DashProtection,
    FireTriple,
    FireBack,
    FireHeal,
    FireMultiple,
    DashBombastic
}

[CreateAssetMenu(fileName = "PowerUpData", menuName = "PowerUp")]
public class PowerUPData : ScriptableObject
{
    public PowerUp Type;
    public string Title;
    public string Description;
    public bool IsSpecial;
    public bool IsActive;
    public bool IsSelected;
}