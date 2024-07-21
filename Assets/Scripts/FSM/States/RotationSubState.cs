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
        IsComplete = false;
        _targetRotation = CalcRotate(_context.DirectionOfView);
    }

    public override void UpdateState()
    {        
        base.UpdateState();
        UpdateRotation();
        IsComplete = IsFacingTarget(_context.RotationNode.rotation, _targetRotation);
    }

    private Quaternion CalcRotate(Vector3 direction)
    {        
        if (_context.Map.RotationType == RotationType.LookAt)
        {
            return Quaternion.LookRotation(direction, Vector3.up);
        }
        else
        {
            return _context.Map.Settings.Flip(direction);
        }
    }

    private void UpdateRotation()
    {
        _context.RotationNode.rotation = Quaternion.RotateTowards(
            _context.RotationNode.rotation, _targetRotation, _rotationSpeed * Time.fixedDeltaTime);
    }

    private bool IsFacingTarget(Quaternion currentRotation, Quaternion targetRotation, float angleThreshold = 0.05f)
    {
        float angle = Quaternion.Angle(currentRotation, targetRotation);
        return angle <= angleThreshold;
    }
}
