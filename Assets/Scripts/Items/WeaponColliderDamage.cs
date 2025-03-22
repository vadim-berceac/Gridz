using UnityEngine;

public class WeaponColliderDamage : MonoBehaviour
{
    private float _damage;
    private AnimationTypes.Type _animationType;
    private float _damageDelay = 0.3f;
    private Collider _triggerCollider;

    public void Init(float damage, float damageDelay, AnimationTypes.Type animationType)
    {
        _damage = damage;
        _damageDelay = damageDelay;
        _animationType = animationType;
        _triggerCollider = GetComponent<Collider>();
    }
    
    private void OnTriggerEnter(Collider other)
    {
        if (!_triggerCollider.enabled)
            return;

        if (transform.IsChildOf(other.transform))
            return;

        if (other.TryGetComponent(out IDamageable damageable))
        {
            damageable.TakeDamage(_damage, _animationType);
            _triggerCollider.enabled = false; 
            Invoke(nameof(ResetCollider), _damageDelay); 
        }
    }

    private void ResetCollider()
    {
        _triggerCollider.enabled = true;
    }
}
