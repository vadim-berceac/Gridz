using UnityEngine;

public class Weapon : MonoBehaviour, IItem
{
    [field: SerializeField] public BoneTransformSlot PrimaryBoneTransformSlot { get; private set; }
    [field: SerializeField] public AnimationTypes.Type AnimationType { get; private set; } = AnimationTypes.Type.None;
    [field: SerializeField] public Rigidbody WeaponRigidbody { get; private set; }

    public void Equip(BonesCollector bonesCollector)
    {
        PrimaryBoneTransformSlot.BakeToParent(bonesCollector, transform);
    }

    public void UnEquip(BonesCollector bonesCollector)
    {
        
    }

    public void Pickup()
    {
        WeaponRigidbody.isKinematic = true;
    }

    public void Drop()
    {
        WeaponRigidbody.isKinematic = false;
    }
}
