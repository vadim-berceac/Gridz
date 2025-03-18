using UnityEngine;

[CreateAssetMenu(fileName = "OneShotClip", menuName = "Scriptable Objects/OneShotClip")]
public class OneShotClip : ScriptableObject
{
    [field: SerializeField] public AnimationClip Clip { get; private set; }
    [field: SerializeField] public float Speed { get; private set; } = 1f;
}
