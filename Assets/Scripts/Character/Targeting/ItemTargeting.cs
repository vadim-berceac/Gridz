using UnityEngine;

public class ItemTargeting : AbstractTargeting
{
    protected override void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.transform == Parent)
        {
            return;
        }
        if (other.gameObject.layer != TagsAndLayersConst.PickupObjectLayerIndex)
        {
            return;
        }
        AddTarget(other.transform);
    }

    protected override void OnTriggerExit(Collider other)
    {
        if (other.gameObject.layer != TagsAndLayersConst.PickupObjectLayerIndex)
        {
            return;
        }
        RemoveTarget(other.transform);
    }
}
