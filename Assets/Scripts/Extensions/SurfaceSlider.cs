using Unity.Burst;
using UnityEngine;

public class SurfaceSlider
{
    private Vector3 _normal;
    
    public SurfaceSlider()
    {

    }
    
    [BurstCompile]
    public void SetNormal(Collision collision)
    {
        if (collision.contacts.Length < 1)
        {
            _normal = Vector3.zero;
            return;
        }
        _normal = collision.contacts[0].normal;
    }

    [BurstCompile]
    private Vector3 Project(Vector3 forward)
    {
        return forward - Vector3.Dot(forward, _normal) * _normal;
    }
    
    [BurstCompile]
    public Vector3 UpdateDirection(Transform characterTransform, Vector2 inputDirection)
    {
        var newDirection = new Vector3(inputDirection.x, characterTransform.position.y, inputDirection.y);
        if (_normal != Vector3.zero)
        {
            return Project(newDirection.normalized);
        }
        return newDirection;
    }
}
