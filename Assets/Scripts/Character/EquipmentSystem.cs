using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class EquipmentSystem : MonoBehaviour
{
    [field: Header("Weapon")]
    [field: SerializeField] public WeaponData[] WeaponData { get; private set; } = new WeaponData [3];

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
        if (WeaponData[0] == null)
        {
            return;
        }
        PrimaryWeaponInstance = CreateWeaponInstance(WeaponData[0]);
    }

    public AnimationTypes.Type GetAnimationType()
    {
        if (WeaponData[0] == null)
        {
            return AnimationTypes.Type.Unarmed;
        }

        return WeaponData[0].AnimationType;
    }

    private static Transform CreateWeaponInstance(WeaponData weaponData)
    {
        var result = weaponData.CreateInstance();
        var weaponDamageCollider = result.AddComponent<WeaponColliderDamage>();
        weaponDamageCollider.Init(weaponData.Damage, weaponData.DamageDelay, weaponData.AnimationType);
        return result;
    }
}
