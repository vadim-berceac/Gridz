using RedBjorn.ProtoTiles;
using RedBjorn.ProtoTiles.Example;
using RedBjorn.Utils;
using System;
using System.Collections.Generic;

public class ClickInputHandler : InputHandler
{
    public ClickInputHandler(UnitFSM unit) : base(unit)
    {

    }

    public override void Update(ref List<TileEntity> tilePath, Action action, AreaOutline area, PathDrawer path)
    {
        if (!NewInput.GetOnWorldUp(_unit.Map.Settings.Plane()))
        {
            return;
        }
        _clickPosition = NewInput.GroundPosition(_unit.Map.Settings.Plane());
        _tileEntity = _unit.Map.Tile(_clickPosition);
        if (_tileEntity != null && _tileEntity.Vacant)
        {
            _unit.PathAndArea.AreaHide(area);
            path.IsEnabled = false;
            _unit.PathAndArea.PathHide(path);
            tilePath = _unit.Map.PathTiles(_unit.transform.position, _clickPosition, _unit.UnitPattern.MoveRange);
            action.SafeInvoke();
        }
    }
}
