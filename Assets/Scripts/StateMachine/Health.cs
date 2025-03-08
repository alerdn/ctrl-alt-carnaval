using UnityEngine;
using UnityEngine.Events;

public record DamageData
{
    public int Damage;
    public int AttackPower;
    public bool IsCritical;
}

public class Health : MonoBehaviour
{
    public event UnityAction<int> OnTakeDamage;
    public event UnityAction OnDie;
    public event UnityAction<int, int> OnHealthChanged;
    public event UnityAction<int> OnMaxHealthChanged;

    public bool IsDead => _health == 0;
    public int CurrentMaxHealth
    {
        get => _currentMaxHealth;
        private set
        {
            _currentMaxHealth = value;
            OnMaxHealthChanged?.Invoke(_currentMaxHealth);
        }
    }
    public int InitialMaxHealth => _initialMaxHealth;
    public int CurrentHealth
    {
        get => _health; private set
        {
            if (value == _health) return;

            _health = value;
            OnHealthChanged?.Invoke(_health, CurrentMaxHealth);
        }
    }

    public int InitialDefence => _initialDefence;
    public int CurrentDefence
    {
        get => _defence;
        private set => _defence = value;
    }

    [SerializeField] private int _initialMaxHealth = 100;
    [SerializeField] private int _initialDefence = 1;

    [Header("Debug")]
    [SerializeField] private int _health;
    [SerializeField] private int _currentMaxHealth;
    [SerializeField] private int _defence;

    private bool _isInvulnerable;

    private void Start()
    {
        CurrentMaxHealth = InitialMaxHealth;
        CurrentHealth = CurrentMaxHealth;

        CurrentDefence = _initialDefence;
    }

    public void SetInvulnerable(bool isInvulnerable)
    {
        _isInvulnerable = isInvulnerable;
    }

    public void SetMaxHealth(int maxHealth)
    {
        CurrentMaxHealth = Mathf.Max(maxHealth, InitialMaxHealth);
    }

    public void SetDefence(int defence)
    {
        CurrentDefence = Mathf.Max(defence, _initialDefence);
    }

    public void TakeDamage(DamageData data)
    {
        if (CurrentHealth == 0 || _isInvulnerable) return;

        int damageBase = data.IsCritical ? data.Damage * 2 : data.Damage;
        int damage = Mathf.RoundToInt((float)damageBase * ((float)data.AttackPower / (float)CurrentDefence));

        CurrentHealth = Mathf.Max(CurrentHealth - damage, 0);
        OnTakeDamage?.Invoke(damage);

        if (CurrentHealth == 0)
        {
            OnDie?.Invoke();
        }
    }

    public void RestoreHealth(int heal = 0)
    {
        if (heal == 0) CurrentHealth = CurrentMaxHealth;
        else CurrentHealth = Mathf.Min(CurrentHealth + heal, CurrentMaxHealth);
    }
}