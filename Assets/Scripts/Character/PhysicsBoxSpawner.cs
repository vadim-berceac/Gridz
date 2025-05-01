using UnityEngine;

public class PhysicsBoxSpawner : MonoBehaviour
{
    [field: SerializeField] public PhysicsBoxSettings PhysicsBoxSettings { get; private set; }
    private PhysicsBox _box;

    private void Awake()
    {
        _box = Instantiate(PhysicsBoxSettings.Prefab, transform.position, Quaternion.identity).GetComponent<PhysicsBox>();
        _box.Attach(PhysicsBoxSettings.Rb, PhysicsBoxSettings.PlayerModel, PhysicsBoxSettings.ScaleDown,
            PhysicsBoxSettings.ScaleUp, PhysicsBoxSettings.ScaleCoefficient, PhysicsBoxSettings.RotationCoefficient);
    }
}

[System.Serializable]
public struct PhysicsBoxSettings
{
    [field: SerializeField] public GameObject Prefab {get; private set;}
    [field: SerializeField] public Rigidbody Rb {get; private set;}
    [field: SerializeField] public Transform PlayerModel {get; private set;}
    
    [field: Header("Character Scale")]
    [field: SerializeField] public Vector3 ScaleDown {get; private set;}
    [field: SerializeField] public Vector3 ScaleUp {get; private set;}
    [field: SerializeField] public float ScaleCoefficient {get; private set;}
    [field: SerializeField] public float RotationCoefficient {get; private set;}

}
