
public static class FSMMinorActions
{
    public static void TakePosition(bool value, UnitFSM unit)
    {
        unit.CurrentPosition?.SetObtacle(false, null);
        unit.CurrentPosition = unit.Map.Tile(unit.transform.position);
        unit.CurrentPosition.SetObtacle(value, unit);
    }
}
