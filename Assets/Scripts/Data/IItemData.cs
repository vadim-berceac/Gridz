using UnityEngine;

public interface IItemData
{
    public Sprite Icon { get; }
    public Transform View  { get;}

    public void Drop(IItemData data);
    
    public void Take(IItemData data);
}
