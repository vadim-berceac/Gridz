using System;
using UnityEngine;

[Serializable]
public struct BoneTransformSlot
{
    [field: SerializeField] public CharacterBones.Type BonesType { get; private set; }
    [field: SerializeField] public Vector3 Position { get; private set; }
    [field: SerializeField] public Vector3 Rotation { get; private set; }
    [field: SerializeField] public Vector3 Scale { get; private set; }
    public Transform ParentTransform { get; private set; }

    public void BakeToParent(BonesCollector bonesCollector, Transform slotObjectTransform)
    {
        ParentTransform = bonesCollector.GetTransform(BonesType);
        Attach(slotObjectTransform);
    }

    private void Attach(Transform slotObjectTransform)
    {
        slotObjectTransform.SetParent(ParentTransform);
        slotObjectTransform.localPosition = Position;
        slotObjectTransform.localRotation = Quaternion.Euler(Rotation);
        slotObjectTransform.localScale = Scale;
    }
}
