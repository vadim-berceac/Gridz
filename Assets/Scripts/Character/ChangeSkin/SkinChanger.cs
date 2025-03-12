using System.Collections.Generic;
using UnityEngine;

public class SkinChanger : MonoBehaviour
{
   [field: SerializeField] public SkinnedMeshRenderer SkinnedMeshRenderer1 { get; private set; }
   [field: SerializeField] public SkinnedMeshRenderer SkinnedMeshRenderer2 { get; private set; }

   void Awake()
   {
      // Клонируем меш из SkinnedMeshRenderer2
      Mesh newMesh = Instantiate(SkinnedMeshRenderer2.sharedMesh);

      // Получаем массивы костей
      Transform[] bones1 = SkinnedMeshRenderer1.bones;
      Transform[] bones2 = SkinnedMeshRenderer2.bones;

      // Словарь для поиска костей по именам
      Dictionary<string, Transform> boneDict1 = new Dictionary<string, Transform>();
      foreach (var bone in bones1)
      {
         boneDict1[bone.name] = bone;
      }

      // Новый массив костей, сопоставленный по именам
      Transform[] newBones = new Transform[bones2.Length];
      for (int i = 0; i < bones2.Length; i++)
      {
         string boneName = bones2[i].name;
         if (boneDict1.TryGetValue(boneName, out Transform bone))
         {
            newBones[i] = bone;
         }
         else
         {
            Debug.LogError($"Кость {boneName} не найдена!");
            return;
         }
      }

      // Обновляем bindposes для нового меша
      Matrix4x4[] newBindposes = new Matrix4x4[newBones.Length];
      for (int i = 0; i < newBones.Length; i++)
      {
         newBindposes[i] = newBones[i].worldToLocalMatrix * SkinnedMeshRenderer1.transform.localToWorldMatrix;
      }

      newMesh.bindposes = newBindposes;

      // Применяем изменения
      SkinnedMeshRenderer1.sharedMesh = newMesh;
      SkinnedMeshRenderer1.bones = newBones;
   }
}
