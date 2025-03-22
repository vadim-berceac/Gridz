using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class InventoryCell : MonoBehaviour
{
   [field: SerializeField] public Image Icon { get; private set; }
   [field: SerializeField] public Button Button { get; private set; }
   [field: SerializeField] public TMP_Text Text { get; private set; }
   
   private IItemData _item;

   public void SetItem(IItemData item)
   {
      _item = item;
      
      if (item == null)
      {
         Icon.sprite = null;
         Text.SetText("");
         return;
      }
      
      Icon.sprite = item.Icon;
      Text.SetText("1");
   }
}
