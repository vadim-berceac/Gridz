using System.Linq;
using UnityEngine;

public class RandomState : StateMachineBehaviour
{
    [field: SerializeField] private RandomStateSettings RandomStateSettings { get; set; }
    
    private float _randomNormTime;
    private AnimationState _currentAnimationState;
   
    public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        var currentAnimationType = (AnimationTypes.Type)animator.GetFloat(AnimationParams.AnimationType);

        _currentAnimationState = null;
        
        _currentAnimationState = RandomStateSettings.AnimationStates.FirstOrDefault(x => x.Type == currentAnimationType);
        
        _randomNormTime = Random.Range(RandomStateSettings.MinNormTime, RandomStateSettings.MaxNormTime);
    }

    public override void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if (_currentAnimationState == null)
        {
            return;
        }
        if (animator.IsInTransition(0) && animator.GetCurrentAnimatorStateInfo(0).fullPathHash == stateInfo.fullPathHash)
        {
            animator.SetInteger(AnimationParams.RandomIdle, -1);
        }
        
        if (stateInfo.normalizedTime > _randomNormTime && !animator.IsInTransition(0))
        {
            animator.SetInteger(AnimationParams.RandomIdle, Random.Range(0, _currentAnimationState.StatesCount));
        }
    }
}

[System.Serializable]
public struct RandomStateSettings
{
    [field: SerializeField] public AnimationState[] AnimationStates { get; private set; }
    [field: SerializeField] public float MinNormTime { get; private set; }
    [field: SerializeField] public float MaxNormTime { get; private set; }
}

[System.Serializable]
public class AnimationState
{
    [field: SerializeField] public string Name { get; private set; }
    [field: SerializeField] public AnimationTypes.Type Type { get; private set; }
    [field: SerializeField] public int StatesCount { get; private set; }
}
