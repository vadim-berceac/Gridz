using RedBjorn.ProtoTiles;
using UnityEngine;

public class RotationSubState : BaseState
{
    private Quaternion _targetRotation;
    private readonly float _rotationSpeed = 180f;

    public RotationSubState(UnitFSM context) : base(context)
    {
        //_crossFadeTime = 0.2f;
       // _animationLayer = 0;
       // _animationName = "Rotation";
    }

    public override void EnterState()
    {
        Rotate(_parentState.DirectionOfView);
    }

    public override void UpdateState()
    {
        base.UpdateState();
        UpdateRotation();
    }

    private void Rotate(Vector3 direction)
    {        
        if (_context.Map.RotationType == RotationType.LookAt)
        {
            _targetRotation = Quaternion.LookRotation(direction, Vector3.up);
        }
        else
        {
            _targetRotation = _context.Map.Settings.Flip(direction);
        }
    }

    private void UpdateRotation()
    {
        _context.RotationNode.rotation = Quaternion.RotateTowards(
            _context.RotationNode.rotation, _targetRotation, _rotationSpeed * Time.fixedDeltaTime);
    }
}
