using UnityEngine;

public class RandomState : StateMachineBehaviour
{
    [field: SerializeField] private RandomStateSettings RandomStateSettings { get; set; }
    
    private float _randomNormTime;

    public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        _randomNormTime = Random.Range(RandomStateSettings.MinNormTime, RandomStateSettings.MaxNormTime);
    }

    public override void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if (animator.IsInTransition(0) && animator.GetCurrentAnimatorStateInfo(0).fullPathHash == stateInfo.fullPathHash)
        {
            animator.SetInteger(AnimationParams.RandomIdle, -1);
        }
        
        if (stateInfo.normalizedTime > _randomNormTime && !animator.IsInTransition(0))
        {
            animator.SetInteger(AnimationParams.RandomIdle, Random.Range(0, RandomStateSettings.NumberOfStates));
        }
    }
}

[System.Serializable]
public struct RandomStateSettings
{
    [field: SerializeField] public int NumberOfStates { get; private set; }
    [field: SerializeField] public float MinNormTime { get; private set; }
    [field: SerializeField] public float MaxNormTime { get; private set; }
}
