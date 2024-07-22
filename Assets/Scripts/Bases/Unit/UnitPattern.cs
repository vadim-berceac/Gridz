using UnityEngine;

[CreateAssetMenu(fileName = "UnitPattern", menuName = "Scriptable Objects/UnitPattern")]
public class UnitPattern : ScriptableObject
{
    [SerializeField] private GameObject _prefab;
    [SerializeField] private string _name = "";
    [SerializeField] private Vector3 _modelScale = new(1, 1, 1);
    [SerializeField] private float _maxHealthValue = 20f;
    [SerializeField] private float _moveSpeed = 5f;
    [SerializeField] private float _moveRange = 6f;
    [SerializeField] private int _attackRadius = 1; // count of TileEntities in PathList

    public GameObject Prefab => _prefab;
    public string Name => _name;
    public Vector3 ModelScale => _modelScale;
    public float MoveSpeed => _moveSpeed;
    public float MoveRange => _moveRange;
    public float MaxHealthValue => _maxHealthValue;
    public int AttackRadius => _attackRadius;
}
