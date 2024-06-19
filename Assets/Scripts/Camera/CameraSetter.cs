using UnityEngine;

public class CameraSetter
{
    private readonly Transform _cameraTarget;
    public CameraSetter(Transform transform)
    {
        _cameraTarget = transform;
    }

    public void SetCameraTarget()
    {
       StrategyCamera.SetTarget(_cameraTarget);
    }

    public void EnableFreeCamera()
    {
        StrategyCamera.SetTarget(null);
    }
}
