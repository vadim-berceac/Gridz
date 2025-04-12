using System;
using UnityEngine;

public static class InventoryExtensions
{
    public static void Open(this Inventory inventory, bool value, ref bool open, GameObject window, PlayerInput playerInput, Action action)
    {
        open = value;
        window.SetActive(value);
        if (value)
        {
            action.Invoke();
            playerInput.InputActionMapCharacter.Disable();
            return;
        }
        playerInput.InputActionMapCharacter.Enable();
    }
}
