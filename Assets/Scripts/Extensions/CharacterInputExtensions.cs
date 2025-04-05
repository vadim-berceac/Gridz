using Unity.Burst;
using UnityEngine;

public static class CharacterInputExtensions
{
    [BurstCompile]
    public static void Correct(this ICharacterInput instance, bool isDead, SurfaceSlider surfaceSlider, 
        Transform cashedTransform, ref Vector2 nominalMovementDirection, ref Vector3 correctedDirection)
    {
        if (isDead)
        {
            return;
        }
        nominalMovementDirection = instance.GetMoveDirection();
        
        correctedDirection = surfaceSlider.UpdateDirection(cashedTransform, nominalMovementDirection);
    }
}
