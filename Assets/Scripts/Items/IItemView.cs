using System.Collections.Generic;

public interface IItemView
{
    public List<BoneTransformSlot> BoneTransformSlots { get; }
    
    public void Equip(BonesCollector bonesCollector, int slotIndex);
    
    public void UnEquip(BonesCollector bonesCollector, int slotIndex);

    public void Pickup();
    public void Drop();
}
