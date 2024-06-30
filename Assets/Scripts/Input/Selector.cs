using UnityEngine;
using UnityEngine.InputSystem;

public class Selector : MonoBehaviour
{
    private readonly int _unitLayer = 3;
    private static UnitFSM _selectedUnit;
    private static bool _unitSelectionBlocked = false;
    public static UnitFSM SelectedUnit => _selectedUnit;
    public static bool UnitSelectionBlocked => _unitSelectionBlocked;
    private InputAction _clickAction;
    private Vector3 _mousePosition;
    private RaycastHit _hit;
    private Collider _lastCollider;

    private void Awake()
    {        
        _clickAction = new InputAction(binding: "<Mouse>/leftButton");
        _clickAction.performed += ctx => HandleMouseClick();
        _clickAction.Enable();
    }

    public static void BlockNewUnitSelection(bool value)
    {
        _unitSelectionBlocked = value;
    }

    private void HandleMouseClick()
    {
        _mousePosition = Mouse.current.position.ReadValue();
        if (Physics.Raycast(Camera.main.ScreenPointToRay(_mousePosition), out _hit) 
            && _hit.collider.gameObject.layer == _unitLayer)
        {            
            if (_hit.collider == _lastCollider)
            {
                return;
            }
            _lastCollider = _hit.collider;
            if(!_unitSelectionBlocked)
            {
                _selectedUnit = _hit.collider.gameObject.GetComponent<UnitFSM>();
            }            
        }
        else
        {
            if (!_unitSelectionBlocked)
            {
                _selectedUnit = null;
            }                
            _lastCollider = null;
        }
    }

    private void OnDisable()
    {
        _clickAction.Disable();
    }
}
