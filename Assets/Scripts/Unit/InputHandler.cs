using RedBjorn.ProtoTiles;
using RedBjorn.ProtoTiles.Example;
using System;
using System.Collections.Generic;
using UnityEngine;

public abstract class InputHandler
{
    protected UnitFSM _unit;
    protected Vector3 _clickPosition;
    protected TileEntity _tileEntity;

    public Vector3 ClickPosition => _clickPosition;
    public TileEntity TileEntity => _tileEntity;
    public InputHandler(UnitFSM unit)
    {
        _unit = unit;
    }

    public abstract void Update(ref List<TileEntity> tilePath, Action action, AreaOutline area, PathDrawer path);
}
