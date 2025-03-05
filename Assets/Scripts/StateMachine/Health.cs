using UnityEngine;
using UnityEngine.Events;

public class Health : MonoBehaviour
{
    public event UnityAction OnTakeDamage;
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

    [SerializeField] private int _initialMaxHealth = 100;

    [Header("Debug")]
    [SerializeField] private int _health;
    [SerializeField] private int _currentMaxHealth;

    private bool _isInvulnerable;

    private void Start()
    {
        CurrentMaxHealth = InitialMaxHealth;
        CurrentHealth = CurrentMaxHealth;
    }

    public void SetInvulnerable(bool isInvulnerable)
    {
        _isInvulnerable = isInvulnerable;
    }

    public void TakeDamage(int damage)
    {
        if (CurrentHealth == 0 || _isInvulnerable) return;

        CurrentHealth = Mathf.Max(CurrentHealth - damage, 0);
        OnTakeDamage?.Invoke();

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