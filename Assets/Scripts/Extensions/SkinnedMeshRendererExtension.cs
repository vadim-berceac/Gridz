using UnityEngine;

public static class SkinnedMeshRendererExtension
{
    public static void ChangeCharacterSkin(this SkinnedMeshRenderer oldSkin, SkinnedMeshRenderer newSkin)
    {
        var newMesh = Object.Instantiate(newSkin.sharedMesh);
       
        var bones2 = newSkin.bones;

        var newBones = new Transform[bones2.Length];
        for (var i = 0; i < bones2.Length; i++)
        {
            var boneName = bones2[i].name;
           
            var bone = oldSkin.transform.root.FindChildRecursive(boneName); 
            if (bone != null)
            {
                newBones[i] = bone;
            }
            else
            {
                Debug.LogError($"Кость {boneName} не найдена в иерархии {oldSkin.transform.root.name}!");
                return;
            }
        }

        oldSkin.sharedMesh = newMesh;
        oldSkin.bones = newBones;
    }
}
