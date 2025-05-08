using UnityEngine;
using Zenject;

public class CharacterFootSteps : MonoBehaviour
{
   [field: SerializeField] private CharacterFootStepsSettings FootStepsSettings { get; set; }

   private const float Tolerance = 0.05f;
   private float _lastFootstepTime;
   private const float FootstepCooldown = 0.1f;
   private SfxSet _footStepsSfxSet;

   [Inject]
   private void Construct(SfxContainer sfxContainer)
   {
       _footStepsSfxSet = sfxContainer.GetSfxSet(FootStepsSettings.FootStepsSetName);
   }

   private void Update()
   {
       CheckCurveValue();
   }

   private void CheckCurveValue()
   {
       if (Time.time < _lastFootstepTime + FootstepCooldown || !FootStepsSettings.Character.IsGrounded)
       {
           return;
       }
       
       var curveValue = FootStepsSettings.Character.FootStepsCurveValue;
    
       if (Mathf.Abs(curveValue - FootStepsSettings.LeftFootStepValue) <= Tolerance)
       {
           _footStepsSfxSet.PlayRandomAtPoint(FootStepsSettings.LeftFootTransform.position);
           _lastFootstepTime = Time.time;
           return;
       }
    
       if (Mathf.Abs(curveValue - FootStepsSettings.RightFootStepValue) > Tolerance)
       {
           return;
       }
       _footStepsSfxSet.PlayRandomAtPoint(FootStepsSettings.RightFootTransform.position);
       _lastFootstepTime = Time.time;
   }
}

[System.Serializable]
public struct CharacterFootStepsSettings
{
    [field: SerializeField] public string FootStepsSetName { get; set; }
    [field: SerializeField] public CharacterAnimationParamsLayer Character { get; private set; }
    [field: SerializeField] public int LeftFootStepValue { get; private set; } 
    [field: SerializeField] public int RightFootStepValue { get; private set; } 
    [field: Header("Foot transforms")]
    [field: SerializeField] public Transform LeftFootTransform { get; private set; }
    [field: SerializeField] public Transform RightFootTransform { get; private set; }
}