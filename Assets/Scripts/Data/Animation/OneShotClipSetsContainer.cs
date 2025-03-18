using System.Linq;
using UnityEngine;

[CreateAssetMenu(fileName = "OneShotClipSetsContainer", menuName = "Scriptable Objects/OneShotClipSetsContainer")]
public class OneShotClipSetsContainer : ScriptableObject
{
    [SerializeField] private OneShotClipSet[] oneShotClipSets;

    public OneShotClip GetOneShotClip(AnimationTypes.Type animationType)
    {
        return oneShotClipSets
            .FirstOrDefault(set => set.Type == animationType)
            ?.GetOneShotClip();
    }

    public OneShotClip GetOneShotClip(AnimationTypes.Type animationType, int indexInSet)
    {
        return oneShotClipSets
            .FirstOrDefault(set => set.Type == animationType)
            ?.GetOneShotClip(indexInSet);
    }
}
