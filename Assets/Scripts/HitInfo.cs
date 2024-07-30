using UnityEngine;

public struct HitInfo
{
    public Vector3 Point;
    public Vector3 Normal;
    public float Distance;
    public float ColliderDistance;
    public bool HaveHit;
    public Transform Transform;

    public HitInfo(Vector3 point, Vector3 normal, float distance, float colliderDistance, bool haveHit, Transform transform)
    {
        Point = point;
        Normal = normal;
        Distance = distance;
        ColliderDistance = colliderDistance + MovementConfig.ContactOffset;
        HaveHit = haveHit;
        Transform = transform;
    }
}
