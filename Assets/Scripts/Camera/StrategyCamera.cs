using RedBjorn.ProtoTiles;
using UnityEngine;
using UnityEngine.InputSystem;

// basic coordiantes 
// Vector3(11.4899998,15,-10.6000004)
// Quaternion(0.3460913,-0.354817897,0.143355697,0.856606185)
public class StrategyCamera : MonoBehaviour
{
    [SerializeField] private float _eps = 0.1f;
    private static Transform _transformToFollow;
    private static StrategyCamera _instance;
    private Vector3? _holdPosition;
    private Vector3? _clickPosition;
    private MapEntity _cachedMap;
    private MapEntity Map => GetMap();
    private StartComponent _startComponent;
    private Vector3 _delta;
    private Vector3 _newPosition;
    private Vector3 _startCameraPosition;
    private static bool _isMovingByPlayer;
    public static bool IsMovingByPlayer => _isMovingByPlayer;

    private void Awake()
    {
        _startCameraPosition = transform.position;
        _instance = this;
    }

    private void LateUpdate()
    {
        HandleButtonsInput();
        UpdatePosition();
    }

    public static void SetTarget(Transform transformToFollow)
    {
        if(_instance == null)
        {
            Debug.LogWarning("Strategic camera does not exist");
            return;
        }
        _transformToFollow = transformToFollow;
    }

    private void OnDisable()
    {
        _isMovingByPlayer = false;
    }

    private MapEntity GetMap()
    {
        if (_cachedMap == null)
        {
            _startComponent = FindFirstObjectByType<StartComponent>();
            if (_startComponent)
            {
                _cachedMap = _startComponent.MapEntity;
            }
        }
        return _cachedMap;
    }

    private void HandleButtonsInput()
    {
        if (NewInput.GetOnWorldDownFree(Map.Settings.Plane()))
        {
            _holdPosition = NewInput.GroundPositionCameraOffset(Map.Settings.Plane());
            _clickPosition = transform.position;
        }
        else if (NewInput.GetOnWorldUpFree(Map.Settings.Plane()))
        {
            _holdPosition = null;
            _clickPosition = null;
        }
    }

    private void UpdatePosition()
    {
        FollowTarget();
        if (_holdPosition.HasValue)
        {
            _delta = _holdPosition.Value - NewInput.GroundPositionCameraOffset(Map.Settings.Plane());
            transform.position += _delta;
            transform.position = _clickPosition.Value + _delta;
            if (!_isMovingByPlayer)
            {
                _isMovingByPlayer = _delta.sqrMagnitude > _eps;
            }
        }
        else
        {
            _isMovingByPlayer = false;
        }
    }

    private void FollowTarget()
    {        
        if (_transformToFollow == null)
        {
            return;
        }
        _newPosition = _transformToFollow.position + _startCameraPosition;
        transform.position = Vector3.Slerp(transform.position, _newPosition, 0.03f);
    }
}