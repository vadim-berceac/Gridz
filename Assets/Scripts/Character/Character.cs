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
    public Vector3 CorrectedDirection => _correctedDirection;
    private Vector3 _correctedDirection;
    private SurfaceSlider _surfaceSlider;
    private bool _rotateByCamera;

    // === Locomotion ===
    public MovementTypes.MovementType CurrentMovementType { get; private set; }
    private AnimationCurve _selectedMovementCurve;
    public float CurrentSpeedZ => _currentSpeedZ; 
    public float CurrentSpeedX => _currentSpeedX; 
    private bool _previousUsePlayerInput; 
    private Vector2 _nominalMovementDirection;
    private float _currentSpeedX;
    private float _currentSpeedZ;

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
        _surfaceSlider = new SurfaceSlider();
        _correctedDirection = CashedTransform.position;
        OnCharacterSelected += OnSelect;
        CharacterStates.Subscribe(CurrentCharacterInput);
        ComponentsSettings.AnimatorLocal.runtimeAnimatorController = OverrideController;
        ComponentsSettings.Equipment.OnAnimationChanged += OnAnimationReset;
    }

    // === Update ===
    [BurstCompile]
    private void Update()
    {
        CurrentCharacterInput.Correct(CharacterStates.IsDead, _surfaceSlider, CashedTransform, ref _nominalMovementDirection, ref _correctedDirection);
        
        Speed.MoveCurve(CorrectedDirection.z, CharacterStates.IsSneaking, CharacterStates.IsRunning, LocoMotionSettings.ForwardSneakSpeedCurve, LocoMotionSettings.ForwardWalkSpeedCurve, 
            LocoMotionSettings.ForwardRunSpeedCurve, LocoMotionSettings.BackWardSneakSpeedCurve, LocoMotionSettings.BackWardWalkSpeedCurve, 
            LocoMotionSettings.BackWardRunSpeedCurve, ref _selectedMovementCurve);
        
        Speed.Update(CharacterStates.IsDead, CharacterStates.IsJump, LocoMotionSettings.SpeedChangeRate, CorrectedDirection, _selectedMovementCurve, ref _currentSpeedX, ref _currentSpeedZ);
        TargetingSettings.EnemyTargeting.Target(CharacterStates.IsDead, CharacterStates.IsTargetLock, CurrentCharacterInput);
        
        CashedTransform.Move(CharacterStates.IsDead, CharacterStates.IsTargetLock, CharacterStates.IsJump, TargetingSettings.EnemyTargeting, CharacterController, CurrentMovementType,
            CorrectedDirection, CurrentSpeedZ, CurrentSpeedX);
        
        CashedTransform.Rotate(CharacterStates.IsDead, CharacterStates.IsTargetLock, TargetingSettings.EnemyTargeting, CurrentMovementType, _rotateByCamera, _cameraSystem,
            _nominalMovementDirection, LocoMotionSettings.RotationSpeed);
        
        _paramsUpdater.UpdateParams();
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
    
    // === Collision Handling ===
    [BurstCompile]
    private void OnCollisionEnter(Collision collision)
    {
        if (CharacterStates.IsDead)
        {
            return;
        }
        _surfaceSlider.SetNormal(collision);
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