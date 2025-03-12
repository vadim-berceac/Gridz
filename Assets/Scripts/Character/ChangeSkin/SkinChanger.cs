using UnityEngine;

public class SkinChanger : MonoBehaviour
{
   private SkinnedMeshRenderer _blankRenderer;
   [field: SerializeField] public CharacterSkinData CharacterSkinData { get; private set; }

   private void Awake()
   {
      _blankRenderer = transform.GetComponentInChildren<SkinnedMeshRenderer>();

      if (_blankRenderer == null || CharacterSkinData == null)
      {
         return;
      }
      
      gameObject.name = CharacterSkinData.SkinName;
      
      _blankRenderer.ChangeCharacterSkin(CharacterSkinData.SkinnedMeshRenderer);
   }
}
