using System.Linq;
using Unity.Burst;
using UnityEngine;

public class CharacterTargeting : AbstractTargeting
{
    public Vector3 TargetDirection { get; private set; }
    
    private Transform _cashedTransform;

    protected override void Awake()
    {
        base.Awake();
        TargetDirection = Vector3.zero;
        _cashedTransform = transform;
    }

    [BurstCompile]
    private void UpdateTarget(float targetMaxDistance)
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

        TargetDirection = directionToTarget; 
    }
    
    [BurstCompile]
    public void Target(bool isDead, bool isTargetLock, float targetMaxDistance)
    {
        if (isDead)
        {
            return;
        }
        if (!isTargetLock)
        {
            return;
        }

        if (Targets.Count < 1)
        {
            return;
        }
        UpdateTarget(targetMaxDistance);
    }
}
