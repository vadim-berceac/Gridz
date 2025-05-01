using Unity.Burst;
using UnityEngine;
using UnityEngine.Events;
using Zenject;

public class Character : GravitationLayer
{
    [field: SerializeField] public LocoMotionSettings LocoMotionSettings { get; private set; }
    [field: SerializeField] public ComponentsSettings ComponentsSettings { get; private set; }
    [field: SerializeField] public TargetingSettings TargetingSettings { get; private set; }
    
    //modules
    public CharacterStates CharacterStates { get; private set; }
    public ContainerInventory LootInventory { get; private set; }
    public OneShotClipSetsContainer OneShotClipSetsContainer { get; private set; }

    // === Camera ===
    private CameraSystem _cameraSystem;
    public static Character SelectedCharacter { get; private set; }
    public static UnityAction<Character> OnCharacterSelected;

    // === Input ===
    private ICharacterInput _playerInput;
    public ICharacterInput CurrentCharacterInput { get; private set; }
    private bool _rotateByCamera;

    // === Locomotion ===
    public MovementTypes.MovementType CurrentMovementType { get; private set; }
    private AnimationCurve _selectedMovementCurve;
    public float CurrentSpeedZ => _currentSpeedZ; 
    public float CurrentSpeedX => _currentSpeedX; 
    private bool _previousUsePlayerInput; 
    private float _currentSpeedX;
    private float _currentSpeedZ;
    private Vector3 _input3;
    public Vector3 Input3 => _input3;

    // === Animation ===
    public AnimationTypes.Type AnimationType { get; private set; } = AnimationTypes.Type.Default;
    public AnimatorOverrideController OverrideController { get; private set; }
    private ParamsUpdater _paramsUpdater;
    public float OneShotClipPlayedValue { get; private set; }
    public float SwitchBoneValue { get; private set; }
   
    [Inject]
    private void Construct(PlayerInput playerInput, CameraSystem cameraSystem, OneShotClipSetsContainer container, ContainerInventory containerInventory)
    {
        _playerInput = playerInput;
        _cameraSystem = cameraSystem;
        OneShotClipSetsContainer = container;
        LootInventory = containerInventory;
    }

    protected override void Initialize()
    {
        base.Initialize();
        OverrideController = new AnimatorOverrideController(ComponentsSettings.AnimatorLocal.runtimeAnimatorController);
        CharacterStates = new CharacterStates(this);
        _paramsUpdater = new ParamsUpdater(this);
        CurrentCharacterInput = new AICharacterInput();
        OnCharacterSelected += OnSelect;
        CharacterStates.Subscribe(CurrentCharacterInput);
        ComponentsSettings.AnimatorLocal.runtimeAnimatorController = OverrideController;
        ComponentsSettings.Equipment.OnAnimationChanged += OnAnimationReset;
    }

    // === Update ===
    [BurstCompile]
    private void Update()
    {
        var input = CurrentCharacterInput.GetMoveDirection();
        _input3 = new Vector3(input.x, CashedTransform.position.y, input.y);
        
        Speed.MoveCurve(_input3.z, CharacterStates.IsSneaking, CharacterStates.IsRunning, LocoMotionSettings.ForwardSneakSpeedCurve, LocoMotionSettings.ForwardWalkSpeedCurve, 
            LocoMotionSettings.ForwardRunSpeedCurve, LocoMotionSettings.BackWardSneakSpeedCurve, LocoMotionSettings.BackWardWalkSpeedCurve, 
            LocoMotionSettings.BackWardRunSpeedCurve, ref _selectedMovementCurve);
        
        Speed.Update(CharacterStates.IsDead, CharacterStates.IsJump, LocoMotionSettings.SpeedChangeRate, _input3, _selectedMovementCurve, ref _currentSpeedX, ref _currentSpeedZ);
        
        TargetingSettings.EnemyTargeting.Target(CharacterStates.IsDead, CharacterStates.IsTargetLock, CurrentCharacterInput);
        
        _paramsUpdater.UpdateParams();
    }

    protected override void FixedUpdate()
    {
        base.FixedUpdate();
        
        Move();
        Rotate();
        
        if (CharacterStates.IsJump && IsGrounded)
        {
            CharacterStates.ResetJump();
        }
    }

    [BurstCompile]
    private void Move()
    {
        if (CharacterStates.IsJump || CharacterStates.IsDead || !IsGrounded)
        {
            return;
        }
        var force = new Vector3(0, CashedTransform.position.y, _input3.z) * CurrentSpeedZ;
        var woldForce = CashedTransform.TransformVector(force);
        ComponentsSettings.Rigidbody.linearVelocity = woldForce;
    }

    [BurstCompile]
    private void Rotate()
    {
        if (CharacterStates.IsJump || CharacterStates.IsDead || !IsGrounded)
        {
            return;
        }
          //не реализовано управление вращением от камеры
        if (_rotateByCamera)
        {
            ComponentsSettings.Rigidbody.angularVelocity = new Vector3 (0f, _input3.x  * LocoMotionSettings.RotationSpeed, 0f);
            return;
        }
        ComponentsSettings.Rigidbody.angularVelocity = new Vector3 (0f, _input3.x * LocoMotionSettings.RotationSpeed, 0f);
    }

    // === Character Selection ===
    [BurstCompile]
    private void OnSelect(Character character)
    {
        if (character != this)
        {
            return;
        }
        
        if (SelectedCharacter != null)
        {
            SelectedCharacter.CharacterStates.Unsubscribe();
            var newInput = new AICharacterInput();
            SelectedCharacter.CurrentCharacterInput = newInput;
            SelectedCharacter.CurrentMovementType = MovementTypes.MovementType.None;
            SelectedCharacter.CharacterStates.Subscribe(newInput);
            _cameraSystem.Select(null);
            _rotateByCamera = false;
        }

        CharacterStates.Unsubscribe();
        SelectedCharacter = this;
        if (!CharacterStates.IsDead)
        {
            CurrentCharacterInput = _playerInput;
            CurrentMovementType = MovementTypes.MovementType.RootMotion;
            CharacterStates.Subscribe(CurrentCharacterInput);
        }
        _cameraSystem.Select(this);
        _rotateByCamera = true;
    }

    // === State Setters ===
    public void SetAnimationType(AnimationTypes.Type animationType)
    {
        AnimationType = animationType;
    }

    public void SetSwitchBoneValue(float value)
    {
        SwitchBoneValue = value;
    }

    public void SetOneShotClipPlayedValue(float value)
    {
        OneShotClipPlayedValue = value;
    }

    public void SetInput(ICharacterInput input)
    {
        CurrentCharacterInput = input;
    }

    // === Animation Reset ===
    private void OnAnimationReset()
    {
        SetAnimationType(AnimationTypes.Type.Default); 
    }

    // === Cleanup ===
    private void OnDisable()
    {
        CharacterStates.Unsubscribe();
        OnCharacterSelected -= OnSelect;
        ComponentsSettings.Equipment.OnAnimationChanged -= OnAnimationReset;
    }
}