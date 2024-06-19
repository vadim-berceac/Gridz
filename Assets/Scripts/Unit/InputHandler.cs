using RedBjorn.ProtoTiles;
using System;
using System.Collections.Generic;

public abstract class InputHandler
{
    protected Unit _unit;
    public InputHandler(Unit unit)
    {
        _unit = unit;
    }

    public abstract void Update(ref List<TileEntity> tilePath, Action action);
}
