using System;
using UnityEngine;

public class HealthModule : MonoBehaviour, IDamageable
{
    [field: SerializeField] public float MaxHealth { get; private set; }
    
    public float CurrentHealth { get; private set; }
    public float NormalizedHealth { get; private set; }
    
    public event Action<AnimationTypes.Type> OnDamage;
    public event Action<AnimationTypes.Type> OnDeath;

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
        NormalizedHealth = CurrentHealth / MaxHealth;

        if (damage < 0)
        {
            OnDamage?.Invoke(hitType);
        }

        if (CurrentHealth <= 0)
        {
            OnDeath?.Invoke(hitType);
        }
        
        Debug.LogWarning($"{name} получил урон {damage} от {hitType}, осталось {CurrentHealth}");
    }
}
