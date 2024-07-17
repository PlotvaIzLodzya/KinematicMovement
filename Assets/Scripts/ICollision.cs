using Unity.VisualScripting.FullSerializer;
using UnityEngine;
using UnityEngine.UIElements;
using static UnityEditor.PlayerSettings;

public interface ICollision
{
    bool TryGetHit(Vector3 pos, Vector3 dir, float dist, LayerMask mask, out HitInfo hit);
    HitInfo GetHit(Vector3 pos, Vector3 dir, float dist, LayerMask mask);
    HitInfo GetHit(Vector3 position, LayerMask mask);
    HitInfo GetHit(LayerMask mask);
    Vector3 GetClosestPositionTo(HitInfo hitInfo);
}

public class SphereCollision3D : CollisionCompute3D<SphereCollider>
{
    public SphereCollision3D(SphereCollider collider, Transform transform) : base(collider, transform)
    {
    }

    public override HitInfo GetHit(Vector3 pos, Vector3 dir, float dist, LayerMask mask)
    {
        Physics.SphereCast(pos, Collider.radius, dir, out RaycastHit hit, dist, mask);

        var hitInfo = hit.ToHitInfo();

        return hitInfo;
    }

    public override HitInfo GetHit(Vector3 position, LayerMask layerMask)
    {
        var colliders = Physics.OverlapSphere(position, Collider.radius, layerMask);

        var hitInfo = GetHitInfo(colliders, position);
        return hitInfo;
    }
}

public class BoxCollision3D : CollisionCompute3D<BoxCollider>
{
    public BoxCollision3D(BoxCollider collider, Transform transform) : base(collider, transform)
    {
    }

    public override HitInfo GetHit(Vector3 pos, Vector3 dir, float dist, LayerMask mask)
    {
        Physics.BoxCast(pos, Collider.bounds.extents, dir, out RaycastHit raycastHit, Transform.localRotation, dist, mask);
        return raycastHit.ToHitInfo();
    }

    public override HitInfo GetHit(Vector3 position, LayerMask layerMask)
    {
        var colliders = Physics.OverlapBox(position, Collider.bounds.extents, Transform.rotation, layerMask);

        var hitInfo = GetHitInfo(colliders, position);
        return hitInfo;
    }
}

public class CapsuleCollision3D : CollisionCompute3D<CapsuleCollider>
{
    public CapsuleCollision3D(CapsuleCollider collider, Transform transform) : base(collider, transform)
    {
    }

    public override HitInfo GetHit(Vector3 pos, Vector3 dir, float dist, LayerMask mask)
    {
        (var p1, var p2) = GetCapsulePoints(pos);
        Physics.CapsuleCast(p1, p2, Collider.radius, dir, out RaycastHit raycastHit, dist, mask);
        return raycastHit.ToHitInfo();
    }

    public override HitInfo GetHit(Vector3 pos, LayerMask layerMask)
    {
        (var p1, var p2) = GetCapsulePoints(pos);
        var colliders = Physics.OverlapCapsule(p1, p2, Collider.radius, layerMask);

        var hitInfo = GetHitInfo(colliders, pos);
        return hitInfo;
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
    public BoxCollision2D(BoxCollider2D collider, Transform transform) : base(collider, transform)
    {
    }

    public override HitInfo GetHit(Vector3 pos, Vector3 dir, float dist, LayerMask mask)
    {
        return Physics2D.BoxCast(pos, Collider.size, 0, dir, dist, mask).ToHitInfo(Collider);
    }
}

public class CapsuleCollision2D : CollisionCompute2D<CapsuleCollider2D>
{
    public CapsuleCollision2D(CapsuleCollider2D collider, Transform transform) : base(collider, transform)
    {
    }

    public override HitInfo GetHit(Vector3 pos, Vector3 dir, float dist, LayerMask mask)
    {
        var raycastHit = Physics2D.CapsuleCast(pos, Collider.size, Collider.direction, 0, dir, dist, mask);
        return raycastHit.ToHitInfo(Collider);
    }
}

public class CircleCollision2D : CollisionCompute2D<CircleCollider2D>
{
    public CircleCollision2D(CircleCollider2D collider, Transform transform) : base(collider, transform)
    {
    }

    public override HitInfo GetHit(Vector3 pos, Vector3 dir, float dist, LayerMask mask)
    {
        return Physics2D.CircleCast(pos, Collider.radius, dir, dist, mask).ToHitInfo(Collider);
    }
}
