using System;
using Unity.Burst;
using Unity.Cinemachine;
using UnityEngine;
using Zenject;

public class CameraSystem : MonoBehaviour
{
    [field:SerializeField] public GameObject MainCamera {get; private set;}
    [field:SerializeField] public CinemachineCamera SceneCamera {get; private set;}
    [field:SerializeField] public CinemachineCamera FollowCamera {get; private set;}
    
    [Header("Follow Camera Settings")]
    [SerializeField] private float topClamp = 70.0f;
    [SerializeField] private float bottomClamp = -30.0f;
    [SerializeField] private float rotationCoefficient = 1f;

    private static LocoMotionLayer _selectedCharacter;
    public static event Action<LocoMotionLayer> SelectedCharacterChanged;
    
    private PlayerInput _gameInput;
    private Vector2 _lookInput;
    private const float Threshold = 0.01f;
    private float _targetYaw;
    private float _targetPitch;
    
    [Inject]
    private void Construct(PlayerInput playerInput)
    {
        _gameInput = playerInput;
    }

    public static LocoMotionLayer GetSelectedCharacter()
    {
        return _selectedCharacter;
    }

    public void Select(LocoMotionLayer locoMotionLayer)
    {
        if (locoMotionLayer == null)
        {
            _selectedCharacter = null;
            SelectedCharacterChanged?.Invoke(null);
            SetTarget(null);
            return;
        }
        var target = locoMotionLayer.transform.FindObjectsWithTag(TagsAndLayersConst.CameraTargetTag)[0];
        if (target == null)
        {
            _selectedCharacter = null;
            SelectedCharacterChanged?.Invoke(null);
            SetTarget(null);
            return;
        }
        _selectedCharacter = locoMotionLayer;
        SelectedCharacterChanged?.Invoke(locoMotionLayer);
        SetTarget(target.transform);
    }

    private void SetTarget(Transform target)
    {
        FollowCamera.Target.TrackingTarget = target;
        FollowCamera.gameObject.SetActive(true);
        SceneCamera.gameObject.SetActive(false);
    }

    private void Update()
    {
        UpdateLookInput();
    }

    private void LateUpdate()
    {
        CameraRotation();
    }

    [BurstCompile]
    private void UpdateLookInput()
    {
        _lookInput = _gameInput.GetLookDirection();
    }

    [BurstCompile]
    private void CameraRotation()
    {
        if (FollowCamera.Target.TrackingTarget == null)
        {
            FollowCamera.gameObject.SetActive(false);
            SceneCamera.gameObject.SetActive(true);
            return;
        }
        
        if (_lookInput.sqrMagnitude >= Threshold)
        {
            _targetYaw += _lookInput.x * rotationCoefficient;
            _targetPitch += _lookInput.y * rotationCoefficient;
        }
        _targetYaw = _targetYaw.ClampAngle(float.MinValue, float.MaxValue);
        _targetPitch = _targetPitch.ClampAngle(bottomClamp, topClamp);
        FollowCamera.Target.TrackingTarget.transform.rotation 
            = Quaternion.Euler(_targetPitch, _targetYaw, 0.0f);
    }
    
    public float GetCameraYaw()
    {
        return MainCamera.transform.eulerAngles.y;
    }
}
