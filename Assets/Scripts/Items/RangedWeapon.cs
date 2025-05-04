using UnityEngine;

public class RangedWeapon : MonoBehaviour
{
   private WeaponColliderDamage _weaponColliderDamage;
   
   public void SetWeaponCollider(WeaponColliderDamage damage)
   {
      _weaponColliderDamage = damage;
      _weaponColliderDamage.AnimationParamsLayer.CharacterInput.OnAttack += Shoot;
   }

   // подписка почему-то не работает11
   private void Shoot()
   {
      //AudioSource.PlayClipAtPoint(_weaponColliderDamage.SvxSet.GetRandomClip(), transform.position);
      Debug.Log("пыщ");
   }

   private void OnDisable()
   {
      _weaponColliderDamage.AnimationParamsLayer.CharacterInput.OnAttack -= Shoot;
   }
}
