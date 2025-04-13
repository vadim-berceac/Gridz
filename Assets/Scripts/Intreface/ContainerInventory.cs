using System;
using System.Collections.Generic;
using System.Linq;
using Unity.Burst;
using UnityEngine;
using Zenject;

public class ContainerInventory : MonoBehaviour
{
    [field: SerializeField] public GameObject ContainerInventoryWindow { get; private set; }
    [field: SerializeField] public Transform BagTransform { get; private set; }
    public InventoryCell[] ContainerCells { get; private set; }
    public Action OnContainerInventoryOpen;
    public Action OnContainerInventoryClose;

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
        ContainerCells = BagTransform.GetComponentsInChildren<InventoryCell>(includeInactive: true);
    }

    [BurstCompile]
    public void OpenContainer(ItemTargeting itemTargeting)
    {
        _inventory.ClearCells(ContainerCells);
        
        var equipSystem = itemTargeting.Targets.First().GetComponent<EquipmentSystem>();

        var result = new HashSet<IItemData>(equipSystem.WeaponData);
        result.UnionWith(equipSystem.InventoryBag);
        //result.UnionWith(equipSystem.armor); и броню прибавляем когда будет
        
        _inventory.FillCells(ContainerCells, result, equipSystem);
        
        OpenClose(true);
    }
    
    [BurstCompile]
    public void OpenClose(bool value)
    {
        _inventory.Open(value, ref _isOpen, ContainerInventoryWindow, false, _playerInput, Refresh);

        if (value)
        {
            OnContainerInventoryOpen?.Invoke();
        }
        else
        {
            OnContainerInventoryClose?.Invoke();
        }
    }

    [BurstCompile]
    public void SimpleClose()
    {
        _inventory.Open(false, ref _isOpen, ContainerInventoryWindow, false, _playerInput, Refresh);
    }

    private void Refresh()
    {
        // заполнить
        Debug.LogWarning("Refresh");
    }
}
