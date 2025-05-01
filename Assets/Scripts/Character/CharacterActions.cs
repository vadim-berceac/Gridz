using System.Linq;
using System.Threading.Tasks;
using Unity.Burst;
using UnityEngine;

public class CharacterStates
{
    private ICharacterInput _characterInput;
    private readonly Character _character;
    private OneShotClip _blankAttack;
    private int _selectedWeaponIndex;

    public bool IsDrawWeapon { get; private set; }
    public bool IsRunning { get; private set; }
    public bool IsSneaking { get; private set; }
    public bool IsTargetLock { get; private set; }
    public bool IsJump { get; private set; }
    public bool IsDead { get; private set; }
   
    public CharacterStates(Character character)
    {
        _character = character;
    }
    
    public void Subscribe(ICharacterInput currentCharacterInput)
    {
        _characterInput = currentCharacterInput;
        _characterInput.OnJump += OnJump;
        _characterInput.OnSneak += HandleSneak;
        _characterInput.OnSprint += HandleSprint;
        _characterInput.OnHoldTarget += HandleTargetLock;
        _characterInput.OnDrawWeapon += HandleDrawWeapon;
        _characterInput.OnAttack += HandleAttack;
        _characterInput.OnInteract += Take;
        _characterInput.OnWeaponSelect0 += SelectWeapon0;
        _characterInput.OnWeaponSelect1 += SelectWeapon1;
        _characterInput.OnWeaponSelect2 += SelectWeapon2;
        _characterInput.OnAttack += HandleAttackTrigger;
        _characterInput.OnDrawWeapon += HandleDrawTrigger;
        _character.ComponentsSettings.Health.OnDamage += HandleDamage;
        _character.ComponentsSettings.Health.OnDeath += HandleDeath;
    }
    
    public void Unsubscribe()
    {
        if (_characterInput == null)
        {
            return;
        }
        _characterInput.OnJump -= OnJump;
        _characterInput.OnSneak -= HandleSneak;
        _characterInput.OnSprint -= HandleSprint;
        _characterInput.OnHoldTarget -= HandleTargetLock;
        _characterInput.OnDrawWeapon -= HandleDrawWeapon;
        _characterInput.OnAttack -= HandleAttack;
        _characterInput.OnInteract -= Take;
        _characterInput.OnWeaponSelect0 -= SelectWeapon0;
        _characterInput.OnWeaponSelect1 -= SelectWeapon1;
        _characterInput.OnWeaponSelect2 -= SelectWeapon2;
        _characterInput.OnAttack -= HandleAttackTrigger;
        _characterInput.OnDrawWeapon -= HandleDrawTrigger;
        _character.ComponentsSettings.Health.OnDamage -= HandleDamage;
        _character.ComponentsSettings.Health.OnDeath -= HandleDeath;
        _characterInput = null;
    }
    
    
    private void HandleAttackTrigger()
    {
        if (_character.OneShotClipPlayedValue > 0 || !IsDrawWeapon)
        {
            return;
        }
        _character.ComponentsSettings.AnimatorLocal.SetTrigger( AnimationParams.OneShotTrigger);
    }


    private void HandleDrawTrigger(bool sda)
    {
        if (_character.OneShotClipPlayedValue > 0)
        {
            return;
        }
        _character.ComponentsSettings.AnimatorLocal.SetTrigger(AnimationParams.DrawTrigger);
    }
    
    private void HandleDamage(AnimationTypes.Type animationType, float value)
    {
        if (value <= 0)
        {
            return;
        }
        Debug.LogWarning($"{_character.name} получил урон {value} от {animationType}, " +
                         $"осталось {_character.ComponentsSettings.Health.CurrentHealth}/{_character.ComponentsSettings.Health.MaxHealth}");
        
        _character.ComponentsSettings.AnimatorLocal.SetTrigger(AnimationParams.HitTrigger);
    }

    private void HandleDeath(AnimationTypes.Type animationType, bool value)
    {
        IsDead = value;
        if (IsDead)
        {
            Unsubscribe();
            _character.SetInput(new AICharacterInput()); 
            _character.CurrentCharacterInput.EnableCharacterInput(false);
            _character.gameObject.layer = TagsAndLayersConst.PickupObjectLayerIndex;
        }
        Debug.LogWarning($"{_character.name} убит {value} от {animationType}");
        
        _character.ComponentsSettings.AnimatorLocal.SetTrigger(AnimationParams.Dead);
    }
    
    private void SelectWeapon0()
    {
        _ = SwitchWeaponAndSlot(0);
    }

    private void SelectWeapon1()
    {
        _ = SwitchWeaponAndSlot(1);
    }

