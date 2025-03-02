using Unity.Burst;
using UnityEngine;
using UnityEngine.Events;
using Zenject;

public class CharacterActions : GravitationObject
{
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
    
    [field:Header("Targeting Settings")]
    [field: SerializeField] public CharacterTargeting Targeting{ get; private set; }
    
    public static UnityAction<CharacterActions> OnCharacterSelected;
    public static CharacterActions SelectedCharacter { get; private set; }
    public Vector3 CorrectedDirection { get; private set; }
    public float CurrentSpeed { get; private set; }
    public bool IsJump => _isJumping;
    public bool IsRunning => _isRunning;
    public bool IsSneaking => _isSneaking;
    public bool IsTargetLock => _isTargetLock;
    public bool IsDrawWeapon => _isDrawWeapon;
    
    private bool _previousUsePlayerInput;
    private Vector2 _nominalMovementDirection;
    private ICharacterInput _characterInput;
    private ICharacterInput _inputByPlayer;
    private CameraSystem _cameraSystem;
    private AnimationCurve _selectedMovementCurve;
    private bool _rotateByCamera;
    private SurfaceSlider _surfaceSlider;
    private bool _isJumping;
    private bool _isRunning;
    private bool _isSneaking;
    private bool _isTargetLock;
    private bool _isDrawWeapon;
    private const float SpeedChangeRate = 4f;

    [Inject]
    private void Construct(CameraSystem cameraSystem, PlayerInput playerInput)
    {
        _cameraSystem = cameraSystem;
        _inputByPlayer = playerInput;
    }

    protected override void Initialize()
    {
        base.Initialize();
        
        _characterInput = new AICharacterInput();
        
        CorrectedDirection = CashedTransform.position;
        _surfaceSlider = new SurfaceSlider();
        OnCharacterSelected += OnSelect;
        SubscribeInputs();
    }

    private void OnSelect(CharacterActions characterActions)
    {
        if (characterActions != this)
        {
            return;
        }
        
        if (SelectedCharacter != null)
        {
            SelectedCharacter.UnsubscribeInputs();
            SelectedCharacter._characterInput = new AICharacterInput();
            SelectedCharacter.MovementType = MovementTypes.MovementType.None;
            SelectedCharacter.SubscribeInputs();
            _cameraSystem.Select(null);
            _rotateByCamera = false;
        }

        UnsubscribeInputs();
        SelectedCharacter = this;
        _characterInput = _inputByPlayer;
        MovementType = MovementTypes.MovementType.DotWeen;
        SubscribeInputs();
        _cameraSystem.Select(this);
        _rotateByCamera = true;
    }

    private void SubscribeInputs()
    {
        _characterInput.OnJump += OnJump;
        _characterInput.OnSneak += HandleSneak;
        _characterInput.OnSprint += HandleSprint;
        _characterInput.OnHoldTarget += HandleTargetLock;
        _characterInput.OnDrawWeapon += HandleDrawWeapon;
    }

    private void UnsubscribeInputs()
    {
        _characterInput.OnJump -= OnJump;
        _characterInput.OnSneak -= HandleSneak;
        _characterInput.OnSprint -= HandleSprint;
        _characterInput.OnHoldTarget -= HandleTargetLock;
        _characterInput.OnDrawWeapon -= HandleDrawWeapon;
    }

    protected override void Update()
    {
        base.Update();
        UpdateInput();
        SelectCurve();
        UpdateSpeed();
        UpdateTargeting();
        CashedTransform.Move(CharacterController, MovementType, CorrectedDirection, CurrentSpeed, Time.deltaTime, _isJumping);
        Rotate();
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
    private void OnJump()
    {
        if (_isJumping || !IsGrounded)
        {
            return;
        }
        _isJumping = true;
        CharacterController.Jump(MovementType, CurrentSpeed, JumpHeight, JumpDuration,() => _isJumping = false);
    }
    
    private void HandleSprint(bool isRunning)
    {
        _isRunning = isRunning;
    }

    private void HandleSneak(bool isSneaking)
    {
        _isSneaking = isSneaking;
    }

    private void HandleTargetLock(bool isTargetLocked)
    {
        _isTargetLock = isTargetLocked;
    }

    private void HandleDrawWeapon(bool isDrawWeapon)
    {
        _isDrawWeapon = isDrawWeapon;
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
    private void Rotate()
    {
        if (Targeting.TargetPosition == Vector3.zero || !_isTargetLock)
        {
            CashedTransform.RotateCombined(MovementType, _nominalMovementDirection, RotationSpeed, 
                Time.deltaTime, _rotateByCamera, _cameraSystem.GetCameraYaw());
            return;
        }
        CashedTransform.RotateCombined(MovementType, Targeting.TargetPosition, RotationSpeed, 
            Time.deltaTime, false, _cameraSystem.GetCameraYaw());
    }

    [BurstCompile]
    private void UpdateTargeting()
    {
        if (!_isTargetLock)
        {
            return;
        }

        if (Targeting.Targets.Count < 1)
        {
            _isTargetLock = false;
            return;
        }
        Targeting.UpdateTarget();
    }

    [BurstCompile]
    private void UpdateInput()
    {
        _nominalMovementDirection = _characterInput.GetMoveDirection();
        
        CorrectedDirection = _surfaceSlider.UpdateDirection(CashedTransform, _nominalMovementDirection);
    }
    
    [BurstCompile]
    private void OnCollisionEnter(Collision collision)
    {
        _surfaceSlider.SetNormal(collision);
    }

    private void OnDisable()
    {
        UnsubscribeInputs();
        OnCharacterSelected -= OnSelect;
    }
}
