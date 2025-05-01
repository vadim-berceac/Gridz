using Unity.Burst;
using UnityEngine;

public class ParamsUpdater
{
    private readonly Character _character;
    
    public ParamsUpdater(Character character)
    {
        _character = character;
    }

    [BurstCompile]
    public void UpdateParams()
    {
        _character.ComponentsSettings.AnimatorLocal.SetFloat(AnimationParams.AnimationType, (int)_character.AnimationType, 0.1f, Time.deltaTime);
        _character.ComponentsSettings.AnimatorLocal.SetBool(AnimationParams.Grounded, _character.IsGrounded);
        _character.ComponentsSettings.AnimatorLocal.SetBool( AnimationParams.Jump, _character.CharacterStates.IsJump);
        _character.ComponentsSettings.AnimatorLocal.SetBool(AnimationParams.Running, _character.CharacterStates.IsRunning);
        _character.ComponentsSettings.AnimatorLocal.SetBool(AnimationParams.Sneaking, _character.CharacterStates.IsSneaking);
        _character.ComponentsSettings.AnimatorLocal.SetBool(AnimationParams.TargetLock, _character.CharacterStates.IsTargetLock);
        _character.ComponentsSettings.AnimatorLocal.SetBool( AnimationParams.DrawWeapon, _character.CharacterStates.IsDrawWeapon);
        _character.ComponentsSettings.AnimatorLocal.SetFloat( AnimationParams.CurrentSpeedZ, _character.CurrentSpeedZ);
        _character.ComponentsSettings.AnimatorLocal.SetFloat(AnimationParams.CurrentSpeedX, _character.CurrentSpeedX, 0.5f, Time.deltaTime);
        _character.ComponentsSettings.AnimatorLocal.SetFloat(AnimationParams.InputX, _character.CorrectedDirection.x, 0.2f, Time.deltaTime);
        _character.ComponentsSettings.AnimatorLocal.SetFloat( AnimationParams.InputZ, _character.CorrectedDirection.z, 0.2f, Time.deltaTime);
        _character.SetSwitchBoneValue(_character.ComponentsSettings.AnimatorLocal.GetFloat(AnimationParams.SwitchBoneCurve));
        _character.SetOneShotClipPlayedValue(_character.ComponentsSettings.AnimatorLocal.GetFloat(AnimationParams.OneShotPlayed));
    }
}
