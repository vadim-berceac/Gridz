using UnityEngine;

public class PhysicsBox : MonoBehaviour
{
   [field: Header("Joint")]
   [SerializeField] private ConfigurableJoint joint;
   [field: Header("Visual")]
   [field: SerializeField] public MeshRenderer MeshRenderer {get; private set;}
   [field: SerializeField] public bool Hide {get; private set;} = false;

   private Vector3 _scaleDown;
   private Vector3 _scaleUp;
   private float _scaleCoefficient;
   private float _rotationCoefficient;
   private Transform _player;
   private Transform _playerModel;
   private Transform _box;
   private Vector3 _relativePosition;
   private Vector3 _scale;
   private float _interpolation;

   private void Awake()
   {
      _box = transform;

      if (Hide)
      {
         MeshRenderer.enabled = false;
      }
   }
   
   public void Attach(Rigidbody rb, Transform playerModel, Vector3 scaleDown, Vector3 scaleUp, float scaleCoefficient, float rotationCoefficient)
   {
      joint.connectedBody = rb;
      _player = rb.transform;
      _playerModel = playerModel;
      
      _scaleDown = scaleDown;
      _scale = scaleUp;
      _scaleCoefficient = scaleCoefficient;
      _rotationCoefficient = rotationCoefficient;
   }

   private void FixedUpdate()
   {
      ScaleBody();
      RotateBody();
   }

   private void ScaleBody()
   {
      _relativePosition = _player.InverseTransformPoint(_box.position);
      _interpolation = _relativePosition.y * _scaleCoefficient;
      _scale = Lerp3(_scaleDown, Vector3.one, _scaleUp, _interpolation);
      _playerModel.localScale = _scale;
   }

   private void RotateBody()
   {
      _playerModel.localEulerAngles = new Vector3(_relativePosition.z, 0, -_relativePosition. x) * _rotationCoefficient;
   }

   private static Vector3 Lerp3Unclamped(Vector3 a, Vector3 b, Vector3 c, float t)
   {
      if (t < 0)
      {
         return Vector3.LerpUnclamped(a, b, t + 1f);
      }
      return Vector3.LerpUnclamped(b, c, t);
   }
   
   private static Vector3 Lerp3(Vector3 a, Vector3 b, Vector3 c, float t)
   {
      if (t < 0)
      {
         return Vector3.Lerp(a, b, t + 1f);
      }
      return Vector3.Lerp(b, c, t);
   }
}
