using UnityEngine;

public abstract class CollisionCompute : ICollision
{
    protected Transform Transform;

    public CollisionCompute(Transform transform)
    {
        Transform = transform;
    }

    public abstract HitInfo GetHit(Vector3 pos, Vector3 dir, float dist, LayerMask mask);

    public Vector3 GetClosestPositionTo(HitInfo hitInfo)
    {
        var deltaPos = GetDeltaPositionToHit(hitInfo);
        var targetPos = Transform.position - deltaPos;
        return targetPos;
    }

    public Vector3 GetDeltaPositionToHit(HitInfo hit)
    {
        var hitDist = hit.ColliderDistance;
        var deltaPos = hit.Normal * hitDist;
        return deltaPos;
    }

    public virtual HitInfo GetHit(Vector3 position, LayerMask mask)
    {
        return GetHit(Transform.position, Vector3.zero, Movement.ContactOffset,  mask);
    }

    public virtual HitInfo GetHit(LayerMask mask)
    {
        return GetHit(Transform.position, mask);
    }

    public bool TryGetHit(Vector3 pos, Vector3 dir, float dist, LayerMask mask, out HitInfo hit)
    {
        hit = GetHit(pos, dir, dist, mask);
        return hit.HaveHit;
    }
}
