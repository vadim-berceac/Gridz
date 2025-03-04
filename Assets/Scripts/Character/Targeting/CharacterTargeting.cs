using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class CharacterTargeting : MonoBehaviour
{
    [SerializeField] private float targetMaxDistance = 10f;
    public HashSet<Transform> Targets { get; private set; }
    public Vector3 TargetDirection { get; private set; }
    
    private Transform _parent;
    private Transform _cashedTransform;

    private void Awake()
    {
        Targets = new HashSet<Transform>();
        TargetDirection = Vector3.zero;
        _parent = transform.parent;
        _cashedTransform = transform;
    }

    public void UpdateTarget()
    {
        Targets.RemoveWhere(t => t == null); 
        if (Targets.Count == 0)
        {
            TargetDirection = Vector3.zero; 
            return;
        }

        var sqrTargetMaxDistance = targetMaxDistance * targetMaxDistance;

        var nearestTarget = Targets
            .Select(target => new 
            { 
                Target = target, 
                VectorToTarget = target.position - _cashedTransform.position 
            })
            .Where(t => t.VectorToTarget.sqrMagnitude <= sqrTargetMaxDistance)
            .OrderBy(t => t.VectorToTarget.sqrMagnitude) 
            .FirstOrDefault();

        var directionToTarget = nearestTarget != null 
            ? nearestTarget.VectorToTarget.normalized 
            : Vector3.zero;

        TargetDirection = directionToTarget; // Сохраняем глобальное направление
        Debug.LogWarning("Target direction: " + directionToTarget);
    }

    private void AddTarget(Transform target)
    {
        Targets.Add(target);
        Debug.LogWarning($"Adding {target.name}");
    }

    private void RemoveTarget(Transform target)
    {
        if (!Targets.Contains(target))
        {
            return;
        }
        Targets.Remove(target);
        Debug.LogWarning($"Removing {target.name}");
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.transform == _parent)
        {
            return;
        }
        if (other.gameObject.layer != TagsAndLayersConst.CharacterLayerIndex)
        {
            return;
        }
        AddTarget(other.transform);
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.layer != TagsAndLayersConst.CharacterLayerIndex)
        {
            return;
        }
        RemoveTarget(other.transform);
    }
}
