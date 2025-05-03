using Unity.Burst;
using UnityEngine;

[RequireComponent(typeof(Animator))]
public abstract class CharacterAnimationParamsLayer : LocoMotionLayer
{
    public AnimationTypes.Type AnimationType { get; private set; } = AnimationTypes.Type.Default;
    public Animator Animator { get; private set; }

    public float OneShotPlayedValue { get; private set; }
    protected float SwitchBoneValue;
   
    protected override void Initialize()
    {
        base.Initialize();
        Animator = GetComponent<Animator>();
    }

    protected override void SubscribeInputs()
    {
        base.SubscribeInputs();
        CharacterInput.OnAttack += HandleAttackTrigger;
        CharacterInput.OnDrawWeapon += HandleDrawTrigger;
    }

    protected override void UnsubscribeInputs()
    {
        base.UnsubscribeInputs();
        CharacterInput.OnAttack -= HandleAttackTrigger;
        CharacterInput.OnDrawWeapon -= HandleDrawTrigger;
    }

    protected void SetAnimationType(AnimationTypes.Type animationType)
    {
        AnimationType = animationType;
    }
   
    protected override void Update()
    {
        base.Update();
        UpdateParams();
    }

    [BurstCompile]
    private void UpdateParams()
    {
        Animator.SetFloat(AnimationParams.AnimationType, (int)AnimationType, 0.1f, Time.deltaTime);
        Animator.SetBool(AnimationParams.Grounded, IsGrounded);
        Animator.SetBool(AnimationParams.Jump, IsJump);
        Animator.SetBool(AnimationParams.Run, IsRunning);
        Animator.SetBool(AnimationParams.Sneak, IsSneaking);
        Animator.SetBool(AnimationParams.TargetLock, IsTargetLock);
        Animator.SetBool(AnimationParams.DrawWeapon, IsDrawWeapon);
        Animator.SetFloat(AnimationParams.CurrentSpeedZ, CurrentSpeedZ);
        Animator.SetFloat(AnimationParams.CurrentSpeedX, CurrentSpeedX, 0.5f, Time.deltaTime);
        Animator.SetFloat(AnimationParams.InputX, CorrectedDirection.x, 0.2f, Time.deltaTime);
        Animator.SetFloat(AnimationParams.InputZ, CorrectedDirection.z, 0.2f, Time.deltaTime);
        SwitchBoneValue = Animator.GetFloat(AnimationParams.SwitchBoneCurve);
        OneShotPlayedValue = Animator.GetFloat(AnimationParams.OneShotPlayed);
    }

    private void HandleAttackTrigger()
    {
        if (OneShotPlayedValue > 0 || !IsDrawWeapon)
        {
            return;
        }
        Animator.SetTrigger(AnimationParams.OneShotTrigger);
    }

    protected override void HandleDamage(AnimationTypes.Type animationType, float value)
    {
        base.HandleDamage(animationType, value);
        Animator.SetTrigger(AnimationParams.HitTrigger);
    }

    protected override void HandleDeath(AnimationTypes.Type animationType, bool value)
    {
        base.HandleDeath(animationType, value);
        Animator.SetTrigger(AnimationParams.Dead);
    }

    private void HandleDrawTrigger(bool sda)
    {
        if (OneShotPlayedValue > 0)
        {
            return;
        }
        Animator.SetTrigger(AnimationParams.DrawTrigger);
    }
}
