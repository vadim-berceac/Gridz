using UnityEngine;
using UnityEngine.InputSystem;

public class Selector : MonoBehaviour
{
    [SerializeField] private int _searchingLayer = 3;
    private static UnitFSM _selectedUnit;
    public static UnitFSM SelectedUnit => _selectedUnit;
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

    private void HandleMouseClick()
    {
        _mousePosition = Mouse.current.position.ReadValue();
        if (Physics.Raycast(Camera.main.ScreenPointToRay(_mousePosition), out _hit) 
            && _hit.collider.gameObject.layer == _searchingLayer)
        {            
            if (_hit.collider == _lastCollider)
            {
                return;
            }
            _lastCollider = _hit.collider;
            _selectedUnit = _hit.collider.gameObject.GetComponent<UnitFSM>();
            //Debug.Log(_selectedUnit.name);
        }
        else
        {
            _selectedUnit = null;
            _lastCollider = null;
            //Debug.Log("reset");
        }
    }

    private void OnDisable()
    {
        _clickAction.Disable();
    }
}
