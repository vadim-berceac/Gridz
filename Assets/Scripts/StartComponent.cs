using RedBjorn.ProtoTiles;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class StartComponent : MonoBehaviour
{
    [SerializeField] private MapSettings _map;
    [SerializeField] private KeyCode _gridToggle = KeyCode.G;
    [SerializeField] private MapView _mapView;

    private List<Unit> _allUnitList = new();

    public MapEntity MapEntity { get; private set; }

    private void Start()
    {
        Init();
    }

    private void Init()
    {
        MapEntity = new MapEntity(_map, _mapView);
        _mapView.Init(MapEntity);
        
        _allUnitList = FindObjectsByType<Unit>(FindObjectsSortMode.InstanceID).ToList();
        if (_allUnitList.Count > 0)
        {
            foreach (var unit in _allUnitList)
            {
                unit.Init(MapEntity);
            }
        }
    }

    private void Update()
    {
        if (Input.GetKeyUp(_gridToggle))
        {
            MapEntity.GridToggle();
        }
    }
}
