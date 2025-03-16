using Unity.Burst;
using UnityEngine;

public class EquipmentSystem : MonoBehaviour
{
    [field: Header("Test Weapon")]
    [field: SerializeField] public WeaponData PrimaryWeaponData { get; private set; }
    
    [field: Header("Test Armor")]
    [field: SerializeField] public CharacterSkinData PrimaryArmorData { get; private set; }
    
    private Transform _viewInstance;
    
    public void CreateWeaponInstance()
    {
        _viewInstance = Instantiate(PrimaryWeaponData.View); 
    }

    [BurstCompile]
    public void Equip(BonesCollector bonesCollector, int slotIndex)
    {
        if (PrimaryWeaponData.BoneTransformSlots.Count <= slotIndex)
        {
            return;
        }
        PrimaryWeaponData.BoneTransformSlots[slotIndex].BakeToParent(bonesCollector, _viewInstance);
    }

    [BurstCompile]
    public void UnEquip(BonesCollector bonesCollector, int slotIndex)
    {
        
    }

    [BurstCompile]
    public void Drop(WeaponData data)
    {
        
    }
    
    public void Destroy()
    {
        if (_viewInstance != null)
        {
            UnityEngine.Object.Destroy(_viewInstance.gameObject); // Уничтожаем экземпляр при необходимости
        }
    }
    
}
