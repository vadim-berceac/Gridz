using UnityEngine;
using UnityEngine.Events;
using Zenject;

[RequireComponent(typeof(HealthModule))]
public class CharacterInputLayer : GravitationLayer
{
    public static UnityAction<CharacterInputLayer> OnCharacterSelected;
    public HealthModule Health { get; private set; }
    protected bool IsJump {get;private set;}
    protected bool IsDead {get;private set;}
    protected bool IsRunning  { get; private set; }
    protected bool IsSneaking  { get; private set; }
    protected bool IsTargetLock  { get; private set; }
    protected bool IsDrawWeapon  { get; private set; }
    protected Vector2 NominalMovementDirection;
    public static CharacterInputLayer SelectedCharacter { get; private set; }
    
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

    protected virtual void Update()
    {
        CharacterInput.Correct(IsDead, SurfaceSlider, CashedTransform, ref NominalMovementDirection, ref CorrectedDirection);
    }
    
    private void OnSelect(CharacterInputLayer characterInputLayer)
    {
        if (characterInputLayer != this)
        {
            return;
        }
        
        if (SelectedCharacter != null)
        {
            SelectedCharacter.UnsubscribeInputs();
            var newInput =  new AICharacterInput();
            SelectedCharacter.CharacterInput = newInput;
            SelectedCharacter.CurrentMovementType = MovementTypes.MovementType.None;
            SelectedCharacter.SubscribeInputs();
            CameraSystem.Select(null);
            RotateByCamera = false;
        }

        UnsubscribeInputs();
        SelectedCharacter = this;
        if (!IsDead)
        {
            CharacterInput = _inputByPlayer;
            CurrentMovementType = MovementTypes.MovementType.RootMotion;
            SubscribeInputs();
        }
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
            UnsubscribeInputs();
            CharacterInput = new AICharacterInput();
            CharacterInput.EnableCharacterInput(false);
            gameObject.layer = TagsAndLayersConst.PickupObjectLayerIndex;
        }
        Debug.LogWarning($"{name} убит {value} от {animationType}");
    }
    
    private void HandleSprint()
    {
        IsRunning = !IsRunning;
    }

    private void HandleSneak()
    {
        IsSneaking = !IsSneaking;
    }

    private void HandleTargetLock()
    {
        IsTargetLock = !IsTargetLock;
    }

    protected virtual void HandleDrawWeapon()
    {
        IsDrawWeapon = !IsDrawWeapon;
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
