using UnityEngine;

public abstract class CollisionCompute3D<T> : CollisionCompute where T : Collider
{
    protected T Collider;

    protected CollisionCompute3D(T collider, Transform transform) : base(transform)
    {
        Collider = collider;
    }

    public HitInfo GetHitInfo(Collider[] colliders, Vector3 position)
    {
        var closestDistance = float.MinValue;

        HitInfo hitInfo = new HitInfo();
        foreach (var collider in colliders)
        {

            Physics.ComputePenetration(collider, collider.transform.position, collider.transform.rotation, Collider, position, Transform.rotation, out Vector3 dir, out float distance);
            var dist = distance;
            if (dist > closestDistance)
            {
                closestDistance = dist + Movement.ContactOffset;
                var closestPosition = position - dir * closestDistance;
                hitInfo = new HitInfo(closestPosition, dir, distance, closestDistance, true, collider.transform);
            }
        }

        return hitInfo;
    }
}
