using Unity.Burst;
using UnityEngine;

[RequireComponent(typeof(CharacterPersonalityModule))]
public class LocoMotionLayer : CharacterInputLayer
{
    [field: SerializeField] public LocoMotionSettings LocoMotionSettings { get; private set; }
    [field: SerializeField] public TargetingSettings TargetingSettings { get; private set; }
   
    protected CharacterPersonalityModule Personality { get; private set; }
    
    protected float CurrentSpeedZ;
    protected float CurrentSpeedX;
    private bool _previousUsePlayerInput;
    private AnimationCurve _selectedMovementCurve;
    private const float SpeedChangeRate = 4f;

    protected override void Initialize()
    {
        base.Initialize();
        Personality = GetComponent<CharacterPersonalityModule>();
    }

    protected override void Update()
    {
        base.Update();
        CharacterInput.Correct(IsDead, SurfaceSlider, CashedTransform, ref NominalMovementDirection, ref CorrectedDirection);
        
        Speed.MoveCurve(CorrectedDirection.z, IsSneaking, IsRunning, LocoMotionSettings.ForwardSneakSpeedCurve, LocoMotionSettings.ForwardWalkSpeedCurve, 
            LocoMotionSettings.ForwardRunSpeedCurve, LocoMotionSettings.BackWardSneakSpeedCurve, LocoMotionSettings.BackWardWalkSpeedCurve, 
            LocoMotionSettings.BackWardRunSpeedCurve, ref _selectedMovementCurve);
        
        Speed.Update(IsDead, IsJump, SpeedChangeRate, CorrectedDirection, _selectedMovementCurve, ref CurrentSpeedX, ref CurrentSpeedZ);
        TargetingSettings.EnemyTargeting.SetTargetingColliderRadius(TargetingSettings.MaxDistance);
        TargetingSettings.EnemyTargeting.Target(IsDead, IsTargetLock, TargetingSettings.MaxDistance);
        
        CashedTransform.Rotate(IsDead, IsTargetLock, TargetingSettings.EnemyTargeting, RotateByCamera, CameraSystem, NominalMovementDirection, LocoMotionSettings.RotationSpeed);
    }

    [BurstCompile]
    protected override void OnJump()
    {
        base.OnJump();
        if (IsDead)
        {
            return;
        }
        CharacterController.Jump(CurrentSpeedZ, LocoMotionSettings.JumpHeight, LocoMotionSettings.JumpDuration, ResetJump);
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

[System.Serializable]
public struct LocoMotionSettings
{
    [field: Header("Rotation Settings")]
    [field: SerializeField]
    public float RotationSpeed { get; private set; }

    [field: Header("Sneak Settings")]
    [field: SerializeField]
    public AnimationCurve ForwardSneakSpeedCurve { get; private set; }

    [field: SerializeField] public AnimationCurve BackWardSneakSpeedCurve { get; private set; }

    [field: Header("Walk Settings")]
    [field: SerializeField]
    public AnimationCurve ForwardWalkSpeedCurve { get; private set; }

    [field: SerializeField] public AnimationCurve BackWardWalkSpeedCurve { get; private set; }

    [field: Header("Run Settings")]
    [field: SerializeField]
    public AnimationCurve ForwardRunSpeedCurve { get; private set; }

    [field: SerializeField] public AnimationCurve BackWardRunSpeedCurve { get; private set; }

    [field: Header("Jump Settings")]
    [field: SerializeField]
    public float JumpDuration { get; private set; }

    [field: SerializeField] public float JumpHeight { get; private set; }
}

[System.Serializable]
public struct TargetingSettings
{
    [field: SerializeField] public CharacterTargeting EnemyTargeting { get; private set; }
    [field: SerializeField] public ItemTargeting ItemTargeting { get; private set; }
    [field: SerializeField] public float MaxDistance { get; private set; }
}