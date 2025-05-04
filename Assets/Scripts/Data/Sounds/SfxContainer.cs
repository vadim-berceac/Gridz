using System.Linq;
using UnityEngine;

[CreateAssetMenu(fileName = "SFXContainer", menuName = "Scriptable Objects/SFXContainer")]
public class SfxContainer : ScriptableObject
{
    [field: SerializeField] public SfxSet[] SfxSets { get; set; }

    public SfxSet GetSfxSet(string id)
    {
        return SfxSets.FirstOrDefault(x => x.SetName == id);
    }
}

[System.Serializable]
public class SfxSet
{
    [field: SerializeField] public string SetName { get; private set; }
    [field: SerializeField] public AudioClip[] Clip { get; private set; }

    public AudioClip GetRandomClip()
    {
        return Clip[Random.Range(0, Clip.Length)];
    }
}