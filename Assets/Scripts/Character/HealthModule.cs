using System;
using UnityEngine;

public class HealthModule : MonoBehaviour, IDamageable
{
    [field: SerializeField] public float MaxHealth { get; private set; }
    public float CurrentHealth { get; private set; }
    private float _normalizedHealth = 1;
    
    public event Action<AnimationTypes.Type, float> OnDamage;
    public event Action<AnimationTypes.Type, bool> OnDeath;

    private void Awake()
    {
        CurrentHealth = MaxHealth;
    }

    public void TakeDamage(float damage, AnimationTypes.Type hitType)
    {
        if (CurrentHealth <= 0)
        {
            return;
        }
        
        CurrentHealth = Mathf.Clamp(CurrentHealth - damage, 0, MaxHealth);
        _normalizedHealth = GetNormalizedHealth();
        
        OnDamage?.Invoke(hitType, damage);

        if (CurrentHealth <= 0)
        {
            OnDeath?.Invoke(hitType, true);
        }
    }

    public float GetNormalizedHealth()
    {
        return CurrentHealth / MaxHealth;
    }
}
