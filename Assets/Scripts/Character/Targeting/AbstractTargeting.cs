using System.Collections.Generic;
using UnityEngine;

public abstract class AbstractTargeting : MonoBehaviour
{
    [field: SerializeField] protected SphereCollider TargetingCollider { get; private set; }
    public HashSet<Transform> Targets { get; private set; }
    
    protected Transform Parent;
    
    protected virtual void Awake()
    {
        Targets = new HashSet<Transform>();
        Parent = transform.parent;
    }
    
    protected virtual void AddTarget(Transform target)
    {
        Targets.Add(target);
        Debug.LogWarning($"Adding {target.name}");
    }

    protected virtual void RemoveTarget(Transform target)
    {
        if (!Targets.Contains(target))
        {
            return;
        }
        Targets.Remove(target);
        Debug.LogWarning($"Removing {target.name}");
    }

    public void SetTargetingColliderRadius(float radius)
    {
        TargetingCollider.radius = radius;
    }

    protected virtual void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.transform == Parent)
        {
            return;
        }
        if (other.gameObject.layer != TagsAndLayersConst.CharacterLayerIndex)
        {
            return;
        }
        AddTarget(other.transform);
    }

    protected virtual void OnTriggerExit(Collider other)
    {
        if (other.gameObject.layer != TagsAndLayersConst.CharacterLayerIndex)
        {
            return;
        }
        RemoveTarget(other.transform);
    }
}
