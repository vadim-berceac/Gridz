
using Unity.Burst;
using UnityEngine;

[RequireComponent(typeof(Animator))]
public abstract class CharacterAnimationParams : CharacterMovement
{
   public Animator Animator { get; private set; }

   private int _groundedHash;
   private int _jumpHash;
   private int _runningHash;
   private int _sneakingHash;
   private int _currentSpeedHash;
   private int _inputXHash;
   private int _inputZHash;

   protected override void Initialize()
   {
      base.Initialize();
      Animator = GetComponent<Animator>();
      HashParams();
   }

   protected override void Update()
   {
      base.Update();
      UpdateParams();
   }

   [BurstCompile]
   private void HashParams()
   {
      _groundedHash = Animator.StringToHash("Grounded");
      _jumpHash = Animator.StringToHash("Jump");
      _runningHash = Animator.StringToHash("Run");
      _sneakingHash = Animator.StringToHash("Sneak");
      _currentSpeedHash = Animator.StringToHash("CurrentSpeed");
      _inputXHash = Animator.StringToHash("InputX");
      _inputZHash = Animator.StringToHash("InputZ");
   }

   [BurstCompile]
   private void UpdateParams()
   {
      Animator.SetBool(_groundedHash, IsGrounded);
      Animator.SetBool(_jumpHash, IsJump);
      Animator.SetBool(_runningHash, IsRunning);
      Animator.SetBool(_sneakingHash, IsSneaking);
      Animator.SetFloat(_currentSpeedHash, CurrentSpeed);
      Animator.SetFloat(_inputXHash, CorrectedDirection.x);
      Animator.SetFloat(_inputZHash, CorrectedDirection.z);
   }
}
