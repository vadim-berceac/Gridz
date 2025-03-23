using UnityEngine;

public interface IItemData
{
    public Sprite Icon { get; }
    public Transform GroundView { get; }

    public void Drop(IItemData data);
    
    public void Take(IItemData data);
}
