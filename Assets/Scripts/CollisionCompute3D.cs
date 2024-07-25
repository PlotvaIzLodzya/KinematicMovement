using System;
using UnityEngine;

public abstract class CollisionCompute3D<T> : CollisionCompute<Body3D> where T : Collider
{
    public const int MaxCollisionCount = 32;

    protected Collider[] Colliders;
    protected T Collider;

    protected CollisionCompute3D(T collider, Body3D body, ILayerMaskProvider layerMaskProvider) : base(body, layerMaskProvider)
    {
        Colliders = new Collider[MaxCollisionCount];
        Collider = collider;
    }

    public abstract Collider[] Overlap(Vector3 pos);

    public override HitInfo GetHit(Vector3 pos)
    {
        var colliders = Overlap(pos);
        var hitInfo = GetClosestHitPosition(colliders, pos);

        return hitInfo;
    }

    public HitInfo GetClosestHitPosition(Collider[] colliders, Vector3 position)
    {
        var closestDistance = float.MinValue;

        HitInfo hitInfo = new HitInfo();

        foreach (var collider in colliders)
        {
            if(collider == null) 
                continue;

            Physics.ComputePenetration(collider, collider.transform.position, collider.transform.rotation, Collider, position, Body.Rotation, out Vector3 dir, out float distance);
            var dist = distance;
            if (dist > closestDistance)
            {
                closestDistance = dist + MovementConfig.ContactOffset;
                var closestPosition = position - dir.normalized * closestDistance;
                hitInfo = new HitInfo(closestPosition, dir, distance, closestDistance, true, collider.transform);
            }
        }

        return hitInfo;
    }
}
