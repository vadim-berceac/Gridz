using RedBjorn.ProtoTiles;
using RedBjorn.Utils;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Movement
{
    private readonly Unit _unit;
    private readonly string _moveAnimation = "move";
    private readonly string _idleAnimation = "idle";
    private readonly float _rotationTime = 0.3f;
    private float _rotationTimer;
    private Vector3 _targetPoint;
    private Vector3 _stepDirection;
    private Quaternion _startRotation;
    private Quaternion _targetRotation;
    private int _nextIndex;
    private bool _reached;
    private bool _isRotating = false;
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
        _unit.Animator.Play(_moveAnimation);
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
        _unit.Animator.CrossFade(_idleAnimation, 0.3f, 0);
    }

    public void Rotate(List<TileEntity> path)
    {
        _unit.StartCoroutine(RotateSmoothly(path));
    }

    private IEnumerator RotateSmoothly(List<TileEntity> path)
    {
        if (path == null || _nextIndex >= path.Count || _stepDirection == null)
        {
            yield break;
        }

        if (_isRotating)
        {
            yield return new WaitUntil(() => !_isRotating);
        }

        _isRotating = true;

        _startRotation = _unit.RotationNode.rotation;
        if (_unit.Map.RotationType == RotationType.LookAt)
        {
            _targetRotation = Quaternion.LookRotation(_stepDirection, Vector3.up);
        }
        else
        {
            _targetRotation = _unit.Map.Settings.Flip(_stepDirection);
        }
        _rotationTimer = 0f;

        while (_rotationTimer < _rotationTime)
        {
            _unit.RotationNode.rotation = Quaternion.Lerp(_startRotation, _targetRotation, _rotationTimer / _rotationTime);
            _rotationTimer += Time.deltaTime;
            yield return null;
        }

        _unit.RotationNode.rotation = _targetRotation;
        _isRotating = false;
    }
}
