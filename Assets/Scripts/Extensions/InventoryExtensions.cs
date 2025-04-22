using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public static class InventoryExtensions
{
    public static void Open(this Inventory inventory, bool value, ref bool open, GameObject window, bool disableInput, PlayerInput playerInput, Action action)
    {
        open = value;
        window.SetActive(value);
        if (value)
        {
            action.Invoke();
            if (disableInput)
            {
                playerInput.InputActionMapCharacter.Disable();
            }
            return;
        }
        playerInput.InputActionMapCharacter.Enable();
    }
    
    // public static void FillCells1(this Inventory inventory, InventoryCell[] cells, IEnumerable<IItemData> items, EquipmentModule equipmentModule)
    // {
    //     var cellIndex = 0;
    //
    //     foreach (var item in items)
    //     {
    //         if (cellIndex >= cells.Length) break; 
    //         if (cells[cellIndex].Item == null)
    //         {
    //             cells[cellIndex].SetItem(item, equipmentModule, inventory);
    //         }
    //         cellIndex++;
    //     }
    // }
    
    public static void FillCells(this Inventory inventory, InventoryCell[] cells, IEnumerable<IItemData> items, EquipmentModule equipmentModule)
    {
        foreach (var item in items) 
        {
            if (item == null)
            {
                continue;
            }

            var stacked = false;
            foreach (var cell in cells)
            {
                if (cell.Item == item && cell.AddToStack(item, 1))
                {
                    stacked = true;
                    break;
                }
            }
            if (stacked)
            {
                return;
            }
            foreach (var cell in cells)
            {
                if (cell.Item == null)
                {
                    cell.SetItem(item, equipmentModule, inventory, 1);
                    break;
                }
            }
        }
    }
    
    public static void ClearCells(this Inventory inventory, IEnumerable<InventoryCell> cells)
    {
        foreach (var cell in cells)
        {
            cell.Clear();
        }
    }
}
