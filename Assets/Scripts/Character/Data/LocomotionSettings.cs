using UnityEngine;

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
