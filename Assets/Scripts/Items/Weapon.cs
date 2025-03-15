using System.Collections.Generic;
using Unity.Burst;
using UnityEngine;

public class Weapon : MonoBehaviour, IItem
{
    [field: SerializeField] public List<BoneTransformSlot> BoneTransformSlots { get; private set; }
    [field: SerializeField] public AnimationTypes.Type AnimationType { get; private set; } = AnimationTypes.Type.None;
    [field: SerializeField] public Rigidbody WeaponRigidbody { get; private set; }

    [BurstCompile]
    public void Equip(BonesCollector bonesCollector, int slotIndex)
    {
        if (BoneTransformSlots.Count <= slotIndex)
        {
            return;
        }
        BoneTransformSlots[slotIndex].BakeToParent(bonesCollector, transform);
    }

    [BurstCompile]
    public void UnEquip(BonesCollector bonesCollector, int slotIndex)
    {
        
    }

    [BurstCompile]
    public void Pickup()
    {
        WeaponRigidbody.isKinematic = true;
    }

    [BurstCompile]
    public void Drop()
    {
        WeaponRigidbody.isKinematic = false;
    }
}
