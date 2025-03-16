using System.Threading.Tasks;
using Unity.Burst;
using UnityEngine;

[RequireComponent(typeof(EquipmentSystem))]
public class CharacterActions : CharacterAnimationParams
{
    private EquipmentSystem _equipmentSystem;

    protected override void Initialize()
    {
        base.Initialize();
        _equipmentSystem = GetComponent<EquipmentSystem>();
    }
    
    private void Start()
    {
        if (!_equipmentSystem.PrimaryWeaponModelHolder)
        {
            return;
        }
        _equipmentSystem.PrimaryWeaponModelHolder.Pickup();
        _equipmentSystem.PrimaryWeaponModelHolder.Equip(Skin.BonesCollector, 0);
    }

    [BurstCompile]
    protected override async void HandleDrawWeapon(bool isDrawWeapon)
    {
        //переключение типов анимации слишком резкое из-за await
        base.HandleDrawWeapon(isDrawWeapon);
    
        if (!isDrawWeapon)
        {
            if (!_equipmentSystem.PrimaryWeaponModelHolder)
            {
                SetAnimationType(AnimationTypes.Type.Default);
                return;
            }
        
            await EquipToSlotAsync(0);
            SetAnimationType(AnimationTypes.Type.Default);
            return;
        }

        if (!_equipmentSystem.PrimaryWeaponModelHolder)
        {
            SetAnimationType(AnimationTypes.Type.Unarmed);
            return;
        }
        
        SetAnimationType(_equipmentSystem.PrimaryWeaponModelHolder.AnimationType);
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
                _equipmentSystem.PrimaryWeaponModelHolder.Equip(Skin.BonesCollector, slotIndex);
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
