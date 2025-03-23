using ModestTree;
using TMPro;
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
   }

   public virtual void OnBagCellClick()
   {
      if (Item == null)
      {
         return;
      }

      if (Item is WeaponData)
      {
         if (FindIndexOfEmpty(out var result))
         {
            _equipmentSystem.WeaponData[result] = Item as WeaponData;
            _equipmentSystem.CreateWeaponInstance(result);
            _inventory.WeaponTableCells[result].SetItem(Item, _equipmentSystem, _inventory);
            Clear();
         }
      }
   }

   public virtual void OnWeaponTableCellClick()
   {
      
   }

   private bool FindIndexOfEmpty(out int index)
   {
      index = _equipmentSystem.WeaponData.IndexOf(null);
      return index != -1;
   }

   private void Clear()
   {
      Icon.sprite = null;
      Item = null;
      _equipmentSystem = null;
      Text.SetText("");
   }
}
