using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Serialization;
using Zenject;

public class Inventory : MonoBehaviour
{
    [field: SerializeField] public GameObject InventoryWindow { get; set; }
    [field: SerializeField] private Transform bagTransform;
    [field: SerializeField] private Transform weaponTable;
    
    public bool IsOpen { get; set; }
    
    public InventoryCell[] BagCells { get; set; }
    public InventoryCell[] WeaponTableCells { get; set; }
    
    private PlayerInput _playerInput;
    private CameraSystem _cameraSystem;

    [Inject]
    private void Construct(PlayerInput playerInput, CameraSystem cameraSystem)
    {
        _playerInput = playerInput;
        _cameraSystem = cameraSystem;
    }
    
    //нужно реализовать отображение стака вещей в одной ячейке (счет не в itemData - потому что то ScriptableObject и общий для всех предметов)

    private void Awake()
    {
        Init();
    }

    private void Init()
    {
        _playerInput.OpenInventory.performed += OnInventoryOpen;
        _cameraSystem.SelectedCharacterChanged += ReloadInventory;
        BagCells = bagTransform.GetComponentsInChildren<InventoryCell>(includeInactive: true);
        WeaponTableCells = weaponTable.GetComponentsInChildren<InventoryCell>(includeInactive: true);
    }

    private void OnInventoryOpen(InputAction.CallbackContext ctx)
    {
        if (_cameraSystem.SelectedCharacter == null)
        {
            OpenClose(false);
            return;
        }
        OpenClose(!IsOpen);
    }

    private void OpenClose(bool value)
    {
        IsOpen = value;
        InventoryWindow.SetActive(value);
        if (value)
        {
            Refresh();
            _playerInput.InputActionMapGame.Disable();
            return;
        }
        _playerInput.InputActionMapGame.Enable();
    }

    private void ReloadInventory(LocoMotion locoMotion)
    {
        if (!IsOpen || _cameraSystem.SelectedCharacter == null)
        {
            return;
        }

        Refresh();
    }

    public void Refresh()
    {
        if (_cameraSystem.SelectedCharacter.EquipmentSystem == null)
        {
            return;
        }
       
        foreach (var cell in BagCells)
        {
            cell.SetItem(null, _cameraSystem.SelectedCharacter.EquipmentSystem, this);
        }
      
        for (var i = 0; i < _cameraSystem.SelectedCharacter.EquipmentSystem.InventoryBag.Count; i++)
        {
            var currentItem = _cameraSystem.SelectedCharacter.EquipmentSystem.InventoryBag[i];
            var isInWeaponTable = false;
           
            foreach (var weaponCell in WeaponTableCells)
            {
                if (weaponCell.Item == currentItem)
                {
                    isInWeaponTable = true;
                    break; 
                }
            }

            if (!isInWeaponTable && i < BagCells.Length) 
            {
                BagCells[i].SetItem(currentItem, _cameraSystem.SelectedCharacter.EquipmentSystem, this);
            }
        }
    }

    private void OnDisable()
    {
        _playerInput.OpenInventory.performed -= OnInventoryOpen;
        _cameraSystem.SelectedCharacterChanged -= ReloadInventory;
    }
}
