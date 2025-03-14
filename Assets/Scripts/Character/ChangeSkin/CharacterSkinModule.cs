using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class CharacterSkinModule : MonoBehaviour
{
   public BonesCollector BonesCollector { get; private set; }
   [field: SerializeField] public CharacterSkinData CharacterSkinData { get; private set; }
   [field: SerializeField] public Transform CapsuleMarker { get; private set; }
   [field: SerializeField] public bool HideCapsule { get; private set; }
   
   private SkinnedMeshRenderer _blankRenderer;
   private List<SkinnedMeshRenderer> _temporaryBlankRenderers;

   private void Awake()
   {
      InitializeBonesCollector();
      InitializeRenderer();
      if (!IsValidSetup()) return;

      Hide();
      SetupBaseSkin();
      ApplyAdditionalSkins();
   }

   private void InitializeBonesCollector()
   {
      BonesCollector = new BonesCollector(transform);
   }

   private void InitializeRenderer()
   {
      _blankRenderer = transform.GetComponentsInChildren<SkinnedMeshRenderer>()
         .FirstOrDefault(renderer1 => renderer1.gameObject.name.Contains(TagsAndLayersConst.BlankSkinnedMeshName));
      _temporaryBlankRenderers = new List<SkinnedMeshRenderer>();
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
      gameObject.transform.localScale = new Vector3(CharacterSkinData.SizeMode, CharacterSkinData.SizeMode, CharacterSkinData.SizeMode);
      ApplySkin(CharacterSkinData.SkinData[0], _blankRenderer);
   }

   private void Hide()
   {
      if (!HideCapsule || CapsuleMarker == null)
      {
         return;
      }
      CapsuleMarker.gameObject.SetActive(false);
   }

   private void ApplyAdditionalSkins()
   {
      if (CharacterSkinData.SkinData.Count <= 1) return;

      for (var i = 1; i < CharacterSkinData.SkinData.Count; i++)
      {
         var newRendererObject = Instantiate(_blankRenderer, _blankRenderer.transform.parent);
         _temporaryBlankRenderers.Add(newRendererObject);
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
