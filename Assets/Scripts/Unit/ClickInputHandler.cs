using RedBjorn.ProtoTiles;
using System.Collections.Generic;
using UnityEngine;

public class ClickInputHandler : InputHandler
{
    private Vector3 _clickPosition;
    private TileEntity _tileEntity;
    public ClickInputHandler(Unit unit) : base(unit)
    {
    }

    public override void Update(ref List<TileEntity> tilePath, ref Coroutine movingCoroutine)
    {
        if (!NewInput.GetOnWorldUp(_unit.Map.Settings.Plane()))
        {
            return;
        }
        _clickPosition = NewInput.GroundPosition(_unit.Map.Settings.Plane());
        _tileEntity = _unit.Map.Tile(_clickPosition);
        if (_tileEntity != null && _tileEntity.Vacant)
        {
            _unit.UnitPathAndArea.AreaHide(_unit.Area);
            _unit.Path.IsEnabled = false;
            _unit.UnitPathAndArea.PathHide(_unit.Path);
            tilePath = _unit.Map.PathTiles(_unit.transform.position, _clickPosition, _unit.Stats.MoveRange);
            _unit.Movement.Move(ref movingCoroutine, tilePath, () =>
            {
                _unit.Path.IsEnabled = true;
                _unit.UnitPathAndArea.AreaShow(_unit.Area, _unit.Map, _unit.transform.position, _unit.Stats);
            });
        }
    }
}
