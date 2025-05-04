using UnityEngine;
using Zenject;

public class CharacterFootSteps : MonoBehaviour
{
   [field: SerializeField] private CharacterFootStepsSettings FootStepsSettings { get; set; }

   private const string FootStepsSet = "FootSteps0";
   private const float Tolerance = 0.05f;
   private float _lastFootstepTime;
   private const float FootstepCooldown = 0.1f;
   private SfxSet _footStepsSfxSet;

   [Inject]
   private void Construct(SfxContainer sfxContainer)
   {
       _footStepsSfxSet = sfxContainer.GetSfxSet(FootStepsSet);
   }

   private void Update()
   {
       CheckCurveValue();
   }

   private void CheckCurveValue()
   {
       if (Time.time < _lastFootstepTime + FootstepCooldown)
       {
           return;
       }
       
       var curveValue = FootStepsSettings.Character.FootStepsCurveValue;
    
       if (Mathf.Abs(curveValue - FootStepsSettings.LeftFootStepValue) <= Tolerance)
       {
           AudioSource.PlayClipAtPoint(_footStepsSfxSet.GetRandomClip(), FootStepsSettings.LeftFootTransform.position);
           _lastFootstepTime = Time.time;
           return;
       }
    
       if (Mathf.Abs(curveValue - FootStepsSettings.RightFootStepValue) > Tolerance)
       {
           return;
       }
       
       AudioSource.PlayClipAtPoint(_footStepsSfxSet.GetRandomClip(), FootStepsSettings.RightFootTransform.position);
       _lastFootstepTime = Time.time;
   }
}

[System.Serializable]
public struct CharacterFootStepsSettings
{
    [field: SerializeField] public CharacterAnimationParamsLayer Character { get; private set; }
    [field: SerializeField] public int LeftFootStepValue { get; private set; } 
    [field: SerializeField] public int RightFootStepValue { get; private set; } 
    [field: Header("Foot transforms")]
    [field: SerializeField] public Transform LeftFootTransform { get; private set; }
    [field: SerializeField] public Transform RightFootTransform { get; private set; }
}