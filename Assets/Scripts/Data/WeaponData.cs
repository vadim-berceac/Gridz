using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "WeaponData", menuName = "Scriptable Objects/WeaponData")]
public class WeaponData : ScriptableObject
{
    [field: SerializeField] public Transform View  { get; private set; }
    [field: SerializeField] public List<BoneTransformSlot> BoneTransformSlots { get; private set; }
    [field: SerializeField] public AnimationTypes.Type AnimationType { get; private set; } = AnimationTypes.Type.None;

}
