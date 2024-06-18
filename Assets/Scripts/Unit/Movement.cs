using RedBjorn.ProtoTiles;
using RedBjorn.Utils;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Movement
{
    private readonly Unit _unit;

    private Vector3 _targetPoint;
    private Vector3 _stepDirection;
    private int _nextIndex;
    private bool _reached;
    public Movement(Unit unit)
    {
        _unit = unit;
    }
    public void Move(ref Coroutine movingCoroutine, List<TileEntity> path, Action onCompleted)
    {
        if (path == null)
        {
            onCompleted.SafeInvoke();
            return;            
        }
        if (movingCoroutine != null)
        {
            _unit.StopCoroutine(movingCoroutine);
        }
        movingCoroutine = _unit.StartCoroutine(Moving(path, onCompleted));
    }

    private IEnumerator Moving(List<TileEntity> path, Action onCompleted)
    {
        _nextIndex = 0;
        _unit.transform.position = _unit.Map.Settings.Projection(_unit.transform.position);

        while (_nextIndex < path.Count)
        {
            _targetPoint = _unit.Map.WorldPosition(path[_nextIndex]);
            _stepDirection = (_targetPoint - _unit.transform.position) * _unit.Stats.MoveSpeed;
            _reached = _stepDirection.sqrMagnitude < 0.01f;
            while (!_reached)
            {
                _unit.transform.position += _stepDirection * Time.deltaTime;
                _reached = Vector3.Dot(_stepDirection, (_targetPoint - _unit.transform.position)) < 0f;
                yield return null;
            }
            _unit.transform.position = _targetPoint;
            _nextIndex++;
        }
        onCompleted.SafeInvoke();
    }

    public void Rotate(List<TileEntity> path)
    {
        if (path == null || _nextIndex >= path.Count || _stepDirection == null)
        {
            return;
        }
        if (_unit.Map.RotationType == RotationType.LookAt)
        {
            _unit.RotationNode.rotation = Quaternion.LookRotation(_stepDirection, Vector3.up);
        }
        else if (_unit.Map.RotationType == RotationType.Flip)
        {
            _unit.RotationNode.rotation = _unit.Map.Settings.Flip(_stepDirection);
        }
    }
}
