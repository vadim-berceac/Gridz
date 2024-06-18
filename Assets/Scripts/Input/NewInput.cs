using UnityEngine;
using UnityEngine.InputSystem;

public class NewInput
{
    public class FrameInfo
    {
        public int Frame;
        public GameObject OverObject;
        public Vector3 CameraGroundPosition;
    }

    private static float _enter;
    private static RaycastHit _hit;
    private static Ray _screemCenterRay;
    private static Ray _mouseRay;
    private static Vector2 _mousePosition;
    private static readonly FrameInfo _lastFrame = new();

    static void Validate(Plane plane)
    {
        if (_lastFrame.Frame != Time.frameCount)
        {
            _lastFrame.Frame = Time.frameCount;
            _mousePosition = Mouse.current.position.ReadValue();
            if (Physics.Raycast(Camera.main.ScreenPointToRay(_mousePosition), out _hit, 100f))
            {
                _lastFrame.OverObject = _hit.collider.gameObject;
            }
            else
            {
                _lastFrame.OverObject = null;
            }
            _screemCenterRay = Camera.main.ScreenPointToRay(new Vector3(Screen.width / 2f, Screen.height / 2f, 0f));
            _enter = 0f;
            if (plane.Raycast(_screemCenterRay, out _enter))
            {
                _lastFrame.CameraGroundPosition = _screemCenterRay.GetPoint(_enter);
            }
            else
            {
                _lastFrame.CameraGroundPosition = Vector3.zero;
            }
        }
    }

    public static bool GetOnWorldDownFree(Plane plane)
    {
        Validate(plane);
        return Mouse.current.leftButton.wasPressedThisFrame;
    }

    public static bool GetOnWorldUpFree(Plane plane)
    {
        Validate(plane);
        return Mouse.current.leftButton.wasReleasedThisFrame;
    }

    public static bool GetOnWorldUp(Plane plane)
    {
        Validate(plane);
        return GetOnWorldUpFree(plane) && !StrategyCamera.IsMovingByPlayer;
    }

    public static bool GetOnWorldFree(Plane plane)
    {
        Validate(plane);
        return Input.GetMouseButton(0);
    }

    public static Vector3 CameraGroundPosition(Plane plane)
    {
        Validate(plane);
        return _lastFrame.CameraGroundPosition;
    }

    public static Vector3 GroundPosition(Plane plane)
    {
        _mouseRay = Camera.main.ScreenPointToRay(Input.mousePosition);
        _enter = 0f;
        if (plane.Raycast(_mouseRay, out _enter))
        {
            return _mouseRay.GetPoint(_enter);
        }
        return Vector3.zero;
    }

    public static Vector3 GroundPositionCameraOffset(Plane plane)
    {
        _mouseRay = Camera.main.ScreenPointToRay(Input.mousePosition); 
        _enter = 0f;
        if (plane.Raycast(_mouseRay, out _enter))
        {
            return _mouseRay.GetPoint(_enter) - Camera.main.transform.position;
        }
        return Vector3.zero;
    }
}
