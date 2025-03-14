using System.Collections.Generic;
using UnityEngine;

public struct CharacterBones
{
    public enum Type
    {
        Hips,
        Head,
        Neck,
        Spine2,
        Spine1,
        Spine0,
        LeftUpperArm,
        RightUpperArm,
        LeftUpperLeg,
        RightUpperLeg,
        LeftLowerArm,
        RightLowerArm,
        LeftLowerLeg,
        RightLowerLeg,
        LeftFoot,
        RightFoot,
        LeftHand,
        RightHand,
    }

    public static readonly Dictionary<Type, string> BonesNames = new()
    {
        { Type.Hips, "Hips"},
        { Type.Head, "Head"},
        { Type.Neck , "Neck"},
        { Type.Spine2 , "Spine2"},
        { Type.Spine1 , "Spine1"},
        { Type.Spine0 , "Spine"},
        { Type.LeftUpperArm, "LeftArm"},
        { Type.RightUpperArm, "RightArm"},
        { Type.LeftUpperLeg , "LeftUpLeg"},
        { Type.RightUpperLeg , "RightUpLeg"},
        { Type.LeftLowerArm , "LeftForeArm"},
        { Type.RightLowerArm , "RightForeArm"},
        { Type.LeftLowerLeg , "LeftLeg"},
        { Type.RightLowerLeg , "RightLeg"},
        { Type.LeftFoot, "LeftFoot"},
        { Type.RightFoot, "RightFoot"},
        { Type.LeftHand, "LeftHand"},
        { Type.RightHand , "RightHand"},
    };
}

public class BoneTransform
{
    public CharacterBones.Type BonesType { get; private set; }
    public Transform Transform { get; private set; }
    public BoneTransform(Transform transform, CharacterBones.Type boneType)
    {
        Transform = transform;
        BonesType = boneType;
    }
}
