using UnityEngine;

public class ItemData : ScriptableObject, IItemData
{
    [field: SerializeField] public Sprite Icon { get; private set; }
    [field: SerializeField] public Transform GroundView  { get; private set; }
   
    public virtual void Drop(IItemData droppedItem)
    {
        
    }

    public virtual void Take(IItemData takedItem)
    {
        
    }
}
