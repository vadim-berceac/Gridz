using Unity.Burst;
using UnityEngine;

public static class Speed
{
    [BurstCompile]
    public static void Update(bool isDead, bool isJumping, float speedChangeRate, Vector3 correctedDirection,
        AnimationCurve selectedMovementCurve, ref float currentSpeedX, ref float currentSpeedZ)
    {
        if (isDead)
        {
            currentSpeedX = 0;
            currentSpeedZ = 0;
            return;
        }
        var speedZ = Mathf.Clamp01(Mathf.Abs(correctedDirection.z));
        var speedX = Mathf.Clamp01(Mathf.Abs(correctedDirection.x));

        if (isJumping)
        {
            return;
        }
        
        currentSpeedX = speedX * currentSpeedZ;
        
        var targetSpeed = selectedMovementCurve.Evaluate(speedZ * selectedMovementCurve.keys[selectedMovementCurve.length - 1].time);

        currentSpeedZ = Mathf.MoveTowards(currentSpeedZ, targetSpeed, Time.deltaTime * speedChangeRate);
    }
    
    [BurstCompile]
    public static void MoveCurve(float correctedDirectionZ, bool isSneaking, bool isRunning, 
        AnimationCurve forwardSneakSpeedCurve, AnimationCurve forwardWalkSpeedCurve, AnimationCurve forwardRunSpeedCurve,
        AnimationCurve backWardSneakSpeedCurve, AnimationCurve backWardWalkSpeedCurve, AnimationCurve backWardRunSpeedCurve,
        ref AnimationCurve selectedMovementCurve)
    {
        var isForward = correctedDirectionZ > 0;

        selectedMovementCurve = isForward
            ? (isSneaking ? forwardSneakSpeedCurve : isRunning ? forwardRunSpeedCurve : forwardWalkSpeedCurve)
            : (isSneaking ? backWardSneakSpeedCurve : isRunning ? backWardRunSpeedCurve : backWardWalkSpeedCurve);
    }
}
