using ModestTree;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class InventoryCell : MonoBehaviour
{
   [field: SerializeField] public Image Icon { get; private set; }
   [field: SerializeField] public Button Button { get; private set; }
   [field: SerializeField] public TMP_Text Text { get; private set; }
   
   private IItemData _item;
   private EquipmentSystem _equipmentSystem;

   public void SetItem(IItemData item, EquipmentSystem equipmentSystem)
   {
      _item = item;
      
      if (item == null)
      {
         Icon.sprite = null;
         Text.SetText("");
         _equipmentSystem = null;
         return;
      }
      
      Icon.sprite = item.Icon;
      Text.SetText("1");
      _equipmentSystem = equipmentSystem;
   }

   public void OnClick()
   {
      if (_item == null)
      {
         return;
      }

      if (_item is WeaponData)
      {
         if (FindIndexOfEmpty(out var result))
         {
            _equipmentSystem.WeaponData[result] = _item as WeaponData;
            _equipmentSystem.CreateWeaponInstance(result);
         }
      }
   }

   private bool FindIndexOfEmpty(out int index)
   {
      index = _equipmentSystem.WeaponData.IndexOf(null);
      return index != -1;
   }
}
