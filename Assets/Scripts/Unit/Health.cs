using UnityEngine;

public class Health : MonoBehaviour
{
    public delegate void HealthChangeHandler(object source, float odlHealth, float newHealth);
    public event HealthChangeHandler OnHealthChanged;
    public delegate void DeathHandler(object source);
    public event DeathHandler OnDeath;

    private float _maxValue;
    private float _currentValue;
    private float _oldHealth;
    private float _newHealth;
    private bool _isDead;

    public float MaxValue => _maxValue;
    public float CurrentValue => _currentValue;
    public bool IsDead => _isDead;

    public void Init(float maxValue)
    {
       _maxValue = maxValue;
        _currentValue = _maxValue;
    }

    public void ChangeHealth(float amount)
    {
        if(amount == 0 || _isDead)
        {
            return;
        }
        _oldHealth = _currentValue;
        _newHealth = Mathf.Clamp(_currentValue + amount, 0, _maxValue);
        _currentValue = _newHealth;

        OnHealthChanged?.Invoke(this, _oldHealth, _currentValue);

        if (_currentValue == 0)
        {
            _isDead = true;
            OnDeath?.Invoke(this);
        }
    }
}
