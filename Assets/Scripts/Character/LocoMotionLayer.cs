using Unity.Burst;
using UnityEngine;
using UnityEngine.Events;
using Zenject;

[RequireComponent(typeof(CharacterSkinModule))]
[RequireComponent(typeof(HealthModule))]
public class LocoMotionLayer : GravitationLayer
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
    
    public static UnityAction<LocoMotionLayer> OnCharacterSelected;
    public CharacterSkinModule Skin { get; private set; }
    public HealthModule Health { get; private set; }
    public static LocoMotionLayer SelectedCharacter { get; private set; }
    public bool IsJump => _isJumping;
    public bool IsRunning => _isRunning;
    public bool IsSneaking => _isSneaking;
    public bool IsTargetLock => _isTargetLock;
    public bool IsDrawWeapon => _isDrawWeapon;
    public bool IsDead {get;private set;}
    
    protected Vector3 CorrectedDirection;
    protected float CurrentSpeedZ;
    protected float CurrentSpeedX;
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
        Health = GetComponent<HealthModule>();
        CharacterInput = new AICharacterInput();
        
        CorrectedDirection = CashedTransform.position;
        _surfaceSlider = new SurfaceSlider();
        OnCharacterSelected += OnSelect;
        Health.OnDamage += HandleDamage;
        Health.OnDeath += HandleDeath;
        SubscribeInputs();
    }

    private void OnSelect(LocoMotionLayer locoMotionLayer)
    {
        if (locoMotionLayer != this)
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
        CharacterInput.OnInteract += Take;
        CharacterInput.OnWeaponSelect0 += SelectWeapon0;
        CharacterInput.OnWeaponSelect1 += SelectWeapon1;
        CharacterInput.OnWeaponSelect2 += SelectWeapon2;
    }

    protected virtual void UnsubscribeInputs()
    {
        CharacterInput.OnJump -= OnJump;
        CharacterInput.OnSneak -= HandleSneak;
        CharacterInput.OnSprint -= HandleSprint;
        CharacterInput.OnHoldTarget -= HandleTargetLock;
        CharacterInput.OnDrawWeapon -= HandleDrawWeapon;
        CharacterInput.OnAttack -= HandleAttack;
        CharacterInput.OnInteract -= Take;
        CharacterInput.OnWeaponSelect0 -= SelectWeapon0;
        CharacterInput.OnWeaponSelect1 -= SelectWeapon1;
        CharacterInput.OnWeaponSelect2 -= SelectWeapon2;
    }

    protected override void Update()
    {
        base.Update();
        CharacterInput.Correct(IsDead,_surfaceSlider, CashedTransform, ref _nominalMovementDirection, ref CorrectedDirection);
        Speed.MoveCurve(CorrectedDirection.z, _isSneaking, _isRunning, ForwardSneakSpeedCurve, ForwardWalkSpeedCurve, ForwardRunSpeedCurve,
            BackWardSneakSpeedCurve, BackWardWalkSpeedCurve, BackWardRunSpeedCurve, ref _selectedMovementCurve);
        Speed.Update(IsDead, _isJumping, SpeedChangeRate, CorrectedDirection, _selectedMovementCurve, ref CurrentSpeedX, ref CurrentSpeedZ);
        EnemyTargeting.Target(IsDead, _isTargetLock, CharacterInput);
        CashedTransform.Move(IsDead, _isTargetLock, _isJumping, EnemyTargeting, CharacterController, _currentMovementType,
            CorrectedDirection, CurrentSpeedZ, CurrentSpeedX);
        CashedTransform.Rotate(IsDead, _isTargetLock, EnemyTargeting, _currentMovementType, _rotateByCamera, _cameraSystem,
            _nominalMovementDirection, RotationSpeed);
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

    protected virtual void HandleDamage(AnimationTypes.Type animationType, float value)
    {
        if (value <= 0)
        {
            return;
        }
        Debug.LogWarning($"{name} получил урон {value} от {animationType}, осталось {Health.CurrentHealth}/{Health.MaxHealth}");
    }

    protected virtual void HandleDeath(AnimationTypes.Type animationType, bool value)
    {
        IsDead = value;
        if (IsDead)
        {
            CharacterInput.EnableCharacterInput(false);
        }
        Debug.LogWarning($"{name} убит {value} от {animationType}");
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

    protected virtual void Take()
    {
        Debug.LogWarning($"{name} попытка поднять предмет");
    }
    
    protected virtual void SelectWeapon0()
    {
        Debug.LogWarning("Select weapon 0");
    }

    protected virtual void SelectWeapon1()
    {
        Debug.LogWarning("Select weapon 1");
    }

    protected virtual void SelectWeapon2()
    {
        Debug.LogWarning("Select weapon 2");
    }
    
    [BurstCompile]
    private void OnCollisionEnter(Collision collision)
    {
        if (IsDead)
        {
            return;
        }
        _surfaceSlider.SetNormal(collision);
    }

    private void OnDisable()
    {
        UnsubscribeInputs();
        OnCharacterSelected -= OnSelect;
        Health.OnDamage -= HandleDamage;
        Health.OnDeath -= HandleDeath;
    }
}