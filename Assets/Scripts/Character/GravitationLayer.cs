using Unity.Burst;
using UnityEngine;
using Zenject;

[RequireComponent(typeof(CharacterController))]
public class GravitationLayer : MonoBehaviour
{
    [field: SerializeField] public GravitationSettings GravitationSettings { get; set; }

    private float _maxHeightReached;
    private bool _wasGroundedLastFrame;
    public event System.Action<float> OnFallDamage; 

    protected CharacterController CharacterController;
    private float _currentFallSpeed = 0f;
    private bool _isOnValidGround;

    public bool IsGrounded { get; private set; }
    protected Transform CashedTransform { get; private set; }
    protected SfxContainer SfxContainer { get; private set; }
    private SfxSet _landingSfx;

    [Inject]
    private void Construct(SfxContainer sfxContainer)
    {
        SfxContainer = sfxContainer;
        _landingSfx = SfxContainer.GetSfxSet(GravitationSettings.LandingSoundsSetName);
    }

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
        ApplyGravitation(ref _currentFallSpeed, GravityConstants.MaxFallSpeed, GravityConstants.GravityForce);
        UpdateFallDetection();
    }
    
    private void OnControllerColliderHit(ControllerColliderHit hit)
    {
        if (hit.normal.y > 0.7f)
        {
            _isOnValidGround = (1 << hit.gameObject.layer & GravitationSettings.GroundLayerMask) != 0;
        }
    }

    [BurstCompile]
    private void ApplyGravitation(ref float currentFallSpeed, float maxFallSpeed, float gravityForce)
    {
        if (_isOnValidGround)
        {
            if (currentFallSpeed < 0f)
            {
                currentFallSpeed = -2f;
            }
        }
        else
        {
            var gravityDelta = gravityForce * Time.fixedDeltaTime;
            currentFallSpeed = Mathf.Max(currentFallSpeed - gravityDelta, -maxFallSpeed);
        }

        var moveVector = currentFallSpeed * Time.fixedDeltaTime * Vector3.up;
        var previousPosition = CharacterController.transform.position;
        CharacterController.Move(moveVector);
        var newPosition = CharacterController.transform.position;

        if (!_isOnValidGround && moveVector.y < 0f && Mathf.Approximately(newPosition.y, previousPosition.y))
        {
            currentFallSpeed = -2f;
        }
    }
    
    [BurstCompile]
    private void UpdateFallDetection()
    {
        var currentHeight = CashedTransform.position.y;
        
        if (!IsGrounded)
        {
            _maxHeightReached = Mathf.Max(_maxHeightReached, currentHeight);
        }
        
        if (IsGrounded && !_wasGroundedLastFrame)
        {
           _landingSfx.PlayRandomAtPoint(CashedTransform.position);
            var fallDistance = _maxHeightReached - currentHeight;
            if (fallDistance > GravitationSettings.FallDamageThreshold && !GravitationSettings.ImmuneToFallDamage)
            {
                OnFallDamage?.Invoke(fallDistance);
            }

            _maxHeightReached = currentHeight; 
        }

        _wasGroundedLastFrame = IsGrounded;
    }
    
    [BurstCompile]
    private void UpdateGrounded()
    {
        var spherePos = CashedTransform.position + GravitationSettings.GroundOffset;
        var hitColliders = new Collider[32]; 
        var hitsCount = Physics.OverlapSphereNonAlloc(spherePos, GravitationSettings.SphereRadius, hitColliders, 
            GravitationSettings.GroundLayerMask);

        IsGrounded = hitsCount > 0;
    }
}

[System.Serializable]
public struct GravitationSettings
{
    [field: SerializeField] public LayerMask GroundLayerMask { get; private set; }

    [field: SerializeField] public Vector3 GroundOffset { get; private set; }
    [field: SerializeField] public float SphereRadius { get; private set; }
    
    [field: Header("Fall Damage")]
    [field: SerializeField] public bool ImmuneToFallDamage { get; private set; }
    [field: SerializeField] public float FallDamageThreshold { get; private set; }
    
    [field: Header("Sound")]
    [field: SerializeField] public string LandingSoundsSetName { get; private set; }
}
