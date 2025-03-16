using System.Collections.Generic;
using Unity.Burst;
using UnityEngine;

[CreateAssetMenu(fileName = "WeaponData", menuName = "Scriptable Objects/WeaponData")]
public class WeaponData : ScriptableObject, IItemData, IWearable, IChangeAnimation
{
    [field: SerializeField] public Sprite Icon { get; private set; }
    [field: SerializeField] public Transform View  { get; private set; }
    [field: SerializeField] public List<BoneTransformSlot> BoneTransformSlots { get; private set; }
    [field: SerializeField] public AnimationTypes.Type AnimationType { get; private set; } = AnimationTypes.Type.None;
    
    [BurstCompile]
    public void Equip(BonesCollector bonesCollector, int slotIndex, Transform objectInstance)
    {
        if (BoneTransformSlots.Count <= slotIndex)
        {
            return;
        }
        BoneTransformSlots[slotIndex].BakeToParent(bonesCollector, objectInstance);
    }
    
    [BurstCompile]
    public void UnEquip(BonesCollector bonesCollector, int slotIndex, Transform objectInstance)
    {
        
    }

    [BurstCompile]
    public void Drop(IItemData droppedItem)
    {
        
    }

    [BurstCompile]
    public void Take(IItemData takedItem)
    {
        
    }
}
