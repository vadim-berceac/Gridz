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
            if (bone == null)
            {
                continue;
            }
            newBones[i] = bone;
        }

        oldSkin.sharedMesh = newMesh;
        oldSkin.bones = newBones;
    }
    
    public static void ChangeCharacterSkin1(this SkinnedMeshRenderer oldSkin, SkinnedMeshRenderer newSkin)
    {
        //новые кости лежат вне иерархии - внизу модели, не использовать
        if (oldSkin == null || newSkin == null)
        {
            Debug.LogError("OldSkin или NewSkin равны null!");
            return;
        }

        var newMesh = Object.Instantiate(newSkin.sharedMesh);
        var newSkinBones = newSkin.bones;
        var newBones = new Transform[newSkinBones.Length];
        Transform oldRoot = oldSkin.transform.root;

        for (int i = 0; i < newSkinBones.Length; i++)
        {
            if (newSkinBones[i] == null)
            {
                Debug.LogWarning($"Кость с индексом {i} в newSkin.bones равна null!");
                continue;
            }

            string boneName = newSkinBones[i].name;
            Transform matchingBone = oldRoot.FindChildRecursive(boneName);

            if (matchingBone != null)
            {
                newBones[i] = matchingBone;
            }
            else
            {
                // Создаем копию кости в иерархии старого объекта
                GameObject newBone = new GameObject(boneName);
                newBone.transform.SetParent(oldRoot, false);
                newBone.transform.localPosition = newSkinBones[i].localPosition;
                newBone.transform.localRotation = newSkinBones[i].localRotation;
                newBone.transform.localScale = newSkinBones[i].localScale;
                newBones[i] = newBone.transform;
            
                Debug.LogWarning($"Кость '{boneName}' не найдена. Создана новая кость в иерархии '{oldRoot.name}'.");
            }
        }

        oldSkin.sharedMesh = newMesh;
        oldSkin.bones = newBones;
    }
}
