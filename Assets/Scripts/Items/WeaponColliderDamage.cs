using System;
using UnityEngine;
using Zenject;

public class WeaponColliderDamage : MonoBehaviour
{
    private float _damage;
    private float _damageDelay = 0.3f;
    private bool _isRanged;
    private Collider _triggerCollider;
    public CharacterAnimationParamsLayer AnimationParamsLayer { get; private set; }
    public bool Enabled { get; private set; }
    public string SvxSetName { get; private set; }
    public SfxSet SvxSet { get; private set; }


    public void Init(float damage, float damageDelay, bool isRanged, CharacterAnimationParamsLayer animationParamsLayer, string svxSetName)
    {
        _damage = damage;
        _damageDelay = damageDelay;
        AnimationParamsLayer = animationParamsLayer;
        _isRanged = isRanged;
        SvxSetName = svxSetName;
        _triggerCollider = GetComponent<Collider>();
        Enabled = true;
        
        if (_isRanged)
        {
            var ranged = gameObject.AddComponent<RangedWeapon>();
            ranged.SetWeaponCollider(this);
        }
    }

    public void Enable(bool value)
    {
        Enabled = value;
    }
    
    private void OnTriggerEnter(Collider other)
    {
        if (!Enabled || _isRanged)
        {
            return;
        }

        if (!_triggerCollider.enabled || AnimationParamsLayer.OneShotPlayedValue < 1)
        {
            return;
        }

        if (transform.IsChildOf(other.transform))
        {
            return;
        }

        if (other.TryGetComponent(out IDamageable damageable))
        {
            damageable.TakeDamage(_damage, AnimationParamsLayer.AnimationType);
            _triggerCollider.enabled = false; 
            Invoke(nameof(ResetCollider), _damageDelay); 
        }
    }

    private void ResetCollider()
    {
        _triggerCollider.enabled = true;
    }
}
