using Unity.Burst;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;
using Zenject;

public class CharacterMovement : GravitationObject
{
    [field: SerializeField] public bool UsePlayerInput { get; private set; }
    [field: SerializeField] public MovementTypes.MovementType MovementType { get; private set; }
    
    [field:Header("Rotation Settings")]
    [field: SerializeField] public float RotationSpeed { get; private set; } = 2f;
    
    [field:Header("Sneak Settings")]
    [field: SerializeField] public AnimationCurve ForwardSneakSpeedCurve { get; private set; } = new AnimationCurve();
    [field: SerializeField] public AnimationCurve BackWardSneakSpeedCurve { get; private set; } = new AnimationCurve();
    
    [field:Header("Walk Settings")]
    [field: SerializeField] public AnimationCurve ForwardWalkSpeedCurve { get; private set; } = new AnimationCurve();
    [field: SerializeField] public AnimationCurve BackWardWalkSpeedCurve { get; private set; } = new AnimationCurve();
   
    [field:Header("Run Settings")]
    [field: SerializeField] public AnimationCurve ForwardRunSpeedCurve { get; private set; } = new AnimationCurve();
    [field: SerializeField] public AnimationCurve BackWardRunSpeedCurve { get; private set; } = new AnimationCurve();
    
    [field:Header("Jump Settings")]
    [field: SerializeField] public float JumpDuration { get; private set; } = 0.5f;
    [field: SerializeField] public float JumpHeight { get; private set; } = 2f;
    
    public static UnityAction<CharacterMovement> OnCharacterSelected;
    public Vector3 CorrectedDirection { get; private set; }
    public float CurrentSpeed { get; private set; }
    public bool IsJump => _isJumping;
    public bool IsRunning => _isRunning;
    public bool IsSneaking => _isSneaking;
    
    private bool _previousUsePlayerInput;
    private Vector2 _nominalMovementDirection;
    private GameInput _gameInput;
    private CameraSystem _cameraSystem;
    private AnimationCurve _selectedMovementCurve;
    private bool _rotateByCamera;
    private SurfaceSlider _surfaceSlider;
    private bool _isJumping;
    private bool _isRunning;
    private bool _isSneaking;
    private const float SpeedChangeRate = 4f;

    [Inject]
    private void Construct(GameInput gameInput, CameraSystem cameraSystem)
    {
        _gameInput = gameInput;
        _cameraSystem = cameraSystem;
    }
    
    private void OnValidate()
    {
        if (_previousUsePlayerInput == UsePlayerInput)
        {
          return;
        }
        CheckIsPlayer(); 
        _previousUsePlayerInput = UsePlayerInput; 
    }

    protected override void Initialize()
    {
        base.Initialize();
        CorrectedDirection = CashedTransform.position;
        _previousUsePlayerInput = UsePlayerInput;
        _surfaceSlider = new SurfaceSlider();
        _gameInput.Jump.performed += OnJump;
        OnCharacterSelected += OnSelect;
       CheckIsPlayer();
    }

    private void CheckIsPlayer()
    {
        if (!UsePlayerInput)
        {
            MovementType = MovementTypes.MovementType.None;
            OnCharacterSelected?.Invoke(null);
            return;
        }
        MovementType = MovementTypes.MovementType.DotWeen;
        OnCharacterSelected?.Invoke(this);
    }

    private void OnSelect(CharacterMovement characterMovement)
    {
        if (_cameraSystem.SelectedCharacter == this)
        {
            MovementType = MovementTypes.MovementType.DotWeen;
            return;
        }
        MovementType = MovementTypes.MovementType.None;
    }

    protected override void Update()
    {
        base.Update();
        UpdateInput();
        SelectCurve();
        UpdateSpeed();
        CheckCameraSelection();
        CashedTransform.Move(CharacterController, MovementType, CorrectedDirection, CurrentSpeed, Time.deltaTime, _isJumping);
        CashedTransform.RotateCombined(MovementType, _nominalMovementDirection, RotationSpeed, 
            Time.deltaTime, _rotateByCamera, _cameraSystem.GetCameraYaw());
    }

    [BurstCompile]
    private void SelectCurve()
    {
        var isForward = CorrectedDirection.z > 0;

        _selectedMovementCurve = isForward
            ? (_isSneaking ? ForwardSneakSpeedCurve : _isRunning ? ForwardRunSpeedCurve : ForwardWalkSpeedCurve)
            : (_isSneaking ? BackWardSneakSpeedCurve : _isRunning ? BackWardRunSpeedCurve : BackWardWalkSpeedCurve);
    }

    [BurstCompile]
    private void OnJump(InputAction.CallbackContext context)
    {
        if (_isJumping || !IsGrounded)
        {
            return;
        }
        _isJumping = true;
        CharacterController.Jump(MovementType, CurrentSpeed, JumpHeight, JumpDuration,() => _isJumping = false);
    }
    
    [BurstCompile]
    private void UpdateSpeed()
    {
        var t = Mathf.Clamp01(Mathf.Abs(CorrectedDirection.z));

        if (_isJumping)
        {
            return;
        }

        if (t == 0)
        {
            CurrentSpeed = 0;
            return;
        }
        
        var targetSpeed = _selectedMovementCurve.Evaluate(t * _selectedMovementCurve.keys[_selectedMovementCurve.length - 1].time);

        CurrentSpeed = Mathf.MoveTowards(CurrentSpeed, targetSpeed, Time.deltaTime * SpeedChangeRate);
    }

    [BurstCompile]
    private void UpdateInput()
    {
        _nominalMovementDirection = _gameInput.Move.ReadValue<Vector2>();

        _isRunning = _gameInput.Sprint.ReadValue<float>() > 0;
        _isSneaking = _gameInput.Sneak.ReadValue<float>() > 0;
        
        CorrectedDirection = _surfaceSlider.UpdateDirection(CashedTransform, _nominalMovementDirection);
    }
    
    [BurstCompile]
    private void CheckCameraSelection()
    {
        if (_cameraSystem.SelectedCharacter != this)
        {
            _rotateByCamera = false;
            return;
        }
        _rotateByCamera = true;
    }
    
    [BurstCompile]
    private void OnCollisionEnter(Collision collision)
    {
        _surfaceSlider.SetNormal(collision);
    }

    private void OnDisable()
    {
        _gameInput.Jump.performed -= OnJump;
        OnCharacterSelected -= OnSelect;
    }
}
