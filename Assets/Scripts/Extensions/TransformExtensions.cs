using System;
using System.Collections.Generic;
using DG.Tweening;
using Unity.Burst;
using UnityEngine;

public static class TransformExtensions
{
    [BurstCompile]
    public static Transform FindChildRecursive(this Transform transform, string name)
    {
        foreach (Transform child in transform)
        {
            if (child.name.Contains(name))
            {
                return child;
            }

            var result = FindChildRecursive(child, name);
            if (result != null)
            {
                return result;
            }
        }
        return null;
    }
    
    [BurstCompile]
    public static List<GameObject> FindObjectsWithTag(this Transform parent, string tag)
    {
        var taggedGameObjects = new List<GameObject>();

        for (var i = 0; i < parent.childCount; i++)
        {
            var child = parent.GetChild(i);
            if (child.CompareTag(tag))
            {
                taggedGameObjects.Add(child.gameObject);
            }
            if (child.childCount > 0)
            {
                taggedGameObjects.AddRange(FindObjectsWithTag(child, tag));
            }
        }
        return taggedGameObjects;
    }
    
    [BurstCompile]
    public static void RotateCombined(this Transform transform, MovementTypes.MovementType type, Vector3 inputDirection, 
        float rotationSpeed, float duration, bool rotateByCamera, float cameraYaw)
    {
        if (type == MovementTypes.MovementType.None)
        {
            return;
        }

        var targetRotation = transform.rotation; 
       
        if (rotateByCamera)
        {
            var currentRotation = transform.eulerAngles;
            var cameraRotation = new Vector3(currentRotation.x, cameraYaw, currentRotation.z);
            targetRotation = Quaternion.Euler(cameraRotation); 
        }

        if (inputDirection != Vector3.zero)
        {
            var desiredMoveDirection = transform.forward * inputDirection.y + transform.right * inputDirection.x;
            var targetDirection = inputDirection.y >= 0 ? desiredMoveDirection : -desiredMoveDirection;
            var inputRotation = Quaternion.LookRotation(targetDirection, Vector3.up);

            targetRotation = rotateByCamera ? targetRotation * Quaternion.Inverse(transform.rotation) * inputRotation : inputRotation;
        }

        transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation.normalized, rotationSpeed * duration);
    }

    public static void RotateTo(this Transform transform, MovementTypes.MovementType type, Vector3 direction, 
        float rotationSpeed, float duration)
    {
        if (type == MovementTypes.MovementType.None)
        {
            return;
        }

        if (direction == Vector3.zero)
        {
           return;
        }
        
        var targetDirection = direction.normalized; 
        var targetRotation = Quaternion.LookRotation(targetDirection, Vector3.up);

        transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * duration);
    }

    [BurstCompile]
    public static void Move(this Transform tr, CharacterController controller, MovementTypes.MovementType movementType,
        Vector3 inputDirection, float currentSpeed, float duration, bool isJumping)
    {
        if (currentSpeed == 0 || !controller.enabled || isJumping)
        {
            return;
        }
        
        if (movementType == MovementTypes.MovementType.DotWeen)
        {
            MoveDotWeen(tr, controller, inputDirection, currentSpeed, duration);
        }
    }
    
    [BurstCompile]
    private static void MoveDotWeen(this Transform tr, CharacterController controller, Vector3 inputDirection,
        float currentSpeed, float duration)
    {
        tr.DOKill();

        var progress = 0f;
        var startPosition = tr.position;
        
        var forwardMovement = tr.forward * Mathf.Max(inputDirection.z, 0f); 
        var backwardMovement = -tr.forward * Mathf.Max(-inputDirection.z, 0f); 
        var rightMovement = tr.right * Mathf.Max(inputDirection.x, 0f); 
        var leftMovement = -tr.right * Mathf.Max(-inputDirection.x, 0f); 

        var movementDirection = (forwardMovement + backwardMovement + rightMovement + leftMovement).normalized *
                                currentSpeed;

        var targetPosition = startPosition + movementDirection * duration;

        DOTween.To(() => progress, x => progress = x, 1f, duration)
            .SetEase(Ease.Linear)
            .SetUpdate(UpdateType.Normal)
            .OnUpdate(() =>
            {
                var currentTarget = Vector3.Lerp(startPosition, targetPosition, progress);
                var moveDelta = currentTarget - tr.position;
                controller.Move(moveDelta);
            })
            .OnComplete(() =>
            {
                var finalDelta = targetPosition - controller.transform.position;
                controller.Move(finalDelta);
            });
    }
}
