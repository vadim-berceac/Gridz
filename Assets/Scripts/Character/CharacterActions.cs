using System.Threading.Tasks;
using Unity.Burst;
using UnityEditor.Animations;
using UnityEngine;
using Zenject;

[RequireComponent(typeof(EquipmentSystem))]
public class CharacterActions : CharacterAnimationParams
{
    private EquipmentSystem _equipmentSystem;
    private OneShotClipSetsContainer _oneShotClipSetsContainer;
    private AnimatorOverrideController _overrideController;

    [Inject]
    private void Construct(OneShotClipSetsContainer container)
    {
        _oneShotClipSetsContainer = container;
    }

    protected override void Initialize()
    {
        base.Initialize();
        _equipmentSystem = GetComponent<EquipmentSystem>();
        _overrideController = new AnimatorOverrideController(Animator.runtimeAnimatorController);
        Animator.runtimeAnimatorController = _overrideController;
    }
    
    private void Start()
    {
        if (!_equipmentSystem.PrimaryWeaponData)
        {
            return;
        }
        _equipmentSystem.PrimaryWeaponData.Equip(Skin.BonesCollector, 0, _equipmentSystem.PrimaryWeaponInstance);
    }

    [BurstCompile]
    protected override void HandleAttack()
    {
        if (!IsDrawWeapon || SwitchBoneValue != 0 || OneShotPlayedValue > 0)
        {
            return;
        }

        var oneShot = _oneShotClipSetsContainer.GetOneShotClip(_equipmentSystem.GetAnimationType());
        if (oneShot == null)
        {
            Debug.LogWarning("No OneShot Clip Set");
            return;
        }
        
        SetNewClipToState(oneShot.Clip, OneShotClipState);
        Animator.SetFloat("AnimationSpeed", oneShot.Speed);
    }

    [BurstCompile]
    protected override async void HandleDrawWeapon(bool isDrawWeapon)
    {
        base.HandleDrawWeapon(isDrawWeapon);
    
        if (!isDrawWeapon)
        {
            if (!_equipmentSystem.PrimaryWeaponData)
            {
                SetAnimationType(AnimationTypes.Type.Default);
                return;
            }
        
            await EquipToSlotAsync(0);
            SetAnimationType(AnimationTypes.Type.Default);
            return;
        }
        
        SetAnimationType(_equipmentSystem.GetAnimationType());
        _ = EquipToSlotAsync(1);
    }
    
    [BurstCompile]
    private async Task EquipToSlotAsync(int slotIndex)
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
                _equipmentSystem.PrimaryWeaponData.Equip(Skin.BonesCollector, slotIndex, _equipmentSystem.PrimaryWeaponInstance);
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
}
