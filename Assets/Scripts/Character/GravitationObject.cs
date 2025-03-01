using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class GravitationObject : MonoBehaviour
{
   protected CharacterController CharacterController;
   private float _currentFallSpeed = 0f;
   private bool _isGrounded;

   public bool IsGrounded => _isGrounded;
   
   public Transform CashedTransform { get; private set; }
   private void Awake()
   {
      Initialize();  
   }
   
   protected virtual void Initialize()
   {
      CashedTransform = transform;
      CharacterController = GetComponent<CharacterController>();
   }
   
   private void FixedUpdate()
   {
      UpdateGravity();
   }

   private void UpdateGravity()
   {
      CharacterController.ApplyGravitation(ref _currentFallSpeed, ref _isGrounded, GravityConstants.MaxFallSpeed,
         GravityConstants.GravityForce);
   }
}
