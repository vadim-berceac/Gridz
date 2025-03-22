using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class EquipmentSystem : MonoBehaviour
{
    [field: Header("Test Weapon")]
    [field: SerializeField] public WeaponData PrimaryWeaponData { get; private set; }

    [field: SerializeField] public WeaponData SecondaryWeaponData { get; private set; }

    [field: Header("Test Armor")]
    [field: SerializeField] public CharacterSkinData PrimaryArmorData { get; private set; }
    
    public List<IItemData> InventoryBag { get; private set; } = new List<IItemData>();
    
    // события вызываемые при назначении в PrimaryWeaponData и SecondaryWeaponData
    // создание Instance - завязать на них
    
    public Transform PrimaryWeaponInstance  {get; private set; }
    public Transform SecondaryWeaponInstance  {get; private set; }
    
    public Transform PrimaryArmorInstance  {get; private set; }

    private void Awake()
    {
        if (PrimaryWeaponData == null)
        {
            return;
        }
        PrimaryWeaponInstance = CreateWeaponInstance(PrimaryWeaponData);
    }

    public AnimationTypes.Type GetAnimationType()
    {
        if (PrimaryWeaponData == null)
        {
            return AnimationTypes.Type.Unarmed;
        }

        return PrimaryWeaponData.AnimationType;
    }

    private static Transform CreateWeaponInstance(WeaponData weaponData)
    {
        var result = weaponData.CreateInstance();
        var weaponDamageCollider = result.AddComponent<WeaponColliderDamage>();
        weaponDamageCollider.Init(weaponData.Damage, weaponData.DamageDelay, weaponData.AnimationType);
        return result;
    }
}
