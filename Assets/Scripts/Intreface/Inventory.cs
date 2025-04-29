using Unity.Burst;
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

    [BurstCompile]
    private void Init()
    {
        _playerInput.OpenInventory.performed += OnInventoryOpen;
        _containerInventory.OnContainerInventoryOpen += OnContainerInventoryOpen;
        _containerInventory.OnContainerInventoryClose += OnContainerInventoryClose;
        CameraSystem.SelectedCharacterChanged += ReloadInventory;
        InventoryCell.OnClick += Refresh;
        BagCells = bagTransform.GetComponentsInChildren<InventoryCell>(includeInactive: true);
        WeaponTableCells = weaponTable.GetComponentsInChildren<InventoryCell>(includeInactive: true);
    }

    [BurstCompile]
    private void OnContainerInventoryClose()
    {
        OpenClose(false);
    }
    
    [BurstCompile]
    private void OnContainerInventoryOpen()
    {
        OpenClose(true);
    }

    [BurstCompile]
    private void OnInventoryOpen(InputAction.CallbackContext ctx)
    {
        if (CameraSystem.GetSelectedCharacter() == null)
        {
            OpenClose(false);
            return;
        }
        OpenClose(!_isOpen);
    }
    
    [BurstCompile]
    public void OpenClose(bool value)
    {
        this.Open(value, ref _isOpen, InventoryWindow, true, _playerInput, Refresh);

        if (!value)
        {
           _containerInventory.SimpleClose();
        }
    }
    
    [BurstCompile]
    private void ReloadInventory(CharacterInputLayer characterInputLayer)
    {
        if (!_isOpen || CameraSystem.GetSelectedCharacter() == null)
        {
            return;
        }

        Refresh();
    }

    [BurstCompile]
    private void Refresh()
    {
        var equipmentSystem = CameraSystem.GetSelectedCharacter().GetComponent<EquipmentModule>();
        if (equipmentSystem == null)
        {
            return;
        }

        DescriptionText.text = "";
        
        this.ClearCells(BagCells);
        this.ClearCells(WeaponTableCells);
        
        this.FillCells(BagCells, equipmentSystem.InventoryBag, equipmentSystem);
        
        this.FillCells(WeaponTableCells, equipmentSystem.WeaponData, equipmentSystem);
    }

    [BurstCompile]
    private void OnDisable()
    {
        _playerInput.OpenInventory.performed -= OnInventoryOpen;
        _containerInventory.OnContainerInventoryOpen -= OnContainerInventoryOpen;
        _containerInventory.OnContainerInventoryClose -= OnContainerInventoryClose;
        CameraSystem.SelectedCharacterChanged -= ReloadInventory;
        InventoryCell.OnClick -= Refresh;
    }
}
