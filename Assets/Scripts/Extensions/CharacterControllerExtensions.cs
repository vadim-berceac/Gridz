using System;
using DG.Tweening;
using Unity.Burst;
using UnityEngine;

public static class CharacterControllerExtensions
{
    [BurstCompile]
    public static void Jump(this CharacterController controller, float currentSpeed, float jumpHeight, float duration, Action onComplete = null)
    {
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
