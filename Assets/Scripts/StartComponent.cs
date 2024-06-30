using RedBjorn.ProtoTiles;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class StartComponent : MonoBehaviour
{
    [SerializeField] private MapSettings _map;
    [SerializeField] private KeyCode _gridToggle = KeyCode.G;
    [SerializeField] private MapView _mapView;

    private List<UnitFSM> _allUnitList = new();
    private GameObject _selector;

    public MapEntity MapEntity { get; private set; }

    private void Awake()
    {
        Init();
    }

    private void Init()
    {
        MapEntity = new MapEntity(_map, _mapView);
        _mapView.Init(MapEntity);
        
        _allUnitList = FindObjectsByType<UnitFSM>(FindObjectsSortMode.InstanceID).ToList();
        if (_allUnitList.Count > 0)
        {
            foreach (var unit in _allUnitList)
            {
                unit.Init(MapEntity);
            }
        }
        _selector = new GameObject("Selector");
        _selector.transform.parent = gameObject.transform;
        _selector.AddComponent<Selector>();
    }

    private void Update()
    {
        if (Input.GetKeyUp(_gridToggle))
        {
            MapEntity.GridToggle();
        }
    }

    private void OnDisable()
    {
        Destroy(_selector);
    }
}
