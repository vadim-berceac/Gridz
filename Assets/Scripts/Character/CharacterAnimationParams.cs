using Unity.Burst;
using UnityEngine;

[RequireComponent(typeof(Animator))]
public abstract class CharacterAnimationParams : LocoMotion
{
   [field:Header("Animation")]
   [field: SerializeField] public AnimationTypes.Type AnimationType { get; private set; } = AnimationTypes.Type.Default;
   public Animator Animator { get; private set; }
  
   protected float SwitchBoneValue;

   private int _animationTypeHash;
   private int _switchBoneCurveHash;
   private int _groundedHash;
   private int _jumpHash;
   private int _runningHash;
   private int _sneakingHash;
   private int _targetLockHash;
   private int _drawWeaponHash;
   private int _currentSpeedZHash;
   private int _currentSpeedXHash;
   private int _oneShotTriggerHash;
   private int _drawTriggerHash;
   private int _inputXHash;
   private int _inputZHash;

   protected override void Initialize()
   {
      base.Initialize();
      Animator = GetComponent<Animator>();
      HashParams();
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
   protected virtual void HashParams()
   {
      _animationTypeHash = Animator.StringToHash("AnimationType");
      _switchBoneCurveHash = Animator.StringToHash("SwitchBoneCurve");
      _groundedHash = Animator.StringToHash("Grounded");
      _jumpHash = Animator.StringToHash("Jump");
      _runningHash = Animator.StringToHash("Run");
      _sneakingHash = Animator.StringToHash("Sneak");
      _targetLockHash = Animator.StringToHash("TargetLock");
      _drawWeaponHash = Animator.StringToHash("DrawWeapon");
      _currentSpeedZHash = Animator.StringToHash("CurrentSpeedZ");
      _currentSpeedXHash = Animator.StringToHash("CurrentSpeedX");
      _oneShotTriggerHash = Animator.StringToHash("OneShotTrigger");
      _drawTriggerHash = Animator.StringToHash("DrawTrigger");
      _inputXHash = Animator.StringToHash("InputX");
      _inputZHash = Animator.StringToHash("InputZ");
   }

   [BurstCompile]
   protected virtual void UpdateParams()
   {
      Animator.SetFloat(_animationTypeHash, (int) AnimationType, 0.1f, Time.deltaTime);
      Animator.SetBool(_groundedHash, IsGrounded);
      Animator.SetBool(_jumpHash, IsJump);
      Animator.SetBool(_runningHash, IsRunning);
      Animator.SetBool(_sneakingHash, IsSneaking);
      Animator.SetBool(_targetLockHash, IsTargetLock);
      Animator.SetBool(_drawWeaponHash, IsDrawWeapon);
      Animator.SetFloat(_currentSpeedZHash, CurrentSpeedZ);
      Animator.SetFloat(_currentSpeedXHash, CurrentSpeedX, 0.5f, Time.deltaTime);
      Animator.SetFloat(_inputXHash, CorrectedDirection.x, 0.2f, Time.deltaTime);
      Animator.SetFloat(_inputZHash, CorrectedDirection.z, 0.2f, Time.deltaTime);
      SwitchBoneValue = Animator.GetFloat(_switchBoneCurveHash);
   }

   private void HandleAttackTrigger()
   {
      Animator.SetTrigger(_oneShotTriggerHash);
   }

   private void HandleDrawTrigger(bool sda)
   {
      Animator.SetTrigger(_drawTriggerHash);
   }
}
