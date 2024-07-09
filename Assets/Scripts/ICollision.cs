using System.Collections.Generic;
using UnityEditor.PackageManager;
using UnityEngine;
using static UnityEditor.PlayerSettings;
using UnityEngine.UI;

public interface ICollision
{
    bool TryGetHit(Vector3 pos, Vector3 dir, float dist, LayerMask mask, out HitInfo hit);
    HitInfo GetHit(Vector3 pos, Vector3 dir, float dist, LayerMask mask);
    Vector3 GetClosestPositionTo(HitInfo hitInfo);
}

public struct HitInfo
{
    public Vector3 point;
    public Vector3 normal;
    public float distance;
    public float ColliderDistance;
    public bool HaveHit;
    public Transform Transform;
}

public abstract class Collision: ICollision
{
    protected Transform Transform;

    public Collision(Transform transform)
    {
        Transform = transform;
    }

    public Vector3 GetClosestPositionTo(HitInfo hitInfo)
    {
        var deltaPos = GetDeltaPositionToHit(hitInfo);
        var targetPos = Transform.position - deltaPos;
        return targetPos;
    }

    public Vector3 GetDeltaPositionToHit(HitInfo hit)
    {
        var hitDist = hit.ColliderDistance;
        var deltaPos = hit.normal * hitDist;
        return deltaPos;
    }

    public abstract HitInfo GetHit(Vector3 pos, Vector3 dir, float dist, LayerMask mask);

    public bool TryGetHit(Vector3 pos, Vector3 dir, float dist, LayerMask mask, out HitInfo hit)
    {
        hit = GetHit(pos, dir, dist, mask);
        return hit.HaveHit;
    }
}

public class SphereCollision3D : Collision
{
    private SphereCollider _collider;

    public SphereCollision3D(SphereCollider sphereCollider, Transform transform) : base(transform)
    {
        _collider = sphereCollider;
    }

    public override HitInfo GetHit(Vector3 pos, Vector3 dir, float dist, LayerMask mask)
    {
        Physics.SphereCast(pos, _collider.radius, dir, out RaycastHit hit, dist, mask);
        return hit.ToHitInfo();
    }
}

public class CircleCollision2D : Collision
{
    private CircleCollider2D _collider;

    public CircleCollision2D(CircleCollider2D collider, Transform transform) : base( transform)
    {
        _collider = collider;
    }

    public override HitInfo GetHit(Vector3 pos, Vector3 dir, float dist, LayerMask mask)
    {
        return Physics2D.CircleCast(pos, _collider.radius, dir, dist, mask).ToHitInfo(_collider);
    }
}
