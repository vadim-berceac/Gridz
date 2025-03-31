using System.Collections;
using Unity.Burst;
using UnityEngine;

public static class AnimatorExtensions
{
    [BurstCompile]
    public static bool IsClipPlaying(this Animator animator, int paramHash)
    {
        return animator.GetFloat(paramHash) > 0;
    }
    
    [BurstCompile]
    public static IEnumerator WaitForClipComplete(this Animator animator, int paramHash)
    {
        while (IsClipPlaying(animator, paramHash))
        {
            yield return null;
        }
    }

    [BurstCompile]
    public static void SetNewClipToState(this Animator animator, AnimationClip clip, string stateName)
    {
        if (clip == null || string.IsNullOrEmpty(stateName))
        {
            return;
        }
        var overrideController = new AnimatorOverrideController(animator.runtimeAnimatorController); 
        overrideController[stateName] = clip;
        animator.runtimeAnimatorController = overrideController; 
    }
}
