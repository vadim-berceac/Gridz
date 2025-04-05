using UnityEngine;

public class WeaponColliderDamage : MonoBehaviour
{
    private float _damage;
    private float _damageDelay = 0.3f;
    private Collider _triggerCollider;
    private CharacterAnimationParamsLayer _animationParamsLayer;
    public bool Enabled { get; private set; }

    public void Init(float damage, float damageDelay, CharacterAnimationParamsLayer animationParamsLayer)
    {
        _damage = damage;
        _damageDelay = damageDelay;
        _animationParamsLayer = animationParamsLayer;
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

        if (!_triggerCollider.enabled || _animationParamsLayer.OneShotPlayedValue < 1)
        {
            return;
        }

        if (transform.IsChildOf(other.transform))
        {
            return;
        }

        if (other.TryGetComponent(out IDamageable damageable))
        {
            damageable.TakeDamage(_damage, _animationParamsLayer.AnimationType);
            _triggerCollider.enabled = false; 
            Invoke(nameof(ResetCollider), _damageDelay); 
        }
    }

    private void ResetCollider()
    {
        _triggerCollider.enabled = true;
    }
}
