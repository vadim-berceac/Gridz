using Unity.Burst;
using UnityEngine;

[RequireComponent(typeof(Animator))]
public abstract class CharacterAnimationParamsLayer : LocoMotionLayer
{
    [field: SerializeField] public CharacterAnimationParamsSettings CharacterAnimationParamsSettings { get; private set; }
    public AnimationTypes.Type AnimationType { get; private set; } = AnimationTypes.Type.Default;
    public Animator Animator { get; private set; }

    public float OneShotPlayedValue { get; private set; }
    public float FootStepsCurveValue { get; private set; }
    protected float SwitchBoneValue;
    private float _currentIdleTimer;
    public bool InputDetected { get; private set; }
   
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
        Debug.Log(animationType);
        AnimationType = animationType;
    }
   
    protected override void Update()
    {
        base.Update();
        InputDetected = CurrentSpeedX > 0 || CurrentSpeedZ > 0 || OneShotPlayedValue > 0;
        UpdateParams();
    }

    protected override void FixedUpdate()
    {
        base.FixedUpdate();
        TimeoutToIdle();
    }

    [BurstCompile]
    private void UpdateParams()
    {
        Animator.SetFloat(AnimationParams.AnimationType, (int)AnimationType);
        Animator.SetBool(AnimationParams.Grounded, IsGrounded);
        Animator.SetBool(AnimationParams.Jump, IsJump);
        Animator.SetBool(AnimationParams.Run, IsRunning);
        Animator.SetBool(AnimationParams.Sneak, IsSneaking);
        Animator.SetBool(AnimationParams.TargetLock, IsTargetLock);
        Animator.SetBool(AnimationParams.DrawWeapon, IsDrawWeapon);
        Animator.SetFloat(AnimationParams.CurrentSpeedZ, CurrentSpeedZ);
        Animator.SetFloat(AnimationParams.CurrentSpeedX, CurrentSpeedX);
        Animator.SetFloat(AnimationParams.InputX, CorrectedDirection.x, 0.2f, Time.deltaTime);
        Animator.SetFloat(AnimationParams.InputZ, CorrectedDirection.z, 0.2f, Time.deltaTime);
        SwitchBoneValue = Animator.GetFloat(AnimationParams.SwitchBoneCurve);
        OneShotPlayedValue = Animator.GetFloat(AnimationParams.OneShotPlayed);
        FootStepsCurveValue = Animator.GetFloat(AnimationParams.FootStepsCurve);
        Animator.SetBool(AnimationParams.InputDetected, InputDetected);
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

    private void HandleDrawTrigger()
    {
        if (OneShotPlayedValue > 0)
        {
            return;
        }
        Animator.SetTrigger(AnimationParams.DrawTrigger);
    }
    
    private void TimeoutToIdle()
    {
        
        if (IsGrounded && !IsDead && !InputDetected)
        {
            _currentIdleTimer += Time.deltaTime;

            if (_currentIdleTimer >= CharacterAnimationParamsSettings.IdleTimeOut)
            {
                _currentIdleTimer = 0f;
                Animator.SetTrigger(AnimationParams.IdleTimeOutTrigger);
            }
        }
        else
        {
            _currentIdleTimer = 0f;
            Animator.ResetTrigger(AnimationParams.IdleTimeOutTrigger);
        }
    }
}

[System.Serializable]
public struct CharacterAnimationParamsSettings
{
    [field: SerializeField] public float IdleTimeOut { get; private set; }
}
