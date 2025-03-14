using System.Collections.Generic;
using UnityEngine;

public class BonesCollector
{
    private readonly List<BoneTransform> _bones  = new();
    
    public BonesCollector(Transform parent)
    {
        foreach (var bone in CharacterBones.BonesNames)
        {
            var boneTrans = parent.FindChildRecursive(bone.Value);

            if (boneTrans == null)
            {
                continue;
            }
            Debug.Log(boneTrans.name);
            _bones.Add(new BoneTransform(boneTrans, bone.Key));
        }
    }

    public BoneTransform GetBoneTransform(CharacterBones.Type boneType)
    {
        foreach (var bone in _bones)
        {
            if (bone.BonesType == boneType)
            {
                return bone;
            }
        }
        return null;
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
