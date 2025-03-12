using UnityEngine;

[CreateAssetMenu(fileName = "CharacterSkinData", menuName = "Scriptable Objects/CharacterSkinData")]
public class CharacterSkinData : ScriptableObject
{
    [field: SerializeField] public string SkinName { get; private set; }
    [field: SerializeField] public SkinnedMeshRenderer SkinnedMeshRenderer{ get; private set; }
}
