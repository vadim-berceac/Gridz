using RedBjorn.ProtoTiles;
using RedBjorn.Utils;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovingState : BaseState
{
    private int _nextIndex;
    private bool _reached;
    private bool _movementComplete = false;
    private Vector3 _targetPoint;

    public MovingState(UnitFSM context) : base(context)
    {
    }

    public override void CheckSwitchState()
    {
        base.CheckSwitchState();  
        if(_movementComplete && _context.CMode == UnitFSM.ControlMode.Player)
        {
            SwitchState(FactoryFSM.IdleSelectedStatePlayer(_context));
        }
        if (_movementComplete && _context.CMode == UnitFSM.ControlMode.AIHostile)
        {
            SwitchState(FactoryFSM.IdleSelectedStateAI(_context));
        }
        if (_context.TilePath != null && _nextIndex < _context.TilePath.Count 
            && _context.DirectionOfView != Vector3.zero)
        {            
            SetNewSubState(FactoryFSM.RotationSubState(_context));
        }
    }

    public override void EnterState()
    {
        _context.CameraSetter.SetCameraTarget();
        Selector.BlockNewUnitActivation(true);
        Move(_context.TilePath, OnPathEnd);
    }

    public override void UpdateState()
    {
        base.UpdateState(); 
    }

    public override void ExitState()
    {
        base.ExitState();
        _context.CameraSetter.EnableFreeCamera();
        Selector.BlockNewUnitActivation(false);
    }

    public void Move(List<TileEntity> path, Action onCompleted)
    {
        if (path == null)
        {
            onCompleted.SafeInvoke();
            return;
        }
        _context.StartCoroutine(Moving(path, onCompleted));
    }

    private IEnumerator Moving(List<TileEntity> path, Action onCompleted)
    {        
        _nextIndex = 0;
        _context.transform.position = _context.Map.Settings.Projection(_context.transform.position);

        while (_nextIndex < path.Count)
        {
            _targetPoint = _context.Map.WorldPosition(path[_nextIndex]);
            _context.DirectionOfView = (_targetPoint - _context.transform.position) 
                * _context.UnitPattern.MoveSpeed;
            _reached = _context.DirectionOfView.sqrMagnitude < 0.01f;
            while (!_reached)
            {
                _context.transform.position += _context.DirectionOfView * Time.deltaTime;
                _reached = Vector3.Dot(_context.DirectionOfView, 
                    (_targetPoint - _context.transform.position)) < 0f;
                yield return null;
            }
            _context.transform.position = _targetPoint;
            _nextIndex++;
        }
        _context.CurrentMoveRange -= path.Count - 1;
        onCompleted.SafeInvoke();
    }

    private void OnPathEnd()
    {
        _movementComplete = true;
    }
}
