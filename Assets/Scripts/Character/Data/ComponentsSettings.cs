using UnityEngine;

[System.Serializable]
public struct ComponentsSettings
{
    [field: SerializeField] public Animator AnimatorLocal { get; private set; }
    [field: SerializeField] public Collider CapsuleCollider { get; private set; }
    [field: SerializeField] public Rigidbody Rigidbody { get; private set; }
    [field: SerializeField] public HealthModule Health { get; private set; }
    [field: SerializeField] public EquipmentModule Equipment { get; private set; }
    [field: SerializeField] public CharacterPersonalityModule CharacterPersonality { get; private set; }
}
