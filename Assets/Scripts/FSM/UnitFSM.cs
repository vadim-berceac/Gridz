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
    //private InputHandler _inputHandler;
    private Animator _animator;
    private MapEntity _map;
    private CameraSetter _cameraSetter;

    public ControlMode CMode
    {
        get => _controlMode;
        set => _controlMode = value;
    }    

    public List<TileEntity> TilePath;
    public Health Health => _health;
    public Animator Animator => _animator;
    public UnitPattern UnitPattern => _unitPattern;
    public UnitPathAndArea PathAndArea => _pathAndArea;
    public Transform RotationNode => _rotationNode;
    public MapEntity Map => _map;
    public CameraSetter CameraSetter => _cameraSetter;

    public void Init(MapEntity map)
    {
        if(!_unitPattern)
        {
            Debug.Log($"Initialization of unit {name} in impossible, UnitPatternt - missed!");
            return;
        }
        _map = map;       
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
    }

    public void SetNewState(BaseState newState)
    {
        _currentState = newState;
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
}
