using System;
using DG.Tweening;
using Unity.Burst;
using UnityEngine;

public static class CharacterControllerExtensions
{
    [BurstCompile]
    public static void ApplyGravitation(this CharacterController controller, ref float currentFallSpeed,
        bool isGrounded, float maxFallSpeed, float gravityForce)
    {
        if (isGrounded && currentFallSpeed < 0)
        {
            currentFallSpeed = -2f;
        }
        else
        {
            currentFallSpeed -= gravityForce * Time.deltaTime;
            currentFallSpeed = Mathf.Max(currentFallSpeed, -maxFallSpeed);
        }
       
        var moveVector = currentFallSpeed * Time.deltaTime * Vector3.up;
      
        controller.Move(moveVector);
    }
    
    [BurstCompile]
    public static void Jump(this CharacterController controller, MovementTypes.MovementType movementType, 
        float currentSpeed, float jumpHeight, float duration, Action onComplete = null)
    {
        if (movementType == MovementTypes.MovementType.None)
        {
            return;
        }
    
        var tr = controller.transform;
        var initialPosition = tr.position;
        var jumpDirection = tr.forward.normalized;
    
        var jumpSequence = DOTween.Sequence();
    
        var horizontalTarget = jumpDirection * currentSpeed;
        var targetPosition = initialPosition + horizontalTarget;

        jumpSequence.Append(
            DOTween.To(
                    () => 0f,
                    t =>
                    {
                        var normalizedTime = t / duration;
                        var horizontalPos = Vector3.Lerp(initialPosition, targetPosition, normalizedTime);
                        var verticalOffset = jumpHeight * 4f * normalizedTime * (1f - normalizedTime);
                    
                        var newPosition = new Vector3(
                            horizontalPos.x,
                            initialPosition.y + verticalOffset,
                            horizontalPos.z
                        );
                    
                        var moveVector = newPosition - tr.position;
                        controller.Move(moveVector);
                    }, 
                    duration,
                    duration
                )
                .SetEase(Ease.Linear)
        );
    
        jumpSequence.OnComplete(() => onComplete?.Invoke());
        jumpSequence.Play();
    }
}
