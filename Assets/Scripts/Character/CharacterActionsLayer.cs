using System.Linq;
using System.Threading.Tasks;
using Unity.Burst;
using UnityEngine;
using Zenject;

[RequireComponent(typeof(EquipmentSystem))]
public class CharacterActionsLayer : CharacterAnimationParamsLayer
{
    private OneShotClipSetsContainer _oneShotClipSetsContainer;
    private AnimatorOverrideController _overrideController;
    private OneShotClip _blankAttack;
    private int _selectedWeaponIndex = 0;

    private int _animationSpeedHash;
    private ContainerInventory _containerInventory;
    private const string OneShotClipName = "Blank";

    [Inject]
    private void Construct(OneShotClipSetsContainer container, ContainerInventory containerInventory)
    {
        _oneShotClipSetsContainer = container;
        _containerInventory = containerInventory;
    }

    protected override void Initialize()
    {
        base.Initialize();
        var baseController = Animator.runtimeAnimatorController;
        _overrideController = new AnimatorOverrideController(baseController);
        Animator.runtimeAnimatorController = _overrideController;
        _animationSpeedHash = Animator.StringToHash("AnimationSpeed");
        EquipmentSystem.OnAnimationChanged += OnAnimationReset;
    }

    private void OnAnimationReset()
    {
        SetAnimationType(AnimationTypes.Type.Default); 
    }

    [BurstCompile]
    protected override void Take()
    {
        base.Take();

        if (ItemTargeting.Targets.Count < 1)
        {
            Debug.LogWarning("нечего подбирать");
            return;
        }

        if (IsDrawWeapon)
        {
            CharacterInput.ForciblyDrawWeapon(false);
        }
        
        if (this.TryTakeItem(ItemTargeting, EquipmentSystem))
        {
            return;
        }

        _containerInventory.OpenContainer(ItemTargeting);
    }

    [BurstCompile]
    protected override void HandleAttack()
    {
        if (!IsDrawWeapon || SwitchBoneValue != 0 || OneShotPlayedValue > 0)
        {
            return;
        }

        _blankAttack = null;

        _blankAttack = _oneShotClipSetsContainer.GetOneShotClip(EquipmentSystem.GetAnimationType(_selectedWeaponIndex));
        
        if (_blankAttack == null)
        {
            Debug.LogWarning("No OneShot Clip Set");
            return;
        }
        
        SetNewClipToState(_blankAttack.Clip, OneShotClipName);
        Animator.SetFloat(_animationSpeedHash, _blankAttack.Speed);
        Animator.SetTrigger("OneShotTrigger");
    }

    [BurstCompile]
    protected override async void HandleDrawWeapon(bool isDrawWeapon)
    {
        base.HandleDrawWeapon(isDrawWeapon);

        var hasWeapon = EquipmentSystem.WeaponData[_selectedWeaponIndex] != null;

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
            ? EquipmentSystem.GetAnimationType(0) 
            : AnimationTypes.Type.Unarmed);

        if (hasWeapon)
        {
            _ = SwitchSlotAsync(_selectedWeaponIndex, 1);
        }
    }

    [BurstCompile]
    private async Task SwitchSlotAsync(int weaponIndex, int slotIndex)
    {
        while (true)
        {
            if (SwitchBoneValue > 0f)
            {
                break;
            }
            await Task.Yield();
        }
       
        while (true)
        {
            if (SwitchBoneValue == 0f)
            {
                EquipmentSystem.WeaponData[weaponIndex].Equip(Skin.BonesCollector, slotIndex, EquipmentSystem.WeaponInstances[weaponIndex]);
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
        Animator.runtimeAnimatorController = _overrideController;
    }
    
    protected override void SelectWeapon0()
    {
        _ = SwitchWeaponAndSlot(0);
    }

    protected override void SelectWeapon1()
    {
        _ = SwitchWeaponAndSlot(1);
    }

    protected override void SelectWeapon2()
    {
       _ = SwitchWeaponAndSlot(3);
    }

    private async Task SwitchWeaponAndSlot(int index)
    {
        if (_selectedWeaponIndex == index)
        {
            return;
        }
       
        if (IsDrawWeapon)
        {
            CharacterInput.ForciblyDrawWeapon(false);
            await Task.Run(() => HandleDrawWeapon(false));
        }
        
        _selectedWeaponIndex = index;
    }

    private void OnDisable()
    {
        EquipmentSystem.OnAnimationChanged -= OnAnimationReset;
    }
}