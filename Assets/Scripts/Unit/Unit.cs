using RedBjorn.ProtoTiles;
using RedBjorn.ProtoTiles.Example;
using RedBjorn.Utils;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Health))]
public class Unit : MonoBehaviour
{
    [SerializeField] private Animator _animator;
    [SerializeField] private Health _health;
    [SerializeField] private UnitPattern _stats;
    [SerializeField] private UnitPathAndArea _pathAndArea;
    [SerializeField] private Transform _rotationNode;

    private CameraSetter _cameraSetter;
    private InputHandler _inputHandler;
    private Movement _movement;
    private MapEntity _map;
    private AreaOutline _area;
    private PathDrawer _path;
    private Coroutine _movingCoroutine;
    private List<TileEntity> _tilePath;

    public Movement Movement => _movement;
    public Animator Animator => _animator;
    public AreaOutline Area => _area;
    public PathDrawer Path => _path;
    public UnitPathAndArea UnitPathAndArea => _pathAndArea;
    public MapEntity Map => _map;
    public UnitPattern Stats => _stats;
    public Transform RotationNode
    {
        get => _rotationNode;
        set => _rotationNode = value;
    }

    private void Update()
    {        
        _inputHandler.Update(ref _tilePath, OnInput);
        _movement.Rotate(_tilePath);
        _pathAndArea.UpdatePath(_area, _path, _map, transform.position, _stats);
    }

    public void Init(MapEntity map)
    {
        _map = map;
        _area = Spawner.Spawn(_pathAndArea.AreaOutline, Vector3.zero, Quaternion.identity);
        _pathAndArea.AreaShow(_area, _map, transform.position, _stats);
        _pathAndArea.PathCreate(ref _path, _map);
        _cameraSetter = new(transform);
        _movement = new(this);
        //_inputHandler = new ClickInputHandler(this);
        _health.OnHealthChanged += OnDamage;
        _health.OnDeath += OnDeath;
    }

    public float GetEfficiencyRating(bool isHostile, float distance)
    {
        //предназначен для сортировки и выбора цели AI

        //Складываем текущее значение здоровья, растояние до цели
        //режим боя (ближний, дальний и тд)
        return _health.CurrentValue - distance;
    }

    private void OnDamage(object source, float oldHP, float newHP)
    {
        Debug.LogWarning($"{source} наносит урон {name}, было {oldHP} - стало {newHP}");
    }

    private void OnDeath(object source)
    {
        Debug.LogWarning($"{source} убивает {name}");
    }

    private void OnDisable()
    {
        _health.OnHealthChanged -= OnDamage;
        _health.OnDeath -= OnDeath;
    }

    private void OnInput()
    {
        _cameraSetter.SetCameraTarget();
        _movement.Move(ref _movingCoroutine, _tilePath, () =>
        {
            Path.IsEnabled = true;
            _pathAndArea.AreaShow(Area, Map, transform.position, Stats);
            _cameraSetter.EnableFreeCamera();
        });
    }
}
