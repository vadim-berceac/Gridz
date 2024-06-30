using RedBjorn.ProtoTiles;
using System.Collections;
using UnityEngine;

public class RotationSubState : BaseState
{
    private bool _isRotating = false;
    private Quaternion _startRotation;
    private Quaternion _targetRotation;
    private readonly float _rotationTime = 0.3f;
    private float _rotationTimer;

    public RotationSubState(UnitFSM context) : base(context)
    {
        //_crossFadeTime = 0.2f;
       // _animationLayer = 0;
       // _animationName = "Rotation";
    }

    public override void EnterState()
    {
        Rotate();
    }

    public void Rotate()
    {
        _context.StartCoroutine(RotateSmoothly());
    }

    private IEnumerator RotateSmoothly()
    {
        if (_isRotating == true)
        {
            yield return new WaitUntil(() => _isRotating == false);
        }

        _isRotating = true;

        _startRotation = _context.RotationNode.rotation;
        if (_context.Map.RotationType == RotationType.LookAt)
        {
            _targetRotation = Quaternion.LookRotation(_parentState.Direction, Vector3.up);
        }
        else
        {
            _targetRotation = _context.Map.Settings.Flip(_parentState.Direction);
        }
        _rotationTimer = 0f;

        while (_rotationTimer < _rotationTime)
        {
            _context.RotationNode.rotation = Quaternion.Lerp
                (_startRotation, _targetRotation, _rotationTimer / _rotationTime);
            _rotationTimer += Time.deltaTime;
            yield return null;
        }

        _context.RotationNode.rotation = _targetRotation;
        _isRotating = false;
    }
}
