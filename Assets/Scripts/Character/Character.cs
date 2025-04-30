using System.Linq;
using System.Threading.Tasks;
using Unity.Burst;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Serialization;
using Zenject;

[RequireComponent(typeof(HealthModule))]
[RequireComponent(typeof(Animator))]
public class Character : GravitationLayer
{
    [field: SerializeField] public LocoMotionSettings LocoMotionSettings { get; private set; }
    [field: SerializeField] public TargetingSettings TargetingSettings { get; private set; }
    
    //modules
    public CharacterActions CharacterActions { get; private set; }
    public HealthModule Health { get; private set; }
    public CharacterPersonalityModule Personality { get; private set; }
    public EquipmentModule EquipmentModule { get; private set; }
    public ContainerInventory LootInventory { get; private set; }
    public OneShotClipSetsContainer OneShotClipSetsContainer { get; private set; }
    
    //camera
    private CameraSystem _cameraSystem;
    public static Character SelectedCharacter { get; private set; }
    public static UnityAction<Character> OnCharacterSelected;

    //input
    private ICharacterInput _playerInput;
    public ICharacterInput CurrentCharacterInput { get; private set; }
    private Vector3 _correctedDirection;
    private SurfaceSlider _surfaceSlider;
    private bool _rotateByCamera;
    
    //locomotion
    public MovementTypes.MovementType CurrentMovementType { get; private set; }
    private AnimationCurve _selectedMovementCurve;
    public float CurrentSpeedZ => _currentSpeedZ; 
    public float CurrentSpeedX => _currentSpeedX; 
    private bool _previousUsePlayerInput;
    public bool IsDead { get; private set; }
    private Vector2 _nominalMovementDirection;
    private float _currentSpeedX;
    private float _currentSpeedZ;

    //animation
    public AnimationTypes.Type AnimationType { get; private set; } = AnimationTypes.Type.Default;
    public Animator AnimatorLocal { get; private set; }
    public AnimatorOverrideController OverrideController { get; private set; }
    
    public float OneShotClipPlayedValue {get; private set;}
    public float SwitchBoneValue {get; private set;}
   
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
        CurrentCharacterInput = new AICharacterInput();
        _correctedDirection = CashedTransform.position;
        _surfaceSlider = new SurfaceSlider();
        
        OnCharacterSelected += OnSelect;
        
        Health = GetComponent<HealthModule>();
        Health.OnDamage += HandleDamage;
        Health.OnDeath += HandleDeath;
        
        CharacterActions = new CharacterActions(this);
        CharacterActions.Subscribe(CurrentCharacterInput);
        
        Personality = GetComponent<CharacterPersonalityModule>();
        AnimatorLocal = GetComponent<Animator>();
        
        OverrideController = new AnimatorOverrideController(AnimatorLocal.runtimeAnimatorController);
        AnimatorLocal.runtimeAnimatorController = OverrideController;
        
