using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using Zenject;

public class Inventory : MonoBehaviour
{
    [field: SerializeField] public GameObject InventoryWindow { get; set; }
    [field: SerializeField] public Text DescriptionText { get; private set; }
    [field: SerializeField] private Transform bagTransform;
    [field: SerializeField] private Transform weaponTable;

    private ContainerInventory _containerInventory;
    public bool IsOpen => _isOpen;
    private bool _isOpen;
    
    public InventoryCell[] BagCells { get; private set; }
    public InventoryCell[] WeaponTableCells { get; private set; }

    public Action OnInventoryClosed;
    
    private PlayerInput _playerInput;

    [Inject]
    private void Construct(PlayerInput playerInput, ContainerInventory containerInventory)
    {
        _playerInput = playerInput;
        _containerInventory = containerInventory;
    }
    
    //нужно реализовать отображение стака вещей в одной ячейке (счет не в itemData - потому что то ScriptableObject и общий для всех предметов)

    private void Awake()
    {
        Init();
    }

    private void Init()
    {
        _playerInput.OpenInventory.performed += OnInventoryOpen;
        _containerInventory.OnContainerInventoryOpen += OnContainerInventoryOpen;
        CameraSystem.SelectedCharacterChanged += ReloadInventory;
        BagCells = bagTransform.GetComponentsInChildren<InventoryCell>(includeInactive: true);
        WeaponTableCells = weaponTable.GetComponentsInChildren<InventoryCell>(includeInactive: true);
    }

    private void OnContainerInventoryOpen()
    {
        OpenClose(true);
    }

    private void OnInventoryOpen(InputAction.CallbackContext ctx)
    {
        if (CameraSystem.GetSelectedCharacter() == null)
        {
            OpenClose(false);
            return;
        }
        OpenClose(!_isOpen);
    }

    public void OpenClose(bool value)
    {
        this.Open(value, ref _isOpen, InventoryWindow, _playerInput, Refresh);

        if (!value)
        {
            OnInventoryClosed?.Invoke();
        }
    }

    private void ReloadInventory(CharacterInputLayer characterInputLayer)
    {
        if (!_isOpen || CameraSystem.GetSelectedCharacter() == null)
        {
            return;
        }

        Refresh();
    }

    private void Refresh()
    {
        if ( CameraSystem.GetSelectedCharacter()?.EquipmentSystem == null)
        {
            return;
        }

        DescriptionText.text = "";

        var equipmentSystem = CameraSystem.GetSelectedCharacter().EquipmentSystem;
        
        ClearCells(BagCells);
        ClearCells(WeaponTableCells);
        
        FillCells(BagCells, equipmentSystem.InventoryBag, equipmentSystem);
        
        FillCells(WeaponTableCells, equipmentSystem.WeaponData, equipmentSystem);
    }

    private static void ClearCells(IEnumerable<InventoryCell> cells)
    {
        foreach (var cell in cells)
        {
            cell.Clear();
        }
    }

    private void FillCells(InventoryCell[] cells, IEnumerable<IItemData> items, EquipmentSystem equipmentSystem)
    {
        var cellIndex = 0;

        foreach (var item in items)
        {
            if (cellIndex >= cells.Length) break; 
            if (cells[cellIndex].Item == null)
            {
                cells[cellIndex].SetItem(item, equipmentSystem, this);
            }
            cellIndex++;
        }
    }

    private void OnDisable()
    {
        _playerInput.OpenInventory.performed -= OnInventoryOpen;
        _containerInventory.OnContainerInventoryOpen -= OnContainerInventoryOpen;
        CameraSystem.SelectedCharacterChanged -= ReloadInventory;
    }
}
