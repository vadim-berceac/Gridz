using UnityEngine;

[System.Serializable]
public struct TargetingSettings
{
    [field: SerializeField] public CharacterTargeting EnemyTargeting { get; private set; }
    [field: SerializeField] public ItemTargeting ItemTargeting { get; private set; }
}