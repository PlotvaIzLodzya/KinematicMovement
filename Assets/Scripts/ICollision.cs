using UnityEditor.PackageManager;
using UnityEngine;

public interface ICollision
{
    bool TryGetHit(Vector3 pos, float radius, Vector3 dir, float dist, LayerMask mask, out HitInfo hit);
    HitInfo GetHit(Vector3 pos, float radius, Vector3 dir, float dist, LayerMask mask);
    Vector3 GetClosestPositionTo(HitInfo hitInfo);
}

public struct HitInfo
{
    public Vector3 point;
    public Vector3 normal;
    public float distance;
    public bool HaveHit;
    public Transform Transform;
}

public abstract class Collision: ICollision
{
    protected Collider2D Collider;
    protected Transform Transform;

    public Collision(Collider2D collider, Transform transform)
    {
        Collider = collider;
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
        var hitDist = hit.distance;
        var deltaPos = hit.normal * hitDist;
        return deltaPos;
    }

    public abstract HitInfo GetHit(Vector3 pos, float radius, Vector3 dir, float dist, LayerMask mask);

    public bool TryGetHit(Vector3 pos, float radius, Vector3 dir, float dist, LayerMask mask, out HitInfo hit)
    {
        hit = GetHit(pos, radius, dir, dist, mask);
        return hit.HaveHit;
    }
}

public class CircleCollision2D : Collision
{
    public CircleCollision2D(Collider2D collider, Transform transform) : base(collider, transform)
    {
    }

    public override HitInfo GetHit(Vector3 pos, float radius, Vector3 dir, float dist, LayerMask mask)
    {
        return Physics2D.CircleCast(pos, radius, dir, dist, mask).ToHitInfo(Collider);
    }
}
