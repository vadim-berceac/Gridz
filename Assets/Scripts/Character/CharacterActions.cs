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
    protected override void HandleDrawWeapon(bool isDrawWeapon)
    {
        base.HandleDrawWeapon(isDrawWeapon);
      
        if (!isDrawWeapon)
        {
            SetAnimationType(AnimationTypes.Type.Default);
            if (!Weapon)
            {
                return;
            }
            _ = EquipAsync(0);
            return;
        }

        if (!Weapon)
        {
            SetAnimationType(AnimationTypes.Type.Unarmed);
            return;
        }
        _ = EquipAsync(1);
        SetAnimationType(Weapon.AnimationType);
    }
    
    [BurstCompile]
    private async Task EquipAsync(int slotIndex)
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
