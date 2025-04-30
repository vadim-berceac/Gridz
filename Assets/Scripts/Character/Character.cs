using System.Linq;
using System.Threading.Tasks;
using Unity.Burst;
using UnityEngine;
using UnityEngine.Events;
using Zenject;

[RequireComponent(typeof(HealthModule))]
[RequireComponent(typeof(Animator))]
public class Character : GravitationLayer
{
    [field: SerializeField] public LocoMotionSettings LocoMotionSettings { get; private set; }
    [field: SerializeField] public TargetingSettings TargetingSettings { get; private set; }
    
    //modules
    public HealthModule Health { get; private set; }
    private CharacterPersonalityModule _personality;
    private EquipmentModule _equipmentModule;
    private ContainerInventory _containerInventory;
    private OneShotClipSetsContainer _oneShotClipSetsContainer;
    
    //camera
    private CameraSystem _cameraSystem;
    public static Character SelectedCharacter { get; private set; }
    public static UnityAction<Character> OnCharacterSelected;

    //input
    private ICharacterInput _playerInput;
    private ICharacterInput _currentCharacterInput;
    private Vector3 _correctedDirection;
    private SurfaceSlider _surfaceSlider;
    private bool _rotateByCamera;
    
    //locomotion
    private MovementTypes.MovementType _currentMovementType;
    private AnimationCurve _selectedMovementCurve;
    private float _currentSpeedZ;
    private float _currentSpeedX;
    private bool _previousUsePlayerInput;
    private bool _isJump;
    private bool _isDead;
    private bool _isRunning;
    private bool _isSneaking;
    private bool _isTargetLock;
    private bool _isDrawWeapon;
    private Vector2 _nominalMovementDirection;

    //animation
    public AnimationTypes.Type AnimationType { get; private set; } = AnimationTypes.Type.Default;
    private Animator _animator;
    private AnimatorOverrideController _overrideController;
    private OneShotClip _blankAttack;
    private const string OneShotClipName = "Blank";
    
    //equp
    private int _selectedWeaponIndex = 0;
    
    //params 
    public float OneShotClipPlayedValue {get; private set;}
    private float _switchBoneValue;
    
    private int _animationSpeedHash;
    private int _oneShotPlayedHash;
    private int _animationTypeHash;
    private int _switchBoneCurveHash;
    private int _groundedHash;
    private int _deadHash;
    private int _hitTriggerHash;
    private int _jumpHash;
    private int _runningHash;
    private int _sneakingHash;
    private int _targetLockHash;
    private int _drawWeaponHash;
    private int _currentSpeedZHash;
    private int _currentSpeedXHash;
    private int _oneShotTriggerHash;
    private int _drawTriggerHash;
    private int _inputXHash;
    private int _inputZHash;
   
    [Inject]
    private void Construct(PlayerInput playerInput, CameraSystem cameraSystem, OneShotClipSetsContainer container, ContainerInventory containerInventory)
    {
        _playerInput = playerInput;
        _cameraSystem = cameraSystem;
        _oneShotClipSetsContainer = container;
        _containerInventory = containerInventory;
    }
    
    protected override void Initialize()
    {
        base.Initialize();
        _currentCharacterInput = new AICharacterInput();
        Health = GetComponent<HealthModule>();
        _correctedDirection = CashedTransform.position;
        _surfaceSlider = new SurfaceSlider();
        OnCharacterSelected += OnSelect;
        Health.OnDamage += HandleDamage;
        Health.OnDeath += HandleDeath;
        SubscribeInputs();
        
        _personality = GetComponent<CharacterPersonalityModule>();
        _animator = GetComponent<Animator>();
        HashParams();
        var baseController = _animator.runtimeAnimatorController;
        _overrideController = new AnimatorOverrideController(baseController);
        _animator.runtimeAnimatorController = _overrideController;
        _animationSpeedHash = Animator.StringToHash("AnimationSpeed");
        _equipmentModule = GetComponent<EquipmentModule>();
        _equipmentModule.OnAnimationChanged += OnAnimationReset;
    }

    protected void Update()
    {
        _currentCharacterInput.Correct(_isDead, _surfaceSlider, CashedTransform, ref _nominalMovementDirection, ref _correctedDirection);
        
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
            SelectedCharacter.UnsubscribeInputs();
            var newInput =  new AICharacterInput();
            SelectedCharacter._currentCharacterInput = newInput;
            SelectedCharacter._currentMovementType = MovementTypes.MovementType.None;
            SelectedCharacter.SubscribeInputs();
            _cameraSystem.Select(null);
            _rotateByCamera = false;
        }