        EquipmentModule = GetComponent<EquipmentModule>();
        EquipmentModule.OnAnimationChanged += OnAnimationReset;
    }

    protected void Update()
    {
        CurrentCharacterInput.Correct(IsDead, _surfaceSlider, CashedTransform, ref _nominalMovementDirection, ref _correctedDirection);
        
        UpdateLocomotion();
        UpdateParams();
    }
    
    private void OnSelect(Character character)
    {
        if (character != this)
        {
            return;
        }
        
        if (SelectedCharacter != null)
        {
            SelectedCharacter.CharacterActions.Unsubscribe();
            var newInput =  new AICharacterInput();
            SelectedCharacter.CurrentCharacterInput = newInput;
            SelectedCharacter.CurrentMovementType = MovementTypes.MovementType.None;
            SelectedCharacter.CharacterActions.Subscribe(newInput);
            _cameraSystem.Select(null);
            _rotateByCamera = false;
        }

        CharacterActions.Unsubscribe();
        SelectedCharacter = this;
        if (!IsDead)
        {
            CurrentCharacterInput = _playerInput;
            CurrentMovementType = MovementTypes.MovementType.RootMotion;
            CharacterActions.Subscribe(CurrentCharacterInput);
        }
        _cameraSystem.Select(this);
        _rotateByCamera = true;
    }
    
    private void HandleDamage(AnimationTypes.Type animationType, float value)
    {
        if (value <= 0)
        {
            return;
        }
        Debug.LogWarning($"{name} получил урон {value} от {animationType}, осталось {Health.CurrentHealth}/{Health.MaxHealth}");
        
        AnimatorLocal.SetTrigger( AnimationParams.HitTrigger);
    }

    private void HandleDeath(AnimationTypes.Type animationType, bool value)
    {
        IsDead = value;
        if (IsDead)
        {
            CharacterActions.Unsubscribe();
            CurrentCharacterInput = new AICharacterInput();
            CurrentCharacterInput.EnableCharacterInput(false);
            gameObject.layer = TagsAndLayersConst.PickupObjectLayerIndex;
        }
        Debug.LogWarning($"{name} убит {value} от {animationType}");
        
        AnimatorLocal.SetTrigger(AnimationParams.Dead);
    }
    
    private void OnDisable()
    {
        CharacterActions.Unsubscribe();
        OnCharacterSelected -= OnSelect;
        Health.OnDamage -= HandleDamage;
        Health.OnDeath -= HandleDeath;
        EquipmentModule.OnAnimationChanged -= OnAnimationReset;
    }

    private void UpdateLocomotion()
    {
        CurrentCharacterInput.Correct(IsDead, _surfaceSlider, CashedTransform, ref _nominalMovementDirection, ref _correctedDirection);
        
        Speed.MoveCurve(_correctedDirection.z, CharacterActions.IsSneaking, CharacterActions.IsRunning, LocoMotionSettings.ForwardSneakSpeedCurve, LocoMotionSettings.ForwardWalkSpeedCurve, 
            LocoMotionSettings.ForwardRunSpeedCurve, LocoMotionSettings.BackWardSneakSpeedCurve, LocoMotionSettings.BackWardWalkSpeedCurve, 
            LocoMotionSettings.BackWardRunSpeedCurve, ref _selectedMovementCurve);
        
        Speed.Update(IsDead, CharacterActions.IsJump, LocoMotionSettings.SpeedChangeRate, _correctedDirection, _selectedMovementCurve, ref _currentSpeedX, ref _currentSpeedZ);
        TargetingSettings.EnemyTargeting.Target(IsDead, CharacterActions.IsTargetLock, CurrentCharacterInput);
        
        CashedTransform.Move(IsDead, CharacterActions.IsTargetLock, CharacterActions.IsJump, TargetingSettings.EnemyTargeting, CharacterController, CurrentMovementType,
            _correctedDirection, CurrentSpeedZ, CurrentSpeedX);
        
        CashedTransform.Rotate(IsDead, CharacterActions.IsTargetLock, TargetingSettings.EnemyTargeting, CurrentMovementType, _rotateByCamera, _cameraSystem,
            _nominalMovementDirection, LocoMotionSettings.RotationSpeed);
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
  
    public void SetAnimationType(AnimationTypes.Type animationType)
    {
        AnimationType = animationType;
    }

    [BurstCompile]
    private void UpdateParams()
    {
        AnimatorLocal.SetFloat(AnimationParams.AnimationType, (int)AnimationType, 0.1f, Time.deltaTime);
        AnimatorLocal.SetBool(AnimationParams.Grounded, IsGrounded);
        AnimatorLocal.SetBool( AnimationParams.Jump, CharacterActions.IsJump);
        AnimatorLocal.SetBool(AnimationParams.Running, CharacterActions.IsRunning);
        AnimatorLocal.SetBool(AnimationParams.Sneaking, CharacterActions.IsSneaking);
        AnimatorLocal.SetBool(AnimationParams.TargetLock, CharacterActions.IsTargetLock);
        AnimatorLocal.SetBool( AnimationParams.DrawWeapon, CharacterActions.IsDrawWeapon);
        AnimatorLocal.SetFloat( AnimationParams.CurrentSpeedZ, CurrentSpeedZ);
        AnimatorLocal.SetFloat(AnimationParams.CurrentSpeedX, CurrentSpeedX, 0.5f, Time.deltaTime);
        AnimatorLocal.SetFloat(AnimationParams.InputX, _correctedDirection.x, 0.2f, Time.deltaTime);
        AnimatorLocal.SetFloat( AnimationParams.InputZ, _correctedDirection.z, 0.2f, Time.deltaTime);
        SwitchBoneValue = AnimatorLocal.GetFloat(AnimationParams.SwitchBoneCurve);
        OneShotClipPlayedValue = AnimatorLocal.GetFloat(AnimationParams.OneShotPlayed);
    }

    private void OnAnimationReset()
    {
        SetAnimationType(AnimationTypes.Type.Default); 
    }
}