using UnityEngine;

public class WeaponColliderDamage : MonoBehaviour
{
    private float _damage;
    private float _damageDelay = 0.3f;
    private Collider _triggerCollider;
    private CharacterAnimationParams _animationParams;
    public bool Enabled { get; private set; }

    public void Init(float damage, float damageDelay, CharacterAnimationParams animationParams)
    {
        _damage = damage;
        _damageDelay = damageDelay;
        _animationParams = animationParams;
        _triggerCollider = GetComponent<Collider>();
        Enabled = true;
    }

    public void Enable(bool value)
    {
        Enabled = value;
    }
    
    private void OnTriggerEnter(Collider other)
    {
        if (!Enabled)
        {
            return;
        }

        if (!_triggerCollider.enabled || _animationParams.OneShotPlayedValue < 1)
        {
            return;
        }

        if (transform.IsChildOf(other.transform))
        {
            return;
        }

        if (other.TryGetComponent(out IDamageable damageable))
        {
            damageable.TakeDamage(_damage, _animationParams.AnimationType);
            _triggerCollider.enabled = false; 
            Invoke(nameof(ResetCollider), _damageDelay); 
        }
    }

    private void ResetCollider()
    {
        _triggerCollider.enabled = true;
    }
}
