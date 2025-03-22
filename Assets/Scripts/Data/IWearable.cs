using System.Collections.Generic;
using UnityEngine;

public interface IWearable
{
    public Transform ModelView { get; }
    public List<BoneTransformSlot> BoneTransformSlots { get; }
    public Transform CreateInstance();

    public void Equip(BonesCollector bonesCollector, int slotIndex, Transform objectInstance);

    public void UnEquip(BonesCollector bonesCollector, int slotIndex, Transform objectInstance);
}
