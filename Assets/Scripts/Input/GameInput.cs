using UnityEngine;
using UnityEngine.InputSystem;

public class GameInput : MonoBehaviour
{
    [SerializeField] private InputActionAsset inputActionAsset;

    public InputActionMap InputActionMapGame { get; private set; }
    public InputActionMap InputActionMapUI { get; private set; }
    
    //Player
    public InputAction Move { get; private set; }
    public InputAction Look { get; private set; }
    public InputAction Attack { get; private set; }
    public InputAction Interact { get; private set; }
    public InputAction Jump { get; private set; }
    
    public InputAction Sprint { get; private set; }
    public InputAction Sneak { get; private set; }
    
    //UI
    public InputAction Navigate { get; private set; }
    public InputAction Point { get; private set; }
    public InputAction Click { get; private set; }
    public InputAction RightClick { get; private set; }
    public InputAction ScrollWheel { get; private set; }

    private void Awake()
    {
        FindActions();
        EnableActions();
    }

    private void FindActions()
    {
        InputActionMapGame = inputActionAsset.FindActionMap("Game");
        InputActionMapUI = inputActionAsset.FindActionMap("UI");
        
        Move = inputActionAsset.FindAction("Move");
        Look = inputActionAsset.FindAction("Look");
        Attack = inputActionAsset.FindAction("Attack");
        Interact = inputActionAsset.FindAction("Interact");
        Jump = inputActionAsset.FindAction("Jump");
        Sprint = inputActionAsset.FindAction("Sprint");
        Sneak = inputActionAsset.FindAction("Sneak");
        
        Navigate = inputActionAsset.FindAction("Navigate");
        Point = inputActionAsset.FindAction("Point");
        Click = inputActionAsset.FindAction("Click");
        RightClick = inputActionAsset.FindAction("RightClick");
        ScrollWheel = inputActionAsset.FindAction("ScrollWheel");
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
        
        Navigate.Enable();
        Point.Enable();
        Click.Enable();
        RightClick.Enable();
        ScrollWheel.Enable();
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
        
        Navigate.Disable();
        Point.Disable();
        Click.Disable();
        RightClick.Disable();
        ScrollWheel.Disable();
    }

    private void OnDisable()
    {
        DisableActions();
    }
}
