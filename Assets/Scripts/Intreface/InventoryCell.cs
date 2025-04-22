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
   [field: SerializeField] public bool IsLootCell { get; private set; }
   
   public IItemData Item { get; private set; }
   public int StackCount { get; private set; }
   private int MaxStackSize { get; set; } = 100;
   private EquipmentModule _equipmentModule;
   private Inventory _inventory;

   [BurstCompile]
   public void SetItem(IItemData item, EquipmentModule equipmentModule, Inventory inventory, int stackCount = 1)
   {
      Item = item;
      _inventory = inventory;
      _equipmentModule = equipmentModule;

      if (item == null)
      {
         Clear();
         return;
      }

      StackCount = Mathf.Clamp(stackCount, 0, MaxStackSize);
      Icon.sprite = item.Icon;
      Text.SetText(StackCount.ToString());
      Button.interactable = true;
   }
   
   [BurstCompile]
   public bool AddToStack(IItemData item, int count = 1)
   {
      if (item == null || count <= 0)
      {
         return false;
      }

      // If cell is empty, initialize it
      if (Item == null)
      {
         SetItem(item, _equipmentModule, _inventory, count);
         return true;
      }

      // Check if item types match
      if (Item != item)
      {
         return false;
      }

      // Check if stack can accommodate additional items
      int newStackCount = StackCount + count;
      if (newStackCount > MaxStackSize)
      {
         return false;
      }

      StackCount = newStackCount;
      Text.SetText(StackCount.ToString());
      return true;
   }

   [BurstCompile]
   public bool RemoveFromStack(int count = 1)
   {
      if (Item == null || count <= 0 || StackCount < count)
      {
         return false;
      }

      StackCount -= count;
      if (StackCount == 0)
      {
         Clear();
      }
      else
      {
         Text.SetText(StackCount.ToString());
      }

      return true;
   }

   [BurstCompile]
   public void OnBagCellClick()
   {
      if (Item == null)
      {
         return;
      }

      if (IsLootCell)
      {
         Debug.LogWarning("Логика передачи из лута в сумку");
         return;
      }

      if (Item is WeaponData)
      {
         if (FindIndexOfEmpty(_equipmentModule.WeaponData, out var result))
         {
            _equipmentModule.WeaponData[result] = Item as WeaponData;
            _equipmentModule.CreateWeaponInstance(result);
            _inventory.WeaponTableCells[result].SetItem(Item, _equipmentModule, _inventory);
            _equipmentModule.InventoryBag[_equipmentModule.InventoryBag.IndexOf(Item)] = null;
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
      
      if (FindIndexOfEmpty(_equipmentModule.InventoryBag, out var result))
      {
         _equipmentModule.InventoryBag[result] = Item;
         _equipmentModule.DestroyWeaponInstance(_equipmentModule.WeaponData.IndexOf(Item));
         _inventory.BagCells[result].SetItem(Item, _equipmentModule, _inventory);
         _equipmentModule.WeaponData[_equipmentModule.WeaponData.IndexOf(Item)] = null;
         Clear();
      }
      Debug.LogWarning(result);
   }

   [BurstCompile]
   public void GetDescription()
   {
      if (Button.interactable == false)
      {
         return;
      }
      _inventory.DescriptionText.text = Item.Description;
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
      _equipmentModule = null;
      _inventory = null;
      Button.interactable = false;
      Text.SetText("");
   }
}