    private void SelectWeapon2()
    {
        _ = SwitchWeaponAndSlot(2);
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
    
    private void HandleAttack()
    {
        if (!IsDrawWeapon || _character.SwitchBoneValue != 0 || _character.OneShotClipPlayedValue > 0)
        {
            return;
        }

        _blankAttack = null;

        _blankAttack = _character.OneShotClipSetsContainer.GetOneShotClip(_character.ComponentsSettings.Equipment.GetAnimationType(_selectedWeaponIndex));
        
        if (_blankAttack == null)
        {
            Debug.LogWarning("No OneShot Clip Set");
            return;
        }
        
        SetNewClipToState(_blankAttack.Clip, AnimationParams.OneShotClipName);
        _character.ComponentsSettings.AnimatorLocal.SetFloat(AnimationParams.AnimationSpeed, _blankAttack.Speed);
        _character.ComponentsSettings.AnimatorLocal.SetTrigger(AnimationParams.OneShotTrigger);
    }

    private void Take()
    {
        if (_character.TargetingSettings.ItemTargeting.Targets.Count < 1)
        {
            Debug.LogWarning("нечего подбирать");
            return;
        }

        if (IsDrawWeapon)
        {
            _character.CurrentCharacterInput.ForciblyDrawWeapon(false);
        }
        
        if (TryTakeItem(_character.TargetingSettings.ItemTargeting, _character.ComponentsSettings.Equipment))
        {
            return;
        }

        _character.LootInventory.OpenContainer(_character.TargetingSettings.ItemTargeting);
    }
    
    private void OnJump()
    {
        if (IsJump || !_character.IsGrounded)
        {
            return;
        }
        IsJump = true;
        
        if (IsDead)
        {
            return;
        }
        _character.CharacterController.Jump(_character.CurrentMovementType, _character.CurrentSpeedZ, _character.LocoMotionSettings.JumpHeight, _character.LocoMotionSettings.JumpDuration, ResetJump);
    }
    
    private void ResetJump()
    {
        IsJump = false;
    }
    
    private async void HandleDrawWeapon(bool isDrawWeapon)
    {
        IsDrawWeapon = isDrawWeapon;
        
        var hasWeapon = _character.ComponentsSettings.Equipment.WeaponData[_selectedWeaponIndex] != null;

        if (!isDrawWeapon)
        {
            if (!hasWeapon)
            {
                _character.SetAnimationType(AnimationTypes.Type.Default);
                return;
            }

            await SwitchSlotAsync(_selectedWeaponIndex, 0);
            _character.SetAnimationType(AnimationTypes.Type.Default);
            return;
        }

        _character.SetAnimationType(hasWeapon 
            ? _character.ComponentsSettings.Equipment.GetAnimationType(0) 
            : AnimationTypes.Type.Unarmed);

        if (hasWeapon)
        {
            _ = SwitchSlotAsync(_selectedWeaponIndex, 1);
        }
    }
    
    private static bool TryTakeItem(ItemTargeting itemTargeting, EquipmentModule equipmentModule)
    {
        var list = itemTargeting.Targets.ToList();
        
        list[0].TryGetComponent<PickupObject>(out var item);

        if (item == null)
        {
            return false;
        }
        
        InventoryCell.FindIndexOfEmpty(equipmentModule.InventoryBag, out var index);
        
        equipmentModule.InventoryBag[index] = item.ItemData;

        Debug.LogWarning($"подбираю {item.name}");

        var toRemove = list[0].gameObject;
        
        itemTargeting.Targets.Remove(toRemove.transform);
        
        Object.Destroy(toRemove); 
        
        return true;
    }

    [BurstCompile]
    private async Task SwitchSlotAsync(int weaponIndex, int slotIndex)
    {
        while (true)
        {
            if (_character.SwitchBoneValue > 0f)
            {
                break;
            }
            await Task.Yield();
        }
       
        while (true)
        {
            if (_character.SwitchBoneValue == 0f)
            {
                _character.ComponentsSettings.Equipment.WeaponData[weaponIndex].Equip(_character.ComponentsSettings.CharacterPersonality.BonesCollector,
                    slotIndex, _character.ComponentsSettings.Equipment.WeaponInstances[weaponIndex]);
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
        _character.OverrideController[clipName] = clip;
        _character.ComponentsSettings.AnimatorLocal.runtimeAnimatorController = _character.OverrideController;
    }
    

    private async Task SwitchWeaponAndSlot(int index)
    {
        if (_selectedWeaponIndex == index)
        {
            return;
        }
       
        if (IsDrawWeapon)
        {
            _character.CurrentCharacterInput.ForciblyDrawWeapon(false);
            await Task.Run(() => HandleDrawWeapon(false));
        }
        
        _selectedWeaponIndex = index;
    }
}
