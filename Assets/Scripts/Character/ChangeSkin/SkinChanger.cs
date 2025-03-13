using UnityEngine;

public class SkinChanger : MonoBehaviour
{
   [field: SerializeField] public CharacterSkinData CharacterSkinData { get; private set; }
   
   private SkinnedMeshRenderer _blankRenderer;

   private void Awake()
   {
      InitializeRenderer();
      if (!IsValidSetup()) return;

      SetupBaseSkin();
      ApplyAdditionalSkins();
   }

   private void InitializeRenderer()
   {
      _blankRenderer = transform.GetComponentInChildren<SkinnedMeshRenderer>();
   }

   private bool IsValidSetup()
   {
      return _blankRenderer != null && 
             CharacterSkinData != null && 
             CharacterSkinData.SkinData?.Count > 0;
   }

   private void SetupBaseSkin()
   {
      gameObject.name = CharacterSkinData.SkinName;
      ApplySkin(CharacterSkinData.SkinData[0], _blankRenderer);
   }

   private void ApplyAdditionalSkins()
   {
      if (CharacterSkinData.SkinData.Count <= 1) return;

      for (var i = 1; i < CharacterSkinData.SkinData.Count; i++)
      {
         var newRendererObject = Instantiate(_blankRenderer, _blankRenderer.transform.parent);
         ApplySkin(CharacterSkinData.SkinData[i], newRendererObject);
      }
   }

   private static void ApplySkin(SkinData skinData, SkinnedMeshRenderer targetRenderer)
   {
      if (skinData == null || targetRenderer == null) return;

      if (skinData.SkinnedMeshRenderer != null)
      {
         targetRenderer.ChangeCharacterSkin(skinData.SkinnedMeshRenderer);
      }

      if (skinData.SkinMaterial != null)
      {
         targetRenderer.sharedMaterial = skinData.SkinMaterial;
      }
   }
}
