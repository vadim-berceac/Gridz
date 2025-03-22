using System.Linq;
using System.Threading.Tasks;
using Unity.Burst;
using UnityEditor.Animations;
using UnityEngine;
using UnityEngine.TextCore.Text;
using Zenject;

[RequireComponent(typeof(EquipmentSystem))]
public class CharacterActions : CharacterAnimationParams
{
    private OneShotClipSetsContainer _oneShotClipSetsContainer;
    private AnimatorOverrideController _overrideController;
    private OneShotClip _blankAttack;

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
    }
    
    private void Start()
    {
        if (!EquipmentSystem.WeaponData[0])
        {
            return;
        }
        EquipmentSystem.WeaponData[0].Equip(Skin.BonesCollector, 0, EquipmentSystem.PrimaryWeaponInstance);
    }

    [BurstCompile]
    protected override void HandleInteract()
    {
        base.HandleInteract();

        if (ItemTargeting.Targets.Count < 1)
        {
            Debug.LogWarning("нечего подбирать");
            return;
        }

        var list = ItemTargeting.Targets.ToList();
        
        var target = list[0].GetComponent<PickupObject>().ItemData;
        
        EquipmentSystem.InventoryBag.Add(target);

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

        _blankAttack = _oneShotClipSetsContainer.GetOneShotClip(EquipmentSystem.GetAnimationType());
        
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
    
        if (!isDrawWeapon)
        {
            if (!EquipmentSystem.WeaponData[0])
            {
                SetAnimationType(AnimationTypes.Type.Default);
                return;
            }
        
            await EquipToSlotAsync(0);
            SetAnimationType(AnimationTypes.Type.Default);
            return;
        }
        
        SetAnimationType(EquipmentSystem.GetAnimationType());
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
                EquipmentSystem.WeaponData[0].Equip(Skin.BonesCollector, slotIndex, EquipmentSystem.PrimaryWeaponInstance);
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
