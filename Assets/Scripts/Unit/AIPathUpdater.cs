using RedBjorn.ProtoTiles;
using RedBjorn.ProtoTiles.Example;
using System;
using System.Collections.Generic;

public class AIPathUpdater : PathUpdater
{
    public AIPathUpdater(UnitFSM unit) : base(unit)
    {
    }

    public override void Update(ref List<TileEntity> tilePath, Action action, AreaOutline area, PathDrawer path)
    {
        
    }
}
