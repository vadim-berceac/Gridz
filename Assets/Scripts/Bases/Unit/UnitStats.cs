using UnityEngine;

[CreateAssetMenu(fileName = "UnitStats", menuName = "Scriptable Objects/UnitStats")]
public class UnitStats : ScriptableObject
{
    [SerializeField] private float _moveSpeed = 5f;
    [SerializeField] private float _moveRange = 6f;

    public float MoveSpeed => _moveSpeed;
    public float MoveRange => _moveRange;
}
