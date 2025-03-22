using UnityEngine;
using UnityEngine.InputSystem;
using Zenject;

public class Inventory : MonoBehaviour
{
    [field: SerializeField] public GameObject InventoryWindow { get; set; }
    public bool IsOpen { get; set; }
    
    private InventoryCell[] _cells;
    
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
        _cells = transform.GetComponentsInChildren<InventoryCell>(includeInactive: true);
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

    private void Refresh()
    {
        if (_cameraSystem.SelectedCharacter.EquipmentSystem == null )
        {
            return;
        }
        
        foreach (var cell in _cells)
        {
            cell.SetItem(null);
        }
        
        for (var i = 0; i < _cameraSystem.SelectedCharacter.EquipmentSystem.InventoryBag.Count; i++)
        {
            _cells[i].SetItem(_cameraSystem.SelectedCharacter.EquipmentSystem.InventoryBag[i]);
        }
    }

    private void OnDisable()
    {
        _playerInput.OpenInventory.performed -= OnInventoryOpen;
        _cameraSystem.SelectedCharacterChanged -= ReloadInventory;
    }
}
