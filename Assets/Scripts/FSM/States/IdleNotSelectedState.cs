using UnityEngine;

public class IdleNotSelectedState : BaseState
{    
    public IdleNotSelectedState(UnitFSM context) : base(context)
    {
        _crossFadeTime = 0.2f;
        _animationLayer = 0;
        _animationName = "Idle";
    }

    public override void EnterState()
    {
        _context.Animator.StopPlayback();
        _context.Animator.CrossFade(_animationName, _crossFadeTime, _animationLayer);
    }
}
