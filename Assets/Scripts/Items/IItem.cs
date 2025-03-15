
public interface IItem
{
    public BoneTransformSlot PrimaryBoneTransformSlot { get; }
    
    public void Equip(BonesCollector bonesCollector);
    
    public void UnEquip(BonesCollector bonesCollector);

    public void Pickup();
    public void Drop();
}
