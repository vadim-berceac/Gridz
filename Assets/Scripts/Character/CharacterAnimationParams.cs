
using Unity.Burst;
using UnityEngine;

[RequireComponent(typeof(Animator))]
public abstract class CharacterAnimationParams : LocoMotion
{
   public Animator Animator { get; private set; }

   private int _groundedHash;
   private int _jumpHash;
   private int _runningHash;
   private int _sneakingHash;
   private int _targetLockHash;
   private int _drawWeaponHash;
   private int _currentSpeedZHash;
   private int _currentSpeedXHash;
   private int _oneShotTriggerHash;
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
   }

   protected override void UnsubscribeInputs()
   {
      base.UnsubscribeInputs();
      CharacterInput.OnAttack -= HandleAttackTrigger;
   }

   protected override void Update()
   {
      base.Update();
      UpdateParams();
   }

   [BurstCompile]
   protected virtual void HashParams()
   {
      _groundedHash = Animator.StringToHash("Grounded");
      _jumpHash = Animator.StringToHash("Jump");
      _runningHash = Animator.StringToHash("Run");
      _sneakingHash = Animator.StringToHash("Sneak");
      _targetLockHash = Animator.StringToHash("TargetLock");
      _drawWeaponHash = Animator.StringToHash("DrawWeapon");
      _currentSpeedZHash = Animator.StringToHash("CurrentSpeedZ");
      _currentSpeedXHash = Animator.StringToHash("CurrentSpeedX");
      _oneShotTriggerHash = Animator.StringToHash("OneShotTrigger");
      _inputXHash = Animator.StringToHash("InputX");
      _inputZHash = Animator.StringToHash("InputZ");
   }

   [BurstCompile]
   protected virtual void UpdateParams()
   {
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
   }

   private void HandleAttackTrigger()
   {
      Animator.SetTrigger(_oneShotTriggerHash);
   }
}
