using System.Collections.Generic;
using System.Linq;
using ModestTree;
using TMPro;
using Unity.Burst;
using UnityEngine;
using UnityEngine.UI;

public class InventoryCell : MonoBehaviour
{
   [field: SerializeField] public Image Icon { get; private set; }
   [field: SerializeField] public Button Button { get; private set; }
   [field: SerializeField] public TMP_Text Text { get; private set; }
   
   public IItemData Item { get; private set; }
   private EquipmentSystem _equipmentSystem;
   private Inventory _inventory;

   [BurstCompile]
   public void SetItem(IItemData item, EquipmentSystem equipmentSystem, Inventory inventory)
   {
      Item = item;
      _inventory = inventory;
      
      if (item == null)
      {
         Clear();
         return;
      }
      
      Icon.sprite = item.Icon;
      Text.SetText("1");
      _equipmentSystem = equipmentSystem;
      Button.interactable = true;
   }

   [BurstCompile]
   public void OnBagCellClick()
   {
      if (Item == null)
      {
         return;
      }

      if (Item is WeaponData)
      {
         if (FindIndexOfEmpty(_equipmentSystem.WeaponData, out var result))
         {
            _equipmentSystem.WeaponData[result] = Item as WeaponData;
            _equipmentSystem.CreateWeaponInstance(result);
            _inventory.WeaponTableCells[result].SetItem(Item, _equipmentSystem, _inventory);
            _equipmentSystem.InventoryBag[_equipmentSystem.InventoryBag.IndexOf(Item)] = null;
            Clear();
         }
      }
   }

   [BurstCompile]
   public void OnWeaponTableCellClick()
   {
      if (Item == null)
      {
         return;
      }
      
      if (FindIndexOfEmpty(_equipmentSystem.InventoryBag, out var result))
      {
         _equipmentSystem.InventoryBag[result] = Item;
         _equipmentSystem.DestroyWeaponInstance(_equipmentSystem.WeaponData.IndexOf(Item));
         _inventory.BagCells[result].SetItem(Item, _equipmentSystem, _inventory);
         _equipmentSystem.WeaponData[_equipmentSystem.WeaponData.IndexOf(Item)] = null;
         Clear();
      }
      Debug.LogWarning(result);
   }

   [BurstCompile]
   public static bool FindIndexOfEmpty<T>(IEnumerable<T> collection, out int index)
   {
      index = collection.ToList().IndexOf(default(T));
      return index != -1;
   }

   [BurstCompile]
   public void Clear()
   {
      Icon.sprite = null;
      Item = null;
      _equipmentSystem = null;
      _inventory = null;
      Button.interactable = false;
      Text.SetText("");
   }
}
