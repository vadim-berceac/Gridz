using UnityEngine;

public class WeaponColliderDamage : MonoBehaviour
{
    private float _damage;
    private float _damageDelay = 0.3f;
    private Collider _triggerCollider;
    private Character _character;
    public bool Enabled { get; private set; }

    public void Init(float damage, float damageDelay, Character animationParamsLayer)
    {
        _damage = damage;
        _damageDelay = damageDelay;
        _character = animationParamsLayer;
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

        if (!_triggerCollider.enabled || _character.OneShotPlayedValue < 1)
        {
            return;
        }

        if (transform.IsChildOf(other.transform))
        {
            return;
        }

        if (other.TryGetComponent(out IDamageable damageable))
        {
            damageable.TakeDamage(_damage, _character.AnimationType);
            _triggerCollider.enabled = false; 
            Invoke(nameof(ResetCollider), _damageDelay); 
        }
    }

    private void ResetCollider()
    {
        _triggerCollider.enabled = true;
    }
}
