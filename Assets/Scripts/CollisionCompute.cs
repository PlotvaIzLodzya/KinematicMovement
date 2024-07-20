using UnityEngine;

public abstract class CollisionCompute : ICollision
{
    protected Transform Transform;
    private ILayerMaskProvider _layerMaskProvider;

    public LayerMask CollisionMask => _layerMaskProvider.GroundMask;

    public CollisionCompute(Transform transform, ILayerMaskProvider layerMaskProvider)
    {
        Transform = transform;
        _layerMaskProvider = layerMaskProvider;
    }

    public abstract HitInfo GetHit(Vector3 pos, Vector3 dir, float dist);

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

    public virtual HitInfo GetHit(Vector3 position)
    {
        return GetHit(Transform.position, Vector3.zero, MovementConfig.ContactOffset);
    }

    public virtual HitInfo GetHit()
    {
        return GetHit(Transform.position);
    }

    public bool TryGetHit(Vector3 pos, Vector3 dir, float dist, out HitInfo hit)
    {
        hit = GetHit(pos, dir, dist);
        return hit.HaveHit;
    }
}
