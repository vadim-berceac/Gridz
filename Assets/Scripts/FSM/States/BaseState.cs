using System.Collections;
using UnityEngine;

public abstract class BaseState
{
    public bool IsComplete;

    protected UnitFSM _context;
    protected BaseState _parentState;
    protected BaseState _subState;
    protected float _crossFadeTime;
    protected int _animationLayer;
    protected string _animationName;

    public BaseState(UnitFSM context)
    {
        _context = context;
    }
    public void SetParentState(BaseState parent)
    {
        _parentState = parent;
    }
    public abstract void EnterState();
    public virtual void UpdateState()
    {
        _subState?.UpdateState();
        CheckSwitchState();
    }
    public virtual void FixedUpdateState()
    {
        _subState?.FixedUpdateState();
    }
    public virtual void ExitState()
    {
        _subState?.ExitState();
        _context.StopAllCoroutines();
    }
    public virtual void CheckSwitchState()
    {
        _subState?.CheckSwitchState();
    }
    public void SwitchState(BaseState newState)
    {
        ExitState();
        newState.EnterState();
        _context.SetNewState(newState);
    }
    protected void SwitchBetweenSubState(BaseState newState)
    {
        ExitState();
        newState.EnterState();
        _parentState.SetNewSubState(newState);
        newState.SetParentState(_parentState);
    }
    protected void SetNewSubState(BaseState newState)
    {
        _subState = newState;
        _subState._parentState = this;
        _subState.EnterState();
    }

    protected IEnumerator WaitForAnimationToEnd(string animationName, int layer, float clipEndOn)
    {
        _context.Animator.Play(animationName);
        while (_context.Animator.GetCurrentAnimatorStateInfo(layer).normalizedTime < clipEndOn)
        {
            yield return new WaitForEndOfFrame();
        }
    }
}
