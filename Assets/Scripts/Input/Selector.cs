using UnityEngine;
using UnityEngine.InputSystem;

public class Selector : MonoBehaviour
{
    private readonly int _unitLayer = 3;
    private static UnitFSM _activeUnit;
    private static UnitFSM _selectedAsTargetUnit;
    private static bool _newUnitActivationIsBlocked = false;
    private static bool _newSelectedAsTargetUnitBlocked = false;
    public static UnitFSM ActiveUnit => _activeUnit;
    public static UnitFSM SelectedAsTargetUnit => _selectedAsTargetUnit;
    public static bool NewUnitActivationIsBlocked => _newUnitActivationIsBlocked;
    public static bool NewSelectedAsTargetUnitBlocked => _newSelectedAsTargetUnitBlocked;
    private InputAction _leftButtonClickAction;
    private InputAction _rightButtonClickAction;
    private Vector3 _mousePosition;
    private RaycastHit _hit;
    private Collider _lastCollider;

    private void Awake()
    {
        InitializeInput();
    }

    private void InitializeInput()
    {
        _leftButtonClickAction = new InputAction(binding: "<Mouse>/leftButton");
        _leftButtonClickAction.performed += ctx => HandleLeftButtonMouseClick();
        _leftButtonClickAction.Enable();

        _rightButtonClickAction = new InputAction(binding: "<Mouse>/rightButton");
        _rightButtonClickAction.performed += ctx => HandleRightButtonMouseClick();
        _rightButtonClickAction.Enable();   
    }

    public static void BlockNewUnitActivation(bool value)
    {
        _newUnitActivationIsBlocked = value;
    }

    public static void BlockNewUnitAsTargetSelection(bool value)
    {
        _newSelectedAsTargetUnitBlocked = value;
    }

    public static void ResetAttackTarget()
    {
        _selectedAsTargetUnit = null;
    }

    private void HandleLeftButtonMouseClick()
    {
        _mousePosition = Mouse.current.position.ReadValue();
        _selectedAsTargetUnit = null;
        if (Physics.Raycast(Camera.main.ScreenPointToRay(_mousePosition), out _hit) 
            && _hit.collider.gameObject.layer == _unitLayer)
        {            
            if (_hit.collider == _lastCollider)
            {
                return;
            }
            _lastCollider = _hit.collider;
            if(!_newUnitActivationIsBlocked)
            {
                _activeUnit = _hit.collider.gameObject.GetComponent<UnitFSM>();
            }            
        }
        else
        {
            if (!_newUnitActivationIsBlocked && _activeUnit.TilePath.Count < 1)
            {
                _activeUnit = null;
                _selectedAsTargetUnit = null;
            }                
            _lastCollider = null;
        }
    }

    private void HandleRightButtonMouseClick()
    {
        if(!_activeUnit)
        {
            _selectedAsTargetUnit = null;
            return;
        }
        if (_newSelectedAsTargetUnitBlocked == true)
        {
            return;
        }
        _mousePosition = Mouse.current.position.ReadValue();
        if (!Physics.Raycast(Camera.main.ScreenPointToRay(_mousePosition), out _hit)
            || _hit.collider.gameObject.layer != _unitLayer)
        {
            return;
        }
        var temp = _hit.collider.gameObject.GetComponent<UnitFSM>();
        if(temp == _activeUnit)
        {
            return;
        }
        _selectedAsTargetUnit = temp;

        Debug.Log(_selectedAsTargetUnit.name);
    }

    private void OnDisable()
    {
        _leftButtonClickAction.Disable();
        _rightButtonClickAction.Disable();
    }
}
