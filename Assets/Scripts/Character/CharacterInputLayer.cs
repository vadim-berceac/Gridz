using UnityEngine;
using UnityEngine.Events;
using Zenject;

[RequireComponent(typeof(HealthModule))]
public class CharacterInputLayer : GravitationLayer
{
    [field:Header("Inventory")]
    [field: SerializeField] public EquipmentSystem EquipmentSystem { get; private set; }
    
    public static UnityAction<CharacterInputLayer> OnCharacterSelected;
    public HealthModule Health { get; private set; }
    protected bool IsJump {get;private set;}
    protected bool IsDead {get;private set;}
    protected bool IsRunning  { get; private set; }
    protected bool IsSneaking  { get; private set; }
    protected bool IsTargetLock  { get; private set; }
    protected bool IsDrawWeapon  { get; private set; }
    protected Vector2 NominalMovementDirection;
    private static CharacterInputLayer _selectedCharacter;
    
    protected ICharacterInput CharacterInput {get; private set;}
    protected Vector3 CorrectedDirection;
    protected SurfaceSlider SurfaceSlider;
    private ICharacterInput _inputByPlayer;
    protected MovementTypes.MovementType CurrentMovementType { get; private set; }
    protected bool RotateByCamera { get; private set; }
    protected CameraSystem CameraSystem { get; private set; }
   
    [Inject]
    private void Construct(PlayerInput playerInput, CameraSystem cameraSystem)
    {
        _inputByPlayer = playerInput;
        CameraSystem = cameraSystem;
    }
    
    protected override void Initialize()
    {
        base.Initialize();
        CharacterInput = new AICharacterInput();
        Health = GetComponent<HealthModule>();
        CorrectedDirection = CashedTransform.position;
        SurfaceSlider = new SurfaceSlider();
        OnCharacterSelected += OnSelect;
        Health.OnDamage += HandleDamage;
        Health.OnDeath += HandleDeath;
        SubscribeInputs();
    }

    protected override void Update()
    {
        base.Update();
        CharacterInput.Correct(IsDead, SurfaceSlider, CashedTransform, ref NominalMovementDirection, ref CorrectedDirection);
    }
    
    private void OnSelect(CharacterInputLayer characterInputLayer)
    {
        if (characterInputLayer != this)
        {
            return;
        }
        
        if (_selectedCharacter != null)
        {
            _selectedCharacter.UnsubscribeInputs();
            var newInput =  new AICharacterInput();
            _selectedCharacter.CharacterInput = newInput;
            _selectedCharacter.CurrentMovementType = MovementTypes.MovementType.None;
            _selectedCharacter.SubscribeInputs();
            CameraSystem.Select(null);
            RotateByCamera = false;
        }

        UnsubscribeInputs();
        _selectedCharacter = this;
        CharacterInput = _inputByPlayer;
        CurrentMovementType = MovementTypes.MovementType.RootMotion;
        SubscribeInputs();
        CameraSystem.Select(this);
        RotateByCamera = true;
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

    protected void ResetJump()
    {
        IsJump = false;
    }
    
    protected virtual void OnJump()
    {
        if (IsJump || !IsGrounded)
        {
            return;
        }
        IsJump = true;
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
        IsRunning = isRunning;
    }

    private void HandleSneak(bool isSneaking)
    {
        IsSneaking = isSneaking;
    }

    private void HandleTargetLock(bool isTargetLocked)
    {
        IsTargetLock = isTargetLocked;
    }

    protected virtual void HandleDrawWeapon(bool isDrawWeapon)
    {
        IsDrawWeapon = isDrawWeapon;
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
    
    private void OnDisable()
    {
        UnsubscribeInputs();
        OnCharacterSelected -= OnSelect;
        Health.OnDamage -= HandleDamage;
        Health.OnDeath -= HandleDeath;
    }
}
