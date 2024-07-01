using RedBjorn.ProtoTiles;
public class IdleNotSelectedState : BaseState
{
    private TileEntity _currentPosition;
    public IdleNotSelectedState(UnitFSM context) : base(context) 
    {
        _crossFadeTime = 0.2f;
        _animationLayer = 0;
        _animationName = "Idle";
    }

    public override void CheckSwitchState()
    {
        base.CheckSwitchState();
        if(Selector.SelectedUnit == _context 
            && _context.CMode == UnitFSM.ControlMode.Player)
        {
            SwitchState(FactoryFSM.IdleSelectedStatePlayer(_context));
        }
        if (Selector.SelectedUnit == _context
            && _context.CMode == UnitFSM.ControlMode.AI)
        {
            SwitchState(FactoryFSM.IdleSelectedStateAI(_context));
        }
    }

    public override void EnterState()
    {
        _context.Animator.StopPlayback();
        _context.Animator.CrossFade(_animationName, _crossFadeTime, _animationLayer);
        UpdatePositionObtacle(true);
    }

    public override void ExitState()
    {
        UpdatePositionObtacle(false);
    }

    private void UpdatePositionObtacle(bool value)
    {
        _currentPosition = _context.Map.Tile(_context.transform.position);
        _currentPosition.SetObtacle(value);
    }
}
