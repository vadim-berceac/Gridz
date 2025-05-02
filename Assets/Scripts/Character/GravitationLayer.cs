using Unity.Burst;
using UnityEngine;

public class GravitationLayer : MonoBehaviour
{
    [field: SerializeField] public GravitationSettings GravitationSettings { get; set; }

    private float _maxHeightReached;
    private bool _wasGroundedLastFrame;
    private float _currentFallSpeed;
    public event System.Action<float> OnFallDamage; 
    private bool _isGrounded;
    private bool _freeFall;

    public bool IsGrounded => _isGrounded;
    public bool IsFreeFall => _freeFall;
    public Transform CashedTransform { get; private set; }

    private void Awake()
    {
        Initialize();
    }

    protected virtual void Initialize()
    {
        CashedTransform = transform;
        _maxHeightReached = CashedTransform.position.y;
    }

    protected virtual void FixedUpdate()
    {
        UpdateGrounded();
        UpdateFallDetection();
    }
    
    [BurstCompile]
    private void UpdateFallDetection()
    {
        var currentHeight = CashedTransform.position.y;
        
        if (!_isGrounded)
        {
            _maxHeightReached = Mathf.Max(_maxHeightReached, currentHeight);
            _freeFall = true;
        }
        
        if (_isGrounded && !_wasGroundedLastFrame)
        {
            var fallDistance = _maxHeightReached - currentHeight;
            if (fallDistance > GravitationSettings.FallDamageThreshold)
            {
                OnFallDamage?.Invoke(fallDistance);
            }

            _maxHeightReached = currentHeight; 
            _freeFall = false;
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

        // if (_isGrounded)
        // {
        //     Debug.LogWarning(hitColliders[0].name + " is grounded");
        // }
    }

    // private void OnDrawGizmosSelected()
    // {
    //     Gizmos.color = Color.red;
    //     Gizmos.DrawSphere(transform.position + GravitationSettings.GroundOffset, GravitationSettings.SphereRadius);
    // }
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
