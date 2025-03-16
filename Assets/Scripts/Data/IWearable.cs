using System.Collections.Generic;
using UnityEngine;

public interface IWearable
{
    public List<BoneTransformSlot> BoneTransformSlots { get; }

    public void Equip(BonesCollector bonesCollector, int slotIndex, Transform objectInstance);

    public void UnEquip(BonesCollector bonesCollector, int slotIndex, Transform objectInstance);
}
