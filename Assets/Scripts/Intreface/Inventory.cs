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
    
    public bool IsOpen { get; set; }
    
    public InventoryCell[] BagCells { get; private set; }
    public InventoryCell[] WeaponTableCells { get; private set; }
    
    private PlayerInput _playerInput;

    [Inject]
    private void Construct(PlayerInput playerInput)
    {
        _playerInput = playerInput;
    }
    
    //нужно реализовать отображение стака вещей в одной ячейке (счет не в itemData - потому что то ScriptableObject и общий для всех предметов)

    private void Awake()
    {
        Init();
    }

    private void Init()
    {
        _playerInput.OpenInventory.performed += OnInventoryOpen;
        CameraSystem.SelectedCharacterChanged += ReloadInventory;
        BagCells = bagTransform.GetComponentsInChildren<InventoryCell>(includeInactive: true);
        WeaponTableCells = weaponTable.GetComponentsInChildren<InventoryCell>(includeInactive: true);
    }

    private void OnInventoryOpen(InputAction.CallbackContext ctx)
    {
        if (CameraSystem.GetSelectedCharacter() == null)
        {
            OpenClose(false);
            return;
        }
        OpenClose(!IsOpen);
    }

    public void OpenClose(bool value)
    {
        IsOpen = value;
        InventoryWindow.SetActive(value);
        if (value)
        {
            Refresh();
            _playerInput.InputActionMapCharacter.Disable();
            return;
        }
        _playerInput.InputActionMapCharacter.Enable();
    }

    private void ReloadInventory(LocoMotionLayer locoMotionLayer)
    {
        if (!IsOpen || CameraSystem.GetSelectedCharacter() == null)
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
        CameraSystem.SelectedCharacterChanged -= ReloadInventory;
    }
}
