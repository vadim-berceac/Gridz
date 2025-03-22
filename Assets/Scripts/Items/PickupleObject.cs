using UnityEngine;

public class PickupObject : MonoBehaviour
{
    [field: SerializeField] public ItemData ItemData { get; private set; }

    private void OnEnable()
    {
        gameObject.layer = TagsAndLayersConst.PickupObjectLayerIndex;
    }
}