        UnsubscribeInputs();
        SelectedCharacter = this;
        if (!_isDead)
        {
            _currentCharacterInput = _playerInput;
            _currentMovementType = MovementTypes.MovementType.RootMotion;
            SubscribeInputs();
        }
        _cameraSystem.Select(this);
        _rotateByCamera = true;
    }

    private void SubscribeInputs()
    {
        _currentCharacterInput.OnJump += OnJump;
        _currentCharacterInput.OnSneak += HandleSneak;
        _currentCharacterInput.OnSprint += HandleSprint;
        _currentCharacterInput.OnHoldTarget += HandleTargetLock;
        _currentCharacterInput.OnDrawWeapon += HandleDrawWeapon;
        _currentCharacterInput.OnAttack += HandleAttack;
        _currentCharacterInput.OnInteract += Take;
        _currentCharacterInput.OnWeaponSelect0 += SelectWeapon0;
        _currentCharacterInput.OnWeaponSelect1 += SelectWeapon1;
        _currentCharacterInput.OnWeaponSelect2 += SelectWeapon2;
        _currentCharacterInput.OnAttack += HandleAttackTrigger;
        _currentCharacterInput.OnDrawWeapon += HandleDrawTrigger;
    }

    private void UnsubscribeInputs()
    {
        _currentCharacterInput.OnJump -= OnJump;
        _currentCharacterInput.OnSneak -= HandleSneak;
        _currentCharacterInput.OnSprint -= HandleSprint;
        _currentCharacterInput.OnHoldTarget -= HandleTargetLock;
        _currentCharacterInput.OnDrawWeapon -= HandleDrawWeapon;
        _currentCharacterInput.OnAttack -= HandleAttack;
        _currentCharacterInput.OnInteract -= Take;
        _currentCharacterInput.OnWeaponSelect0 -= SelectWeapon0;
        _currentCharacterInput.OnWeaponSelect1 -= SelectWeapon1;
        _currentCharacterInput.OnWeaponSelect2 -= SelectWeapon2;
        _currentCharacterInput.OnAttack -= HandleAttackTrigger;
        _currentCharacterInput.OnDrawWeapon -= HandleDrawTrigger;
    }

    private void ResetJump()
    {
        _isJump = false;
    }
    
    private void OnJump()
    {
        if (_isJump || !IsGrounded)
        {
            return;
        }
        _isJump = true;
        
        if (_isDead)
        {
            return;
        }
        CharacterController.Jump(_currentMovementType, _currentSpeedZ, LocoMotionSettings.JumpHeight, LocoMotionSettings.JumpDuration, ResetJump);
    }
    
    private void HandleDamage(AnimationTypes.Type animationType, float value)
    {
        if (value <= 0)
        {
            return;
        }
        Debug.LogWarning($"{name} получил урон {value} от {animationType}, осталось {Health.CurrentHealth}/{Health.MaxHealth}");
        
        _animator.SetTrigger(_hitTriggerHash);
    }

    private void HandleDeath(AnimationTypes.Type animationType, bool value)
    {
        _isDead = value;
        if (_isDead)
        {
            UnsubscribeInputs();
            _currentCharacterInput = new AICharacterInput();
            _currentCharacterInput.EnableCharacterInput(false);
            gameObject.layer = TagsAndLayersConst.PickupObjectLayerIndex;
        }
        Debug.LogWarning($"{name} убит {value} от {animationType}");
        
        _animator.SetTrigger(_deadHash);
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

    private async void HandleDrawWeapon(bool isDrawWeapon)
    {
        _isDrawWeapon = isDrawWeapon;
        
        var hasWeapon = _equipmentModule.WeaponData[_selectedWeaponIndex] != null;

        if (!isDrawWeapon)
        {
            if (!hasWeapon)
            {
                SetAnimationType(AnimationTypes.Type.Default);
                return;
            }

            await SwitchSlotAsync(_selectedWeaponIndex, 0);
            SetAnimationType(AnimationTypes.Type.Default);
            return;
        }

        SetAnimationType(hasWeapon 
            ? _equipmentModule.GetAnimationType(0) 
            : AnimationTypes.Type.Unarmed);

        if (hasWeapon)
        {
            _ = SwitchSlotAsync(_selectedWeaponIndex, 1);
        }
    }

    private void HandleAttack()
    {
        if (!_isDrawWeapon || _switchBoneValue != 0 || OneShotClipPlayedValue > 0)
        {
            return;
        }

        _blankAttack = null;

        _blankAttack = _oneShotClipSetsContainer.GetOneShotClip(_equipmentModule.GetAnimationType(_selectedWeaponIndex));
        
        if (_blankAttack == null)
        {
            Debug.LogWarning("No OneShot Clip Set");
            return;
        }
        
        SetNewClipToState(_blankAttack.Clip, OneShotClipName);
        _animator.SetFloat(_animationSpeedHash, _blankAttack.Speed);
        _animator.SetTrigger("OneShotTrigger");
    }

    private  void Take()
    {
        if (TargetingSettings.ItemTargeting.Targets.Count < 1)
        {
            Debug.LogWarning("нечего подбирать");
            return;
        }

        if (_isDrawWeapon)
        {
            _currentCharacterInput.ForciblyDrawWeapon(false);
        }
        
        if (TryTakeItem(TargetingSettings.ItemTargeting, _equipmentModule))
        {
            return;
        }

        _containerInventory.OpenContainer(TargetingSettings.ItemTargeting);
    }
    
    private void SelectWeapon0()
    {
        _ = SwitchWeaponAndSlot(0);
    }

    private void SelectWeapon1()
    {
        _ = SwitchWeaponAndSlot(1);
    }

    private void SelectWeapon2()
    {
        _ = SwitchWeaponAndSlot(2);
    }
    
    private void OnDisable()
    {
        UnsubscribeInputs();
        OnCharacterSelected -= OnSelect;
        Health.OnDamage -= HandleDamage;
        Health.OnDeath -= HandleDeath;
        _equipmentModule.OnAnimationChanged -= OnAnimationReset;
    }

    private void UpdateLocomotion()
    {
        _currentCharacterInput.Correct(_isDead, _surfaceSlider, CashedTransform, ref _nominalMovementDirection, ref _correctedDirection);
        
        Speed.MoveCurve(_correctedDirection.z, _isSneaking, _isRunning, LocoMotionSettings.ForwardSneakSpeedCurve, LocoMotionSettings.ForwardWalkSpeedCurve, 
            LocoMotionSettings.ForwardRunSpeedCurve, LocoMotionSettings.BackWardSneakSpeedCurve, LocoMotionSettings.BackWardWalkSpeedCurve, 
            LocoMotionSettings.BackWardRunSpeedCurve, ref _selectedMovementCurve);
        
        Speed.Update(_isDead, _isJump, LocoMotionSettings.SpeedChangeRate, _correctedDirection, _selectedMovementCurve, ref _currentSpeedX, ref _currentSpeedZ);
        TargetingSettings.EnemyTargeting.Target(_isDead, _isTargetLock, _currentCharacterInput);
        
        CashedTransform.Move(_isDead, _isTargetLock, _isJump, TargetingSettings.EnemyTargeting, CharacterController, _currentMovementType,
            _correctedDirection, _currentSpeedZ, _currentSpeedX);
        
        CashedTransform.Rotate(_isDead, _isTargetLock, TargetingSettings.EnemyTargeting, _currentMovementType, _rotateByCamera, _cameraSystem,
            _nominalMovementDirection, LocoMotionSettings.RotationSpeed);
    }
    
    
    [BurstCompile]
    private void OnCollisionEnter(Collision collision)
    {
        if (_isDead)
        {
            return;
        }
        _surfaceSlider.SetNormal(collision);
    }
  
    private void SetAnimationType(AnimationTypes.Type animationType)
    {
        AnimationType = animationType;
    }

    [BurstCompile]
    private void HashParams()
    {
        _animationTypeHash = Animator.StringToHash("AnimationType");
        _switchBoneCurveHash = Animator.StringToHash("SwitchBoneCurve");
        _groundedHash = Animator.StringToHash("Grounded");
        _deadHash = Animator.StringToHash("Dead");
        _hitTriggerHash = Animator.StringToHash("HitTrigger");
        _jumpHash = Animator.StringToHash("Jump");
        _runningHash = Animator.StringToHash("Run");
        _sneakingHash = Animator.StringToHash("Sneak");
        _targetLockHash = Animator.StringToHash("TargetLock");
        _drawWeaponHash = Animator.StringToHash("DrawWeapon");
        _currentSpeedZHash = Animator.StringToHash("CurrentSpeedZ");
        _currentSpeedXHash = Animator.StringToHash("CurrentSpeedX");
        _oneShotTriggerHash = Animator.StringToHash("OneShotTrigger");
        _drawTriggerHash = Animator.StringToHash("DrawTrigger");
        _inputXHash = Animator.StringToHash("InputX");
        _inputZHash = Animator.StringToHash("InputZ");
        _oneShotPlayedHash = Animator.StringToHash("OneShotPlayed");
    }

    [BurstCompile]
    private void UpdateParams()
    {
        _animator.SetFloat(_animationTypeHash, (int)AnimationType, 0.1f, Time.deltaTime);
        _animator.SetBool(_groundedHash, IsGrounded);
        _animator.SetBool(_jumpHash, _isJump);
        _animator.SetBool(_runningHash, _isRunning);
        _animator.SetBool(_sneakingHash, _isSneaking);
        _animator.SetBool(_targetLockHash, _isTargetLock);
        _animator.SetBool(_drawWeaponHash, _isDrawWeapon);
        _animator.SetFloat(_currentSpeedZHash, _currentSpeedZ);
        _animator.SetFloat(_currentSpeedXHash, _currentSpeedX, 0.5f, Time.deltaTime);
        _animator.SetFloat(_inputXHash, _correctedDirection.x, 0.2f, Time.deltaTime);
        _animator.SetFloat(_inputZHash, _correctedDirection.z, 0.2f, Time.deltaTime);
        _switchBoneValue = _animator.GetFloat(_switchBoneCurveHash);
        OneShotClipPlayedValue = _animator.GetFloat(_oneShotPlayedHash);
    }

    private void HandleAttackTrigger()
    {
        if (OneShotClipPlayedValue > 0 || !_isDrawWeapon)
        {
            return;
        }
        _animator.SetTrigger(_oneShotTriggerHash);
    }


    private void HandleDrawTrigger(bool sda)
    {
        if (OneShotClipPlayedValue > 0)
        {
            return;
        }
        _animator.SetTrigger(_drawTriggerHash);
    }

    private void OnAnimationReset()
    {
        SetAnimationType(AnimationTypes.Type.Default); 
    }
    
    private static bool TryTakeItem(ItemTargeting itemTargeting, EquipmentModule equipmentModule)
    {
        var list = itemTargeting.Targets.ToList();
        
        list[0].TryGetComponent<PickupObject>(out var item);

        if (item == null)
        {
            return false;
        }
        
        InventoryCell.FindIndexOfEmpty(equipmentModule.InventoryBag, out var index);
        
        equipmentModule.InventoryBag[index] = item.ItemData;

        Debug.LogWarning($"подбираю {item.name}");

        var toRemove = list[0].gameObject;
        
        itemTargeting.Targets.Remove(toRemove.transform);
        
        Object.Destroy(toRemove); 
        
        return true;
    }

    [BurstCompile]
    private async Task SwitchSlotAsync(int weaponIndex, int slotIndex)
    {
        while (true)
        {
            if (_switchBoneValue > 0f)
            {
                break;
            }
            await Task.Yield();
        }
       
        while (true)
        {
            if (_switchBoneValue == 0f)
            {
                _equipmentModule.WeaponData[weaponIndex].Equip(_personality.BonesCollector, slotIndex, _equipmentModule.WeaponInstances[weaponIndex]);
                break;
            }
            await Task.Yield();
        }
    }
    
    [BurstCompile]
    private void SetNewClipToState(AnimationClip clip, string clipName)
    {
        if (clip == null || string.IsNullOrEmpty(clipName))
        {
            return;
        }
        _overrideController[clipName] = clip;
        _animator.runtimeAnimatorController = _overrideController;
    }
    

    private async Task SwitchWeaponAndSlot(int index)
    {
        if (_selectedWeaponIndex == index)
        {
            return;
        }
       
        if (_isDrawWeapon)
        {
            _currentCharacterInput.ForciblyDrawWeapon(false);
            await Task.Run(() => HandleDrawWeapon(false));
        }
        
        _selectedWeaponIndex = index;
    }
}