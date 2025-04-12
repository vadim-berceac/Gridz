using System;
using UnityEngine;
using Zenject;

public class ContainerInventory : MonoBehaviour
{
    [field: SerializeField] public GameObject ContainerInventoryWindow { get; private set; }

    public Action OnContainerInventoryOpen;

    private Inventory _inventory;
    private PlayerInput _playerInput;
    private bool _isOpen;

    [Inject]
    private void Construct(Inventory inventory, PlayerInput playerInput)
    {
        _inventory = inventory;
        _playerInput = playerInput;
    }

    private void Awake()
    {
        _inventory.OnInventoryClosed += OnInventoryClosed;
    }

    private void OnInventoryClosed()
    {
        OpenClose(false);
    }
    
    public void OpenClose(bool value)
    {
        _inventory.Open(value, ref _isOpen, ContainerInventoryWindow, _playerInput, Refresh);
        
        if(value) OnContainerInventoryOpen?.Invoke();
    }

    private void Refresh()
    {
        Debug.LogWarning("Refresh");
    }

    private void OnDisable()
    {
        _inventory.OnInventoryClosed -= OnInventoryClosed;
    }
}
