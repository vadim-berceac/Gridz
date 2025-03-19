using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "OneShotClipSet", menuName = "Scriptable Objects/OneShotClipSet")]
public class OneShotClipSet : ScriptableObject
{
    [field: SerializeField] public AnimationTypes.Type Type { get; private set; }
    [SerializeField] private List<OneShotClip> clips;

    public OneShotClip GetOneShotClip()
    {
        if (clips.Count < 1)
        {
            return null;
        }
        return clips[Random.Range(0, clips.Count)];
    }

    public OneShotClip GetOneShotClip(int index)
    {
        if (clips.Count <= index || clips[index] == null || clips.Count < 1)
        {
            return null;
        }
        return clips[index];
    }
}
