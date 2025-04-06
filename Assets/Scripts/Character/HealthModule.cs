using System;
using UnityEngine;

[RequireComponent(typeof(GravitationLayer))]
public class HealthModule : MonoBehaviour, IDamageable
{
    [field: SerializeField] public float MaxHealth { get; private set; }
    public float CurrentHealth { get; private set; }
    private float _normalizedHealth = 1;
    private GravitationLayer _gravitationLayer;
    
    public event Action<AnimationTypes.Type, float> OnDamage;
    public event Action<AnimationTypes.Type, bool> OnDeath;

    private void Awake()
    {
        _gravitationLayer = GetComponent<GravitationLayer>();
        _gravitationLayer.OnFallDamage += OnFallDamage;
        CurrentHealth = MaxHealth;
    }

    private void OnFallDamage(float fallDistance)
    {
        var damage = fallDistance * 4;
        TakeDamage(damage, 0);
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
