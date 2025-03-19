using System.Collections.Generic;
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
        PrimaryWeaponInstance = PrimaryWeaponData?.CreateInstance();
    }

    public AnimationTypes.Type GetAnimationType()
    {
        if (PrimaryWeaponData == null)
        {
            return AnimationTypes.Type.Unarmed;
        }

        return PrimaryWeaponData.AnimationType;
    }
}
