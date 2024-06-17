using RedBjorn.ProtoTiles;
using UnityEngine;

public class StrategyCamera : MonoBehaviour
{
    [SerializeField] private float _eps = 0.1f;

    private Vector3? _holdPosition;
    private Vector3? _clickPosition;
    private MapEntity _cachedMap;
    private MapEntity Map => GetMap();
    private StartComponent _startComponent;
    private Vector3 _delta;

    private static bool _isMovingByPlayer;

    public static bool IsMovingByPlayer => _isMovingByPlayer;

    private void LateUpdate()
    {
        HandleInput();
        UpdatePosition();
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

    private void HandleInput()
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
        if (_holdPosition.HasValue)
        {
            _delta = (_holdPosition.Value - NewInput.GroundPositionCameraOffset(Map.Settings.Plane()));
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
}