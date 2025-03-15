using UnityEngine;

public struct BoneTransform
{
    public CharacterBones.Type BonesType { get; private set; }
    public Transform Transform { get; private set; }
    public BoneTransform(Transform transform, CharacterBones.Type boneType)
    {
        Transform = transform;
        BonesType = boneType;
    }
}
