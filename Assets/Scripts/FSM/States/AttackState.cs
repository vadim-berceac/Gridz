using RedBjorn.ProtoTiles;
using UnityEngine;

public class AttackState : BaseState
{
    public AttackState(UnitFSM context) : base(context)
    {
        _animationLayer = 0;
        _animationName = "Attack";
    }

    public override void CheckSwitchState()
    {
        base.CheckSwitchState();
        if (_subState.IsComplete && _context.CMode == UnitFSM.ControlMode.Player)
        {
            SwitchState(FactoryFSM.IdleSelectedStatePlayer(_context));
        }
        if(_subState.IsComplete && _context.CMode != UnitFSM.ControlMode.Player)
        {
            SwitchState(FactoryFSM.IdleSelectedStateAI(_context));
        }
    }

    public override void EnterState()
    {
        _context.Animator.StopPlayback();
        _context.CurrentMoveRange = 0;
        _context.DirectionOfView = Selector.SelectedAsTargetUnit.transform.position - _context.transform.position;        
        _context.CameraSetter.SetCameraTarget();
        SetNewSubState(FactoryFSM.RotationSubState(_context));
        Selector.BlockNewUnitActivation(true);
        Selector.BlockNewUnitAsTargetSelection(true);
        DoDamage(Selector.SelectedAsTargetUnit);        
    }

    public override void ExitState()
    {
        _context.AttackPosition = Vector3.zero;
        _context.CameraSetter.EnableFreeCamera();
        Selector.BlockNewUnitActivation(false);
        Selector.BlockNewUnitAsTargetSelection(false);
        Selector.ResetAttackTarget();
        _context.StartCoroutine(WaitForAnimationToEnd(_animationName, _animationLayer, 1f));
        base.ExitState();
    }

    private void DoDamage(UnitFSM toUnit)
    {
        //בüוע ןמ ןכמשאהט
    }
}
