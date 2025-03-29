using System;
using UnityEngine;

public class HealthModule : MonoBehaviour, IDamageable
{
    [field: SerializeField] public float MaxHealth { get; private set; }
    [field: SerializeField] public Animator Animator { get; private set; }
    [field: SerializeField] public string AnimatorHitTriggerName { get; private set; }
    [field: SerializeField] public string DeathParamName { get; private set; }
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
        
        Debug.LogWarning($"{name} получил урон {damage} от {hitType}, осталось {CurrentHealth}");
        
        if (!Animator || AnimatorHitTriggerName == null)
        {
            return;
        }
       
        if (damage > 0)
        {
            OnDamage?.Invoke(hitType);
            Animator.SetTrigger(AnimatorHitTriggerName); // перенести ко всем триггерам
        }

        if (CurrentHealth <= 0)
        {
            OnDeath?.Invoke(hitType);
            Animator.SetTrigger(DeathParamName); // перенести ко всем параметрам
        }
    }
}
