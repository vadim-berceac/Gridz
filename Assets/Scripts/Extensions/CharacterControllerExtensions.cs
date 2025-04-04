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
        if (isGrounded)
        {
            if (currentFallSpeed < 0f)
            {
                currentFallSpeed = -2f; 
            }
        }
        else
        {
            var gravityDelta = gravityForce * Time.fixedDeltaTime;
            currentFallSpeed = Mathf.Max(currentFallSpeed - gravityDelta, -maxFallSpeed);
        }

        var moveVector = currentFallSpeed * Time.fixedDeltaTime * Vector3.up;
        var previousPosition = controller.transform.position; 
        controller.Move(moveVector); 
        var newPosition = controller.transform.position;
        
        if (!isGrounded && moveVector.y < 0f && Mathf.Approximately(newPosition.y, previousPosition.y))
        {
            currentFallSpeed = -2f;
        }
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
