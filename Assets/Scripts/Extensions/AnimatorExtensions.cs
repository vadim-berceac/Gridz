using System.Collections;
using System.Linq;
using Unity.Burst;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor.Animations;
#endif

public static class AnimatorExtensions
{
    [BurstCompile]
    public static bool IsClipPlaying(this Animator animator,  int paramHash)
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
    public static AnimatorController GetAnimatorController(this Animator animator)
    {
        return animator.runtimeAnimatorController as AnimatorController;
    }

    [BurstCompile]
    public static AnimatorState GetAnimatorState(this Animator animator, int layer, string stateName)
    {
        return GetAnimatorController(animator).layers[layer].stateMachine.states.FirstOrDefault
            (s => s.state.name == stateName).state;
    }

    [BurstCompile]
    public static void SetNewClipToState(this Animator animator, AnimationClip clip, AnimatorState state)
    {
        if (clip == null)
        {
            return;
        }
        var overrideController = new AnimatorOverrideController(animator.runtimeAnimatorController); 

        overrideController[state.motion.name] = clip;
        
        animator.runtimeAnimatorController = overrideController; 
    }
}
