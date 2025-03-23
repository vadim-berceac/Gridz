using System.Collections.Generic;
using Unity.Burst;
using UnityEngine;

[CreateAssetMenu(fileName = "WeaponData", menuName = "Scriptable Objects/WeaponData")]
public class WeaponData : ItemData, IWearable, IChangeAnimation
{
    [field: SerializeField] public Transform ModelView  { get; private set; }
    [field: SerializeField] public float Damage { get; private set; }
    [field: SerializeField] public float DamageDelay { get; private set; }
    [field: SerializeField] public List<BoneTransformSlot> BoneTransformSlots { get; private set; }
    [field: SerializeField] public AnimationTypes.Type AnimationType { get; private set; } = AnimationTypes.Type.None;

    [BurstCompile]
    public Transform CreateInstance()
    {
        return Instantiate(ModelView);
    }
   
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
    public override void Drop(IItemData droppedItem)
    {
        
    }

    [BurstCompile]
    public override void Take(IItemData takedItem)
    {
        
    }
}
