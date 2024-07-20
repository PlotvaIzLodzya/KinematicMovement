using UnityEngine;

public interface ICollision
{
    bool TryGetHit(Vector3 pos, Vector3 dir, float dist, out HitInfo hit);
    HitInfo GetHit(Vector3 pos, Vector3 dir, float dist);
    HitInfo GetHit(Vector3 position);
    HitInfo GetHit();
    Vector3 GetClosestPositionTo(HitInfo hitInfo);
}

public class SphereCollision3D : CollisionCompute3D<SphereCollider>
{
    public SphereCollision3D(SphereCollider collider, Transform transform, ILayerMaskProvider layerMaskProvider) : base(collider, transform, layerMaskProvider)
    {
    }

    public override HitInfo GetHit(Vector3 pos, Vector3 dir, float dist)
    {
        Physics.SphereCast(pos, Collider.radius, dir, out RaycastHit hit, dist, CollisionMask);

        var hitInfo = hit.ToHitInfo();

        return hitInfo;
    }
    public override Collider[] Overlap(Vector3 pos)
    {
        var colliders = Physics.OverlapSphere(pos, Collider.radius, CollisionMask);
        return colliders;
    }
}

public class BoxCollision3D : CollisionCompute3D<BoxCollider>
{
    public BoxCollision3D(BoxCollider collider, Transform transform, ILayerMaskProvider layerMaskProvider) : base(collider, transform, layerMaskProvider)
    {
    }

    public override HitInfo GetHit(Vector3 pos, Vector3 dir, float dist)
    {
        Physics.BoxCast(pos, Collider.bounds.extents, dir, out RaycastHit raycastHit, Transform.localRotation, dist, CollisionMask);
        return raycastHit.ToHitInfo();
    }

    public override Collider[] Overlap(Vector3 pos)
    {
        var colliders = Physics.OverlapBox(pos, Collider.bounds.extents, Transform.rotation, CollisionMask);
        return colliders;
    }
}

public class CapsuleCollision3D : CollisionCompute3D<CapsuleCollider>
{
    public CapsuleCollision3D(CapsuleCollider collider, Transform transform, ILayerMaskProvider layerMaskProvider) : base(collider, transform, layerMaskProvider)
    {
    }

    public override HitInfo GetHit(Vector3 pos, Vector3 dir, float dist)
    {
        (var p1, var p2) = GetCapsulePoints(pos);
        Physics.CapsuleCast(p1, p2, Collider.radius, dir, out RaycastHit raycastHit, dist, CollisionMask);
        return raycastHit.ToHitInfo();
    }

    public override Collider[] Overlap(Vector3 pos)
    {
        (var p1, var p2) = GetCapsulePoints(pos);
        var colliders = Physics.OverlapCapsule(p1, p2, Collider.radius, CollisionMask);
        return colliders;
    }

    private (Vector3 point1, Vector3 point2) GetCapsulePoints(Vector3 pos)
    {
        var p1 = pos + Collider.center + Vector3.up * -Collider.height * 0.5f;
        var p2 = p1 + Vector3.up * Collider.height;
        return (p1, p2);
    }
}

public class BoxCollision2D : CollisionCompute2D<BoxCollider2D>
{
    public BoxCollision2D(BoxCollider2D collider, Transform transform, ILayerMaskProvider layerMaskProvider) : base(collider, transform, layerMaskProvider)
    {
    }

    public override HitInfo GetHit(Vector3 pos, Vector3 dir, float dist)
    {
        return Physics2D.BoxCast(pos, Collider.size, 0, dir, dist, CollisionMask).ToHitInfo(Collider);
    }
}

public class CapsuleCollision2D : CollisionCompute2D<CapsuleCollider2D>
{
    public CapsuleCollision2D(CapsuleCollider2D collider, Transform transform, ILayerMaskProvider layerMaskProvider) : base(collider, transform, layerMaskProvider)
    {
    }

    public override HitInfo GetHit(Vector3 pos, Vector3 dir, float dist)
    {
        var raycastHit = Physics2D.CapsuleCast(pos, Collider.size, Collider.direction, 0, dir, dist, CollisionMask);
        return raycastHit.ToHitInfo(Collider);
    }
}

public class CircleCollision2D : CollisionCompute2D<CircleCollider2D>
{
    public CircleCollision2D(CircleCollider2D collider, Transform transform, ILayerMaskProvider layerMaskProvider) : base(collider, transform, layerMaskProvider)
    {
    }

    public override HitInfo GetHit(Vector3 pos, Vector3 dir, float dist)
    {
        return Physics2D.CircleCast(pos, Collider.radius, dir, dist, CollisionMask).ToHitInfo(Collider);
    }
}
