using Unity.Burst;
using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class GravitationObject : MonoBehaviour
{
   [field: Header("Grounding")]
   [field: SerializeField] public LayerMask GroundLayerMask { get; private set; }
   [field: SerializeField] public Vector3 GroundOffset { get; private set; }
   [field: SerializeField] public float SphereRadius { get; private set; }
   
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
      UpdateGrounded();
   }

   private void UpdateGravity()
   {
      CharacterController.ApplyGravitation(ref _currentFallSpeed, ref _isGrounded, GravityConstants.MaxFallSpeed,
         GravityConstants.GravityForce);
   }

   [BurstCompile]
   private void UpdateGrounded()
   {
      var spherePos = CashedTransform.position + GroundOffset;
      
      var hits = Physics.OverlapSphere(spherePos, SphereRadius,GroundLayerMask);

      if (hits.Length > 0)
      {
         _isGrounded = true;
         return;
      }
      _isGrounded = false;
   }
}
