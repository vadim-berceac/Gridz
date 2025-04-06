using System.Linq;
using Unity.Burst;
using UnityEngine;

public static class CharacterActionsLayerExtensions
{
    [BurstCompile]
    public static bool TryTakeItem(this CharacterActionsLayer layer, ItemTargeting itemTargeting, EquipmentSystem equipmentSystem)
    {
        var list = itemTargeting.Targets.ToList();
        
        list[0].TryGetComponent<PickupObject>(out var item);

        if (item == null)
        {
            return false;
        }
        
        InventoryCell.FindIndexOfEmpty(equipmentSystem.InventoryBag, out var index);
        
        equipmentSystem.InventoryBag[index] = item.ItemData;

        Debug.LogWarning($"подбираю {item.name}");

        var toRemove = list[0].gameObject;
        
        itemTargeting.Targets.Remove(toRemove.transform);
        
        Object.Destroy(toRemove); 
        
        return true;
    }
}
