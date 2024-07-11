public class IdleNotSelectedState : BaseState
{
    public IdleNotSelectedState(UnitFSM context) : base(context) 
    {
        _crossFadeTime = 0.2f;
        _animationLayer = 0;
        _animationName = "IdleNotSelected";
    }

    public override void CheckSwitchState()
    {
        base.CheckSwitchState();
        if(TeamTurnManager.Instance.ActiveTeam != _context.Team)
        {
            return;
        }
        if(Selector.SelectedUnit == _context 
            && _context.CMode == UnitFSM.ControlMode.Player)
        {
            SwitchState(FactoryFSM.IdleSelectedStatePlayer(_context));
        }
        if (Selector.SelectedUnit == _context
            && _context.CMode == UnitFSM.ControlMode.AIHostile)
        {
            SwitchState(FactoryFSM.IdleSelectedStateAI(_context));
        }
    }

    public override void EnterState()
    {
        _context.Animator.StopPlayback();
        _context.Animator.CrossFade(_animationName, _crossFadeTime, _animationLayer);
        FSMMinorActions.TakePosition(true, _context);
    }

    public override void ExitState()
    {
        base.ExitState();
        _context.StartCoroutine(WaitForAnimationToEnd("IdleSelected", 0, 1f));
        FSMMinorActions.TakePosition(false, _context);
    }
}
