using RedBjorn.ProtoTiles;
using RedBjorn.ProtoTiles.Example;
using RedBjorn.Utils;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "UnitPathAndArea", menuName = "Scriptable Objects/UnitPathAndArea")]
public class UnitPathAndArea : ScriptableObject
{
    [SerializeField] private AreaOutline _areaOutline;
    [SerializeField] private PathDrawer _pathDrawer;

    public AreaOutline AreaOutline => _areaOutline;
    public PathDrawer PathDrawer => _pathDrawer;

    public void AreaShow(AreaOutline area, MapEntity map, Vector3 position, UnitStats stats)
    {
        AreaHide(area);
        area.Show(map.WalkableBorder(position, stats.MoveRange), map);
    }

    public void AreaHide(AreaOutline area)
    {
        area.Hide();
    }

    public void PathCreate(ref PathDrawer path, MapEntity map)
    {
        if (!path)
        {
            path = Spawner.Spawn(_pathDrawer, Vector3.zero, Quaternion.identity);
            path.Show(new List<Vector3>() { }, map);
            path.InactiveState();
            path.IsEnabled = true;
        }
    }

    public void PathHide(PathDrawer path)
    {
        if (path)
        {
            path.Hide();
        }
    }

    public void PathUpdate(AreaOutline area, PathDrawer path, MapEntity map, Vector3 position, UnitStats stats)
    {
        if(!path || !path.IsEnabled)
        {
            return;
        }
        var tile = map.Tile(NewInput.GroundPosition(map.Settings.Plane()));
        if (tile != null && tile.Vacant)
        {
            var newpath = map.PathPoints(position, map.WorldPosition(tile.Position), stats.MoveRange);
            path.Show(newpath, map);
            path.ActiveState();
            area.ActiveState();
        }
        else
        {
            path.InactiveState();
            area.InactiveState();
        }
    }
}
