using Unity.Burst;
using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class GravitationLayer : MonoBehaviour
{
    [field: SerializeField] public GravitationSettings GravitationSettings { get; set; }

    private float _maxHeightReached;
    private bool _wasGroundedLastFrame;
    public event System.Action<float> OnFallDamage; 

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
        _maxHeightReached = CashedTransform.position.y;
    }

    protected virtual void FixedUpdate()
    {
        UpdateGrounded();
        UpdateGravity();
        UpdateFallDetection();
    }

    private void UpdateGravity()
    {
        CharacterController.ApplyGravitation(ref _currentFallSpeed, _isGrounded, GravityConstants.MaxFallSpeed,
            GravityConstants.GravityForce);
    }
    [BurstCompile]

    private void UpdateFallDetection()
    {
        var currentHeight = CashedTransform.position.y;
        
        if (!_isGrounded)
        {
            _maxHeightReached = Mathf.Max(_maxHeightReached, currentHeight);
        }
        
        if (_isGrounded && !_wasGroundedLastFrame)
        {
            var fallDistance = _maxHeightReached - currentHeight;
            if (fallDistance > GravitationSettings.FallDamageThreshold)
            {
                OnFallDamage?.Invoke(fallDistance);
            }

            _maxHeightReached = currentHeight; 
        }

        _wasGroundedLastFrame = _isGrounded;
    }
    
    [BurstCompile]
    private void UpdateGrounded()
    {
        var spherePos = CashedTransform.position + GravitationSettings.GroundOffset;
        var hitColliders = new Collider[32]; 
        var hitsCount = Physics.OverlapSphereNonAlloc(spherePos, GravitationSettings.SphereRadius, hitColliders, 
            GravitationSettings.GroundLayerMask);

        _isGrounded = hitsCount > 0;
    }
}

[System.Serializable]
public struct GravitationSettings
{
    [field: SerializeField] public LayerMask GroundLayerMask { get; private set; }

    [field: SerializeField] public Vector3 GroundOffset { get; private set; }
    [field: SerializeField] public float SphereRadius { get; private set; }
    
    [field: Header("Fall Damage")]
    [field: SerializeField] public float FallDamageThreshold { get; private set; }
}
