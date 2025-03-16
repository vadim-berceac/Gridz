using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInput : MonoBehaviour, ICharacterInput
{
    [SerializeField] private InputActionAsset inputActionAsset;

    public InputActionMap InputActionMapGame { get; private set; }
    public InputActionMap InputActionMapUI { get; private set; }
    
    // Player Input Actions
    protected InputAction Move { get; private set; }
    protected InputAction Look { get; private set; }
    protected InputAction Attack { get; private set; }
    protected InputAction Interact { get; private set; }
    protected InputAction Jump { get; private set; }
    protected InputAction Sprint { get; private set; }
    protected InputAction Sneak { get; private set; }
    protected InputAction HoldTarget { get; private set; }
    protected InputAction DrawWeapon { get; private set; }
    
    // UI Input Actions
    public InputAction Navigate { get; private set; }
    public InputAction Point { get; private set; }
    public InputAction Click { get; private set; }
    public InputAction RightClick { get; private set; }
    public InputAction ScrollWheel { get; private set; }
    public InputAction OpenInventory { get; private set; }
    
    private Vector2 _moveDirection;
    private Vector2 _lookDirection;
    private bool _isWeaponDrawn;
    private bool _isTargetLock;
    
    public event Action OnAttack;
    public event Action OnInteract;
    public event Action OnJump;
    public event Action<bool> OnHoldTarget;
    public event Action<bool> OnDrawWeapon;
    public event Action<bool> OnSprint;
    public event Action<bool> OnSneak;

    private void Awake()
    {
        FindActions();
        EnableActions();
        SubscribeToActions();
    }

    public void ResetHoldTarget()
    {
        _isTargetLock = false;
        OnHoldTarget?.Invoke(false);
    }

    private void FindActions()
    {
        InputActionMapGame = inputActionAsset.FindActionMap("Player");
        InputActionMapUI = inputActionAsset.FindActionMap("UI");
        
        Move = inputActionAsset.FindAction("Move");
        Look = inputActionAsset.FindAction("Look");
        Attack = inputActionAsset.FindAction("Attack");
        Interact = inputActionAsset.FindAction("Interact");
        Jump = inputActionAsset.FindAction("Jump");
        Sprint = inputActionAsset.FindAction("Sprint");
        Sneak = inputActionAsset.FindAction("Sneak");
        HoldTarget = inputActionAsset.FindAction("HoldTarget");
        DrawWeapon = inputActionAsset.FindAction("DrawWeapon");
        
        Navigate = inputActionAsset.FindAction("Navigate");
        Point = inputActionAsset.FindAction("Point");
        Click = inputActionAsset.FindAction("Click");
        RightClick = inputActionAsset.FindAction("RightClick");
        ScrollWheel = inputActionAsset.FindAction("ScrollWheel");
        OpenInventory = inputActionAsset.FindAction("OpenInventory");
    }

    private void EnableActions()
    {
        Move.Enable();
        Look.Enable();
        Attack.Enable();
        Interact.Enable();
        Jump.Enable();
        Sprint.Enable();
        Sneak.Enable();
        HoldTarget.Enable();
        DrawWeapon.Enable();
        
        Navigate.Enable();
        Point.Enable();
        Click.Enable();
        RightClick.Enable();
        ScrollWheel.Enable();
        OpenInventory.Enable();
    }

    private void DisableActions()
    {
        Move.Disable();
        Look.Disable();
        Attack.Disable();
        Interact.Disable();
        Jump.Disable();
        Sprint.Disable();
        Sneak.Disable();
        HoldTarget.Disable();
        DrawWeapon.Disable();
        
        Navigate.Disable();
        Point.Disable();
        Click.Disable();
        RightClick.Disable();
        ScrollWheel.Disable();
        OpenInventory.Disable();
    }

    private void SubscribeToActions()
    {
        Move.performed += ctx => _moveDirection = ctx.ReadValue<Vector2>();
        Move.canceled += ctx => _moveDirection = Vector2.zero;
        
        Look.performed += ctx => _lookDirection = ctx.ReadValue<Vector2>();
        Look.canceled += ctx => _lookDirection = Vector2.zero;
        
        Attack.performed += ctx => OnAttack?.Invoke();
        Interact.performed += ctx => OnInteract?.Invoke();
        Jump.performed += ctx => OnJump?.Invoke();

        HoldTarget.performed += ctx =>
        {
            _isTargetLock = !_isTargetLock;
            OnHoldTarget?.Invoke(_isTargetLock);
        };
        
        DrawWeapon.performed += ctx => 
        {
            _isWeaponDrawn = !_isWeaponDrawn; 
            OnDrawWeapon?.Invoke(_isWeaponDrawn);
        };
        
        Sprint.performed += ctx => OnSprint?.Invoke(true);
        Sprint.canceled += ctx => OnSprint?.Invoke(false);
        
        Sneak.performed += ctx => OnSneak?.Invoke(true);
        Sneak.canceled += ctx => OnSneak?.Invoke(false);
    }

    private void OnDisable()
    {
        DisableActions();
    }
    
    public Vector2 GetMoveDirection() => _moveDirection;
    public Vector2 GetLookDirection() => _lookDirection;
}
