using RedBjorn.ProtoTiles;
using RedBjorn.ProtoTiles.Example;
using RedBjorn.Utils;
using System.Collections.Generic;
using UnityEngine;

public class Unit : MonoBehaviour
{
    [SerializeField] private UnitStats _stats;
    [SerializeField] private UnitPathAndArea _pathAndArea;
    [SerializeField] private Transform _rotationNode;

    private Movement _movement;
    private MapEntity _map;
    private AreaOutline _area;
    private PathDrawer _path;
    private Coroutine _movingCoroutine;

    private Vector3 _clickPosition;
    private TileEntity _tileEntity;
    private List<TileEntity> _tilePath;

    public MapEntity Map => _map;
    public UnitStats Stats => _stats;
    public Transform RotationNode
    {
        get => _rotationNode;
        set => _rotationNode = value;
    }

    private void Update()
    {
        HandleWorldClick();
        _pathAndArea.PathUpdate(_area, _path, _map, transform.position, _stats);
    }

    public void Init(MapEntity map)
    {
        _map = map;
        _area = Spawner.Spawn(_pathAndArea.AreaOutline, Vector3.zero, Quaternion.identity);
        _pathAndArea.AreaShow(_area, _map, transform.position, _stats);
        _pathAndArea.PathCreate(ref _path, _map);
        _movement = new(this);
    }

    private void HandleWorldClick()
    {
        if (!NewInput.GetOnWorldUp(_map.Settings.Plane()))
        {
            return;
        }
        _clickPosition = NewInput.GroundPosition(_map.Settings.Plane());
        _tileEntity = _map.Tile(_clickPosition);
        if (_tileEntity != null && _tileEntity.Vacant)
        {
            _pathAndArea.AreaHide(_area);
            _path.IsEnabled = false;
            _pathAndArea.PathHide(_path);
            _tilePath = _map.PathTiles(transform.position, _clickPosition, _stats.MoveRange);
            _movement.Move(ref _movingCoroutine, _tilePath, () =>
            {
                _path.IsEnabled = true;
                _pathAndArea.AreaShow(_area, _map, transform.position, _stats);
            });
        }
    }
}
