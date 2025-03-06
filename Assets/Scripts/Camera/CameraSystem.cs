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
    [SerializeField] private float rotationKoef = 1f;
    
    [field:Header("Player")]
    [field:SerializeField] public LocoMotion SelectedCharacter {get; private set;}
    
    private PlayerInput _gameInput;
    public Vector2 LookInput {get; private set;}
    private const float Threshold = 0.01f;
    private float _targetYaw;
    private float _targetPitch;
    
    [Inject]
    private void Construct(PlayerInput playerInput)
    {
        _gameInput = playerInput;
    }

    public void Select(LocoMotion locoMotion)
    {
        if (locoMotion == null)
        {
            SelectedCharacter = null;
            SetTarget(null);
            return;
        }
        var target = locoMotion.transform.FindObjectsWithTag(TagsAndLayersConst.CameraTargetTag)[0];
        if (target == null)
        {
            SelectedCharacter = null;
            SetTarget(null);
            return;
        }
        SelectedCharacter = locoMotion;
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
        LookInput = _gameInput.GetLookDirection();
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
        
        if (LookInput.sqrMagnitude >= Threshold)
        {
            _targetYaw += LookInput.x * rotationKoef;
            _targetPitch += LookInput.y * rotationKoef;
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
