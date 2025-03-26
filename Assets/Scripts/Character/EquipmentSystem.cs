using System.Collections.Generic;
using System.Linq;
using ModestTree;
using Unity.Burst;
using Unity.VisualScripting;
using UnityEngine;

[RequireComponent(typeof(CharacterSkinModule))]
public class EquipmentSystem : MonoBehaviour
{
    [field: Header("Weapon")]
    [field: SerializeField] public WeaponData[] WeaponData { get; private set; } = new WeaponData [3];
    public Transform[] WeaponInstances { get; private set; } = new Transform[3];

    [field: Header("Test Armor")]
    [field: SerializeField] public CharacterSkinData PrimaryArmorData { get; private set; }
    
    private CharacterSkinModule _characterSkinModule;
    public List<IItemData> InventoryBag { get; private set; } = Enumerable.Repeat<IItemData>(null, 25).ToList();

    [BurstCompile]
    private void Awake()
    {
        _characterSkinModule = GetComponent<CharacterSkinModule>();

        foreach (var data in WeaponData)
        {
            if (data != null)
            {
                CreateWeaponInstance(WeaponData.IndexOf(data));
            }
        }
    }

    [BurstCompile]
    public AnimationTypes.Type GetAnimationType(int indexOfWeapon)
    {
        if (WeaponData == null || 
            indexOfWeapon < 0 || 
            indexOfWeapon >= WeaponData.Length || 
            WeaponData[indexOfWeapon] == null)
        {
            return AnimationTypes.Type.Unarmed;
        }

        return WeaponData[indexOfWeapon].AnimationType;
    }

    [BurstCompile]
    public void CreateWeaponInstance(int index)
    {
        WeaponInstances[index] = CreateWeaponInstance((WeaponData[index]));
        WeaponData[index].Equip(_characterSkinModule.BonesCollector, 0, WeaponInstances[index]);
    }

    [BurstCompile]
    public void DestroyWeaponInstance(int index)
    {
        Destroy(WeaponInstances[index].gameObject);
        WeaponInstances[index] = null;
    }

    [BurstCompile]
    private static Transform CreateWeaponInstance(WeaponData weaponData)
    {
        var result = weaponData.CreateInstance();
        var weaponDamageCollider = result.AddComponent<WeaponColliderDamage>();
        weaponDamageCollider.Init(weaponData.Damage, weaponData.DamageDelay, weaponData.AnimationType);
        return result;
    }
}
