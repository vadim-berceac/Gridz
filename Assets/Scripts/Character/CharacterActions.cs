using System.Threading.Tasks;
using Unity.Burst;
using UnityEngine;

public class CharacterActions : CharacterAnimationParams
{
    [field: Header("Test Weapon")]
    [field: SerializeField] public Weapon Weapon { get; private set; }
    
    private void Start()
    {
        if (!Weapon)
        {
            return;
        }
        Weapon.Pickup();
        Weapon.Equip(Skin.BonesCollector, 0);
    }

    [BurstCompile]
    protected override async void HandleDrawWeapon(bool isDrawWeapon)
    {
        base.HandleDrawWeapon(isDrawWeapon);
    
        if (!isDrawWeapon)
        {
            if (!Weapon)
            {
                SetAnimationType(AnimationTypes.Type.Default);
                return;
            }
        
            await EquipToSlotAsync(0);
            SetAnimationType(AnimationTypes.Type.Default);
            return;
        }

        if (!Weapon)
        {
            SetAnimationType(AnimationTypes.Type.Unarmed);
            return;
        }
        
        SetAnimationType(Weapon.AnimationType);
        await EquipToSlotAsync(1);
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
                Weapon.Equip(Skin.BonesCollector, slotIndex);
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
