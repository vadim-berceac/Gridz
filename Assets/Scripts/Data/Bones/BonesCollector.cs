using System;
using System.Collections.Generic;
using UnityEngine;

public class BonesCollector
{
    private readonly List<BoneTransform> _bones  = new();
    
    public BonesCollector(Transform parent)
    {
        var boneCount = CharacterBones.BoneNames.Length;
       
        _bones.Capacity = Math.Max(_bones.Capacity, boneCount);

        for (var i = 0; i < boneCount; i++)
        {
            var boneName = CharacterBones.BoneNames[i];
            var boneTrans = parent.FindChildRecursive(boneName);

            if (boneTrans == null)
            {
                continue;
            }
            var boneType = (CharacterBones.Type)i;
            _bones.Add(new BoneTransform(boneTrans, boneType));
        }
    }

    public BoneTransform GetBoneTransform(CharacterBones.Type boneType)
    {
        var result = new BoneTransform();
        foreach (var bone in _bones)
        {
            if (bone.BonesType == boneType)
            {
                result = bone;
            }
        }
        return result;
    }
    
    public Transform GetTransform(CharacterBones.Type boneType)
    {
        foreach (var bone in _bones)
        {
            if (bone.BonesType == boneType)
            {
                return bone.Transform;
            }
        }
        return null;
    }
}
