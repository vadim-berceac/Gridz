using System.Threading.Tasks;
using Unity.Burst;
using UnityEngine;
using Zenject;

[RequireComponent(typeof(EquipmentSystem))]
public class CharacterActions : CharacterAnimationParams
{
    private EquipmentSystem _equipmentSystem;
    private OneShotClipSetsContainer _oneShotClipSetsContainer;

    [Inject]
    private void Construct(OneShotClipSetsContainer container)
    {
        _oneShotClipSetsContainer = container;
    }

    protected override void Initialize()
    {
        base.Initialize();
        _equipmentSystem = GetComponent<EquipmentSystem>();
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
        //дописать
        base.HandleAttack();
        var testanim = _oneShotClipSetsContainer.GetOneShotClip(_equipmentSystem.GetAnimationType());
        Debug.LogWarning(testanim?.name);
    }

    [BurstCompile]
    protected override async void HandleDrawWeapon(bool isDrawWeapon)
    {
        //переключение типов анимации слишком резкое из-за await
        //переписать с учетом _equipmentSystem.GetAnimationType()
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

        if (!_equipmentSystem.PrimaryWeaponData)
        {
            SetAnimationType(AnimationTypes.Type.Unarmed);
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
    
    //текущий OneShotClip
    protected override void HashParams()
    {
        base.HashParams();
        //находим хеш OneShotClip
    }
}
