using System;
using System.Collections.Generic;
using System.Linq;
using ModestTree;
using Unity.Burst;
using Unity.VisualScripting;
using UnityEngine;

[RequireComponent(typeof(CharacterPersonalityModule))]
[RequireComponent(typeof(Character))]
public class EquipmentModule : MonoBehaviour
{
    public WeaponData[] WeaponData { get; private set; }
    public Transform[] WeaponInstances { get; private set; } = new Transform[3];
    
    private CharacterPersonalityModule _characterPersonalityModule;
    private Character _character;
    public List<IItemData> InventoryBag { get; private set; } = Enumerable.Repeat<IItemData>(null, 25).ToList();

    public event Action OnAnimationChanged;

    [BurstCompile]
    private void Awake()
    {
        _characterPersonalityModule = GetComponent<CharacterPersonalityModule>();
        _character = GetComponent<Character>();
        WeaponData = _characterPersonalityModule.CharacterPersonalityData.WeaponData;

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
    public void DestroyWeaponInstance(int index)
    {
        Destroy(WeaponInstances[index].gameObject);
        WeaponInstances[index] = null;
        OnAnimationChanged?.Invoke();
    }

    [BurstCompile]
    public void CreateWeaponInstance(int index)
    {
        WeaponInstances[index] = SetWeaponData((WeaponData[index]), _character);
        WeaponData[index].Equip(_characterPersonalityModule.BonesCollector, 0, WeaponInstances[index]);
    }
    
    [BurstCompile]
    private static Transform SetWeaponData(WeaponData weaponData, Character animationParamsLayer)
    {
        var result = weaponData.CreateInstance();
        var weaponDamageCollider = result.AddComponent<WeaponColliderDamage>();
        weaponDamageCollider.Init(weaponData.Damage, weaponData.DamageDelay, animationParamsLayer);
        return result;
    }
}
