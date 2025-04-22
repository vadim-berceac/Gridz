using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class CharacterPersonalityModule : MonoBehaviour
{
   public BonesCollector BonesCollector { get; private set; }
   [field: SerializeField] public CharacterPersonalityData CharacterPersonalityData { get; private set; }
   [field: SerializeField] public CapsuleSettings CapsuleSettings { get; private set; }
   
   private SkinnedMeshRenderer _blankRenderer;
   private List<SkinnedMeshRenderer> _temporaryBlankRenderers;

   private void Awake()
   {
      InitializeBonesCollector();
      InitializeRenderer();
      if (!IsValidSetup()) return;

      Hide();
      SetName();
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
             CharacterPersonalityData.CharacterSkinData != null && 
             CharacterPersonalityData.CharacterSkinData.SkinData?.Count > 0 &&
             CharacterPersonalityData.CharacterName != null;
   }

   private void SetupBaseSkin()
   {
      gameObject.transform.localScale = new Vector3(CharacterPersonalityData.CharacterSkinData.SizeMode, 
         CharacterPersonalityData.CharacterSkinData.SizeMode, CharacterPersonalityData.CharacterSkinData.SizeMode);
      ApplySkin(CharacterPersonalityData.CharacterSkinData.SkinData[0], _blankRenderer);
   }

   private void Hide()
   {
      if (!CapsuleSettings.HideCapsule || CapsuleSettings.CapsuleMarker == null)
      {
         return;
      }
      CapsuleSettings.CapsuleMarker.gameObject.SetActive(false);
   }

   private void SetName()
   {
      gameObject.name = CharacterPersonalityData.CharacterName;
   }

   private void ApplyAdditionalSkins()
   {
      if (CharacterPersonalityData.CharacterSkinData.SkinData.Count <= 1) return;

      for (var i = 1; i < CharacterPersonalityData.CharacterSkinData.SkinData.Count; i++)
      {
         var newRendererObject = Instantiate(_blankRenderer, _blankRenderer.transform.parent);
         _temporaryBlankRenderers.Add(newRendererObject);
         ApplySkin(CharacterPersonalityData.CharacterSkinData.SkinData[i], newRendererObject);
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

[System.Serializable]
public struct CapsuleSettings
{
   [field: SerializeField] public Transform CapsuleMarker { get; private set; }
   [field: SerializeField] public bool HideCapsule { get; private set; }
}
