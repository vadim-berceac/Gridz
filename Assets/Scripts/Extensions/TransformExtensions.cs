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
    public static void Rotate(this Transform transform, bool isDead, bool isTargetLock, CharacterTargeting enemyTargeting, bool rotateByCamera, CameraSystem cameraSystem, 
        Vector2 nominalMovementDirection, float rotationSpeed)
    {
        if (isDead)
        {
            return;
        }
        if (enemyTargeting.TargetDirection == Vector3.zero || !isTargetLock)
        {
            transform.RotateCombined(nominalMovementDirection, rotationSpeed, 
                Time.deltaTime, rotateByCamera, cameraSystem.GetCameraYaw());
            return;
        }
        transform.RotateTo( enemyTargeting.TargetDirection, rotationSpeed, Time.deltaTime);
    }
    
    [BurstCompile]
    private static void RotateCombined(this Transform transform, Vector3 inputDirection, float rotationSpeed, float duration, bool rotateByCamera, float cameraYaw)
    {

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

    [BurstCompile]
    private static void RotateTo(this Transform transform, Vector3 direction, float rotationSpeed, float duration)
    {
        if (direction == Vector3.zero)
        {
           return;
        }
        
        var targetDirection = direction.normalized; 
        var targetRotation = Quaternion.LookRotation(targetDirection, Vector3.up);

        transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * duration);
    }
}
