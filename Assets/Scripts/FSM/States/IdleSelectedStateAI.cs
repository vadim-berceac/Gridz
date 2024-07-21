using RedBjorn.ProtoTiles.Example;

public class IdleSelectedStateAI : BaseState
{
    private AreaOutline _area;
    private PathDrawer _path;
    private bool _needToMove;
    private PathUpdater _inputHandler;

    public IdleSelectedStateAI(UnitFSM context) : base(context)
    {
        _crossFadeTime = 0.2f;
        _animationLayer = 0;
        _animationName = "IdleNotSelected";
    }

    public override void CheckSwitchState()
    {
        base.CheckSwitchState();
        if (Selector.ActiveUnit == null && !_context.TilePath.Contains(_inputHandler.TileEntity)
            && _inputHandler.TileEntity != null)
        {
            SwitchState(FactoryFSM.IdleNotSelectedState(_context));
        }
        if (Selector.ActiveUnit != null && Selector.ActiveUnit != _context)
        {
            SwitchState(FactoryFSM.IdleNotSelectedState(_context));
        }
        if (_context.CMode == UnitFSM.ControlMode.Player)
        {
            SwitchState(FactoryFSM.IdleSelectedStatePlayer(_context));
        }
        if (Selector.SelectedAsTargetUnit)
        {
            SwitchState(FactoryFSM.AttackState(_context));
        }
    }

    public override void EnterState()
    {
        _inputHandler = new AIPathUpdater(_context);
        _context.Animator.StopPlayback();
        _context.Animator.CrossFade(_animationName, _crossFadeTime, _animationLayer);
    }
}
