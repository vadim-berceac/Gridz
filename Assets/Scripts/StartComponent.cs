using RedBjorn;
using RedBjorn.ProtoTiles;
using UnityEngine;

public class StartComponent : MonoBehaviour
{
    public MapSettings Map;
    public KeyCode GridToggle = KeyCode.G;
    public MapView MapView;
    public Unit Unit;

    public MapEntity MapEntity { get; private set; }

    void Start()
    {
        if (!MapView)
        {
#if UNITY_2023_1_OR_NEWER
            MapView = FindFirstObjectByType<MapView>();
#else
                MapView = FindObjectOfType<MapView>();
#endif
        }
        MapEntity = new MapEntity(Map, MapView);
        if (MapView)
        {
            MapView.Init(MapEntity);
        }
        else
        {
            Log.E("Can't find MapView. Random errors can occur");
        }

        if (!Unit)
        {
#if UNITY_2023_1_OR_NEWER
            Unit = FindFirstObjectByType<Unit>();
#else
                Unit = FindObjectOfType<UnitMove>();
#endif
        }
        if (Unit)
        {
            Unit.Init(MapEntity);
        }
        else
        {
            Log.E("Can't find any Unit. Example level start incorrect");
        }
    }

    void Update()
    {
        if (Input.GetKeyUp(GridToggle))
        {
            MapEntity.GridToggle();
        }
    }
}
