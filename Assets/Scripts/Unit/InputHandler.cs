using RedBjorn.ProtoTiles;
using System;
using System.Collections.Generic;

public abstract class InputHandler
{
    protected UnitFSM _unit;
    public InputHandler(UnitFSM unit)
    {
        _unit = unit;
    }

    public abstract void Update(ref List<TileEntity> tilePath, Action action);
}
