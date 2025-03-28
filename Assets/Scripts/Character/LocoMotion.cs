using Unity.Android.Gradle.Manifest;
using Unity.Burst;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using Zenject;
using Action = System.Action;

[RequireComponent(typeof(CharacterSkinModule))]
public class LocoMotion : GravitationObject
{
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
    [field: SerializeField] public CharacterTargeting EnemyTargeting { get; private set; }
    [field: SerializeField] public ItemTargeting ItemTargeting { get; private set; }
    
    [field:Header("Inventory")]
    [field: SerializeField] public EquipmentSystem EquipmentSystem { get; private set; }
    
    public static UnityAction<LocoMotion> OnCharacterSelected;
    
    public CharacterSkinModule Skin { get; private set; }
    public static LocoMotion SelectedCharacter { get; private set; }
    public Vector3 CorrectedDirection { get; private set; }
    public float CurrentSpeedZ { get; private set; }
    public float CurrentSpeedX { get; private set; }
    public bool IsJump => _isJumping;
    public bool IsRunning => _isRunning;
    public bool IsSneaking => _isSneaking;
    public bool IsTargetLock => _isTargetLock;
    public bool IsDrawWeapon => _isDrawWeapon;
    
    private bool _previousUsePlayerInput;
    private Vector2 _nominalMovementDirection;
    private MovementTypes.MovementType _currentMovementType;
    protected ICharacterInput CharacterInput {get; private set;}
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
        Skin = GetComponent<CharacterSkinModule>();
        CharacterInput = new AICharacterInput();
        
        CorrectedDirection = CashedTransform.position;
        _surfaceSlider = new SurfaceSlider();
        OnCharacterSelected += OnSelect;
        SubscribeInputs();
    }

    private void OnSelect(LocoMotion locoMotion)
    {
        if (locoMotion != this)
        {
            return;
        }
        
        if (SelectedCharacter != null)
        {
            SelectedCharacter.UnsubscribeInputs();
            var newInput =  new AICharacterInput();
            SelectedCharacter.CharacterInput = newInput;
            SelectedCharacter._currentMovementType = MovementTypes.MovementType.None;
            SelectedCharacter.SubscribeInputs();
            _cameraSystem.Select(null);
            _rotateByCamera = false;
        }

        UnsubscribeInputs();
        SelectedCharacter = this;
        CharacterInput = _inputByPlayer;
        _currentMovementType = MovementTypes.MovementType.RootMotion;
        SubscribeInputs();
        _cameraSystem.Select(this);
        _rotateByCamera = true;
    }

    protected virtual void SubscribeInputs()
    {
        CharacterInput.OnJump += OnJump;
        CharacterInput.OnSneak += HandleSneak;
        CharacterInput.OnSprint += HandleSprint;
        CharacterInput.OnHoldTarget += HandleTargetLock;
        CharacterInput.OnDrawWeapon += HandleDrawWeapon;
        CharacterInput.OnAttack += HandleAttack;
        CharacterInput.OnInteract += HandleInteract;
    }

    protected virtual void UnsubscribeInputs()
    {
        CharacterInput.OnJump -= OnJump;
        CharacterInput.OnSneak -= HandleSneak;
        CharacterInput.OnSprint -= HandleSprint;
        CharacterInput.OnHoldTarget -= HandleTargetLock;
        CharacterInput.OnDrawWeapon -= HandleDrawWeapon;
        CharacterInput.OnAttack -= HandleAttack;
        CharacterInput.OnInteract -= HandleInteract;
    }

    protected override void Update()
    {
        base.Update();
        UpdateInput();
        SelectCurve();
        UpdateSpeed();
        UpdateTargeting();
        UpdateMove();
        UpdateRotate();
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
        CharacterController.Jump(_currentMovementType, CurrentSpeedZ, JumpHeight, JumpDuration,() => _isJumping = false);
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

    protected virtual void HandleDrawWeapon(bool isDrawWeapon)
    {
        _isDrawWeapon = isDrawWeapon;
    }

    protected virtual void HandleAttack()
    {
        Debug.LogWarning($"{name} атакует");
    }

    protected virtual void HandleInteract()
    {
        Debug.LogWarning($"{name} попытка поднять предмет");
    }
    
    [BurstCompile]
    private void UpdateSpeed()
    {
        var speedZ = Mathf.Clamp01(Mathf.Abs(CorrectedDirection.z));
        var speedX = Mathf.Clamp01(Mathf.Abs(CorrectedDirection.x));

        if (_isJumping)
        {
            return;
        }
        
        CurrentSpeedX = speedX * CurrentSpeedZ;
        
        var targetSpeed = _selectedMovementCurve.Evaluate(speedZ * _selectedMovementCurve.keys[_selectedMovementCurve.length - 1].time);

        CurrentSpeedZ = Mathf.MoveTowards(CurrentSpeedZ, targetSpeed, Time.deltaTime * SpeedChangeRate);
    }

    [BurstCompile]
    private void UpdateRotate()
    {
        if (EnemyTargeting.TargetDirection == Vector3.zero || !_isTargetLock)
        {
            CashedTransform.RotateCombined(_currentMovementType, _nominalMovementDirection, RotationSpeed, 
                Time.deltaTime, _rotateByCamera, _cameraSystem.GetCameraYaw());
            return;
        }
        CashedTransform.RotateTo(_currentMovementType, EnemyTargeting.TargetDirection, RotationSpeed, 
            Time.deltaTime);
    }

    [BurstCompile]
    private void UpdateMove()
    {
        if (EnemyTargeting.TargetDirection == Vector3.zero || !_isTargetLock)
        {
            CashedTransform.Move(CharacterController, _currentMovementType, CorrectedDirection, CurrentSpeedZ, Time.deltaTime, _isJumping);
            return;
        }

        //нестабильно
        if (Mathf.Abs(CorrectedDirection.z) <  0.1f)
        {
            CashedTransform.Move(CharacterController, _currentMovementType, EnemyTargeting.TargetDirection, CurrentSpeedX, Time.deltaTime, _isJumping);
            return;
        }
    
        CashedTransform.Move(CharacterController, _currentMovementType, CorrectedDirection, CurrentSpeedZ, Time.deltaTime, _isJumping);
    }

    [BurstCompile]
    private void UpdateTargeting()
    {
        if (!_isTargetLock)
        {
            return;
        }

        if (EnemyTargeting.Targets.Count < 1)
        {
            CharacterInput.ResetHoldTarget();
            return;
        }
        EnemyTargeting.UpdateTarget();
    }

    [BurstCompile]
    private void UpdateInput()
    {
        _nominalMovementDirection = CharacterInput.GetMoveDirection();
        
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
