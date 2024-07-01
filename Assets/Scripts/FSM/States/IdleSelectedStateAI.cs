using RedBjorn.ProtoTiles.Example;

public class IdleSelectedStateAI : BaseState
{
    private AreaOutline _area;
    private PathDrawer _path;
    private bool _needToMove;
    private InputHandler _inputHandler;

    public IdleSelectedStateAI(UnitFSM context) : base(context)
    {
        _crossFadeTime = 0.2f;
        _animationLayer = 0;
        _animationName = "Idle";
    }

    public override void CheckSwitchState()
    {
        base.CheckSwitchState();
        if (Selector.SelectedUnit == null && !_context.TilePath.Contains(_inputHandler.TileEntity)
            && _inputHandler.TileEntity != null)
        {
            SwitchState(FactoryFSM.IdleNotSelectedState(_context));
        }
        if (Selector.SelectedUnit != null && Selector.SelectedUnit != _context)
        {
            SwitchState(FactoryFSM.IdleNotSelectedState(_context));
        }
        if (_context.CMode == UnitFSM.ControlMode.Player)
        {
            SwitchState(FactoryFSM.IdleSelectedStatePlayer(_context));
        }
    }

    public override void EnterState()
    {
        _inputHandler = new AIInputHandler(_context);
        _context.Animator.StopPlayback();
        _context.Animator.CrossFade(_animationName, _crossFadeTime, _animationLayer);
    }
}
