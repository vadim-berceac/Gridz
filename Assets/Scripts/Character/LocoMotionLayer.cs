using Unity.Burst;
using UnityEngine;

[RequireComponent(typeof(CharacterSkinModule))]
public class LocoMotionLayer : CharacterInputLayer
{
    [field:Header("Rotation Settings")]
    [field: SerializeField] public float RotationSpeed { get; private set; } = 2f;
    
    [field:Header("Sneak Settings")]
    [field: SerializeField] public AnimationCurve ForwardSneakSpeedCurve { get; private set; } = new AnimationCurve();
    [field: SerializeField] public AnimationCurve BackWardSneakSpeedCurve { get; private set; } = new AnimationCurve();
    
    [field:Header("Walk Settings")]
    [field: SerializeField] public AnimationCurve ForwardWalkSpeedCurve { get; private set; } = new AnimationCurve();
    [field: SerializeField] public AnimationCurve BackWardWalkSpeedCurve { get; private set; } = new AnimationCurve();
   
    [field:Header("Run Settings")]
    [field: SerializeField] public AnimationCurve ForwardRunSpeedCurve { get; private set; } = new AnimationCurve();
    [field: SerializeField] public AnimationCurve BackWardRunSpeedCurve { get; private set; } = new AnimationCurve();
    
    [field:Header("Jump Settings")]
    [field: SerializeField] public float JumpDuration { get; private set; } = 0.5f;
    [field: SerializeField] public float JumpHeight { get; private set; } = 2f;
    
    [field:Header("Targeting Settings")]
    [field: SerializeField] public CharacterTargeting EnemyTargeting { get; private set; }
    [field: SerializeField] public ItemTargeting ItemTargeting { get; private set; }
   
    protected CharacterSkinModule Skin { get; private set; }
    
    protected float CurrentSpeedZ;
    protected float CurrentSpeedX;
    private bool _previousUsePlayerInput;
    private AnimationCurve _selectedMovementCurve;
    private const float SpeedChangeRate = 4f;

    protected override void Initialize()
    {
        base.Initialize();
        Skin = GetComponent<CharacterSkinModule>();
    }

    protected override void Update()
    {
        base.Update();
        CharacterInput.Correct(IsDead, SurfaceSlider, CashedTransform, ref NominalMovementDirection, ref CorrectedDirection);
        Speed.MoveCurve(CorrectedDirection.z, IsSneaking, IsRunning, ForwardSneakSpeedCurve, ForwardWalkSpeedCurve, ForwardRunSpeedCurve,
            BackWardSneakSpeedCurve, BackWardWalkSpeedCurve, BackWardRunSpeedCurve, ref _selectedMovementCurve);
        Speed.Update(IsDead, IsJump, SpeedChangeRate, CorrectedDirection, _selectedMovementCurve, ref CurrentSpeedX, ref CurrentSpeedZ);
        EnemyTargeting.Target(IsDead, IsTargetLock, CharacterInput);
        CashedTransform.Move(IsDead, IsTargetLock, IsJump, EnemyTargeting, CharacterController, CurrentMovementType,
            CorrectedDirection, CurrentSpeedZ, CurrentSpeedX);
        CashedTransform.Rotate(IsDead, IsTargetLock, EnemyTargeting, CurrentMovementType, RotateByCamera, CameraSystem,
            NominalMovementDirection, RotationSpeed);
    }

    [BurstCompile]
    protected override void OnJump()
    {
        base.OnJump();
        CharacterController.Jump(CurrentMovementType, CurrentSpeedZ, JumpHeight, JumpDuration, ResetJump);
    }
    
    [BurstCompile]
    private void OnCollisionEnter(Collision collision)
    {
        if (IsDead)
        {
            return;
        }
        SurfaceSlider.SetNormal(collision);
    }
}