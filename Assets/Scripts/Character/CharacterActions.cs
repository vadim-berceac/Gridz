using System.Linq;
using System.Threading.Tasks;
using Unity.Burst;
using UnityEngine;
using Zenject;
using UnityEditor.Animations;

[RequireComponent(typeof(EquipmentSystem))]
public class CharacterActions : CharacterAnimationParams
{
    private OneShotClipSetsContainer _oneShotClipSetsContainer;
    private AnimatorOverrideController _overrideController;
    private OneShotClip _blankAttack;
    private int _selectedWeaponIndex = 0;

    private int _animationSpeedHash;

    [Inject]
    private void Construct(OneShotClipSetsContainer container)
    {
        _oneShotClipSetsContainer = container;
    }

    protected override void Initialize()
    {
        base.Initialize();
        _overrideController = new AnimatorOverrideController(Animator.runtimeAnimatorController);
        Animator.runtimeAnimatorController = _overrideController;
        _animationSpeedHash = Animator.StringToHash("AnimationSpeed");
        EquipmentSystem.OnAnimationChanged += OnAnimationReseted;
    }

    private void OnAnimationReseted()
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

        var list = ItemTargeting.Targets.ToList();
        
        var target = list[0].GetComponent<PickupObject>().ItemData;
        
        InventoryCell.FindIndexOfEmpty(EquipmentSystem.InventoryBag, out var index);
        
        EquipmentSystem.InventoryBag[index] = target;

        Debug.LogWarning($"подбираю {target.name}");

        var toRemove = list[0].gameObject;
        
        ItemTargeting.Targets.Remove(toRemove.transform);
        
        Destroy(toRemove); 
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
        
        SetNewClipToState(_blankAttack.Clip, OneShotClipState);
        Animator.SetFloat(_animationSpeedHash, _blankAttack.Speed);
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
    private void SetNewClipToState(AnimationClip clip, AnimatorState state)
    {
        if (clip == null)
        {
            return;
        }
        _overrideController[state.motion.name] = clip;
    }
    
    protected override void SelectWeapon0()
    {
        _selectedWeaponIndex = 0;
    }

    protected override void SelectWeapon1()
    {
        _selectedWeaponIndex = 1;
    }

    protected override void SelectWeapon2()
    {
        _selectedWeaponIndex = 2;
    }

    private void OnDisable()
    {
        EquipmentSystem.OnAnimationChanged -= OnAnimationReseted;
    }
}
