using RedBjorn.ProtoTiles;
using System.Collections.Generic;
using UnityEngine;

public abstract class InputHandler
{
    protected Unit _unit;
    public InputHandler(Unit unit)
    {
        _unit = unit;
    }

    public abstract void Update(ref List<TileEntity> tilePath, ref Coroutine movingCoroutine);
}
