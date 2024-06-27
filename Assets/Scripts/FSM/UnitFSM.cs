using RedBjorn.ProtoTiles;
using RedBjorn.ProtoTiles.Example;
using RedBjorn.Utils;
using System.Collections.Generic;
using UnityEngine;

public class UnitFSM : MonoBehaviour
{
    public enum ControlMode
    {
        Player,
        AI
    }
    [Header("Input Settings")]
    [SerializeField] private ControlMode _controlMode;

    [Header("Params")]
    [SerializeField] private Health _health;
    [SerializeField] private UnitPattern _unitPattern;
    [SerializeField] private UnitPathAndArea _pathAndArea;
    [SerializeField] private BaseState _currentState;

    [Header("Rotation Node")]
    [SerializeField] private Transform _rotationNode;

    private GameObject _model;
    private InputHandler _inputHandler;
    private Animator _animator;
    private MapEntity _map;
    private AreaOutline _area;
    private PathDrawer _path;
    private CameraSetter _cameraSetter;
    private Coroutine _movingCoroutine;
    private List<TileEntity> _tilePath;

    public Health Health => _health;
    public Animator Animator => _animator;
    public UnitPattern UnitPattern => _unitPattern;
    public UnitPathAndArea PathAndArea => _pathAndArea;
    public Transform RotationNode => _rotationNode;
    public MapEntity Map => _map;
    public AreaOutline Area => _area;
    public PathDrawer Path => _path;

    public void Init(MapEntity map)
    {
        if(!_unitPattern)
        {
            Debug.Log($"Initialization of unit {name} in impossible, UnitPatternt - missed!");
            return;
        }
        _map = map;
        SetControlMode(_controlMode);
        _area = Spawner.Spawn(_pathAndArea.AreaOutline, Vector3.zero, Quaternion.identity);
        _pathAndArea.AreaShow(_area, _map, transform.position, _unitPattern);
        _pathAndArea.PathCreate(ref _path, _map);
        _cameraSetter = new(transform);        
        _model = Instantiate(_unitPattern.Prefab, _rotationNode);
        _model.transform.localScale = _unitPattern.ModelScale;
        _animator = _model.GetComponent<Animator>();
        _health.Init(_unitPattern.MaxHealthValue);
        name = _unitPattern.Name;       
        _currentState = FactoryFSM.IdleNotSelectedState(this);
        _currentState.EnterState();
        _health.OnHealthChanged += OnDamage;
        _health.OnDeath += OnDeath;
    }

    private void Update()
    {
        _currentState.UpdateState();
        _inputHandler.Update(ref _tilePath, OnInput);
        _pathAndArea.UpdatePath(_area, _path, _map, transform.position, _unitPattern);
    }

    public void SetNewState(BaseState newState)
    {
        _currentState.SwitchState(newState);
    }

    public void SetControlMode(ControlMode mode)
    {
        _controlMode = mode;
        if(_controlMode == ControlMode.Player)
        {
            _inputHandler = new ClickInputHandler(this);
        }
        else
        {
            //_inputHandler = new AIInputHandler(this);
        }
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
        //Movement.Move(ref _movingCoroutine, _tilePath, () =>
        //{
        //    Path.IsEnabled = true;
        //    UnitPathAndArea.AreaShow(Area, Map, transform.position, UnitPattern);
         //   _cameraSetter.EnableFreeCamera();
        //});
    }
}
