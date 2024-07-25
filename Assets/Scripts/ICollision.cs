using System;
using Unity.VisualScripting;
using UnityEngine;

public interface ICollision
{
    void Depenetrate();
    bool TryGetHit(Vector3 pos, Vector3 dir, float dist, out HitInfo hit);
    bool TryGetHit(out HitInfo hit);
    HitInfo GetHit(Vector3 pos, Vector3 dir, float dist);
    bool CheckDirection(Vector3 direction);
    HitInfo GetHit(Vector3 position);
    HitInfo GetHit();
    Vector3 GetClosestPositionTo(HitInfo hitInfo);
}

public class SphereCollision3D : CollisionCompute3D<SphereCollider>
{
    public SphereCollision3D(SphereCollider collider, Body3D body, ILayerMaskProvider layerMaskProvider) : base(collider, body, layerMaskProvider)
    {
    }

    public override HitInfo GetHit(Vector3 pos, Vector3 dir, float dist)
    {
        Array.Clear(Colliders, 0, Colliders.Length);
        float scaled = Collider.radius * GetScale();
        Physics.SphereCast(pos, scaled, dir, out RaycastHit hit, dist, CollisionMask);

        return hit.ToHitInfo();
    }
    public override Collider[] Overlap(Vector3 pos)
    {
        Array.Clear(Colliders, 0, Colliders.Length);
        float scaled = Collider.radius * GetScale();
        Physics.OverlapSphereNonAlloc(pos, scaled, Colliders, CollisionMask);

        return Colliders;
    }

    public float GetScale()
    {
        return Body.Scale.GetMax();
    }
}

public class BoxCollision3D : CollisionCompute3D<BoxCollider>
{
    public BoxCollision3D(BoxCollider collider, Body3D body, ILayerMaskProvider layerMaskProvider) : base(collider, body, layerMaskProvider)
    {
    }

    public override HitInfo GetHit(Vector3 pos, Vector3 dir, float dist)
    {
        Physics.BoxCast(pos, Collider.bounds.extents, dir, out RaycastHit raycastHit, Body.Rotation, dist, CollisionMask);

        return raycastHit.ToHitInfo();
    }

    public override Collider[] Overlap(Vector3 pos)
    {
        Array.Clear(Colliders, 0, Colliders.Length);
        Physics.OverlapBoxNonAlloc(pos, Collider.bounds.extents, Colliders, Body.Rotation, CollisionMask);

        return Colliders;
    }
}

public class CapsuleCollision3D : CollisionCompute3D<CapsuleCollider>
{
    public CapsuleCollision3D(CapsuleCollider collider, Body3D body, ILayerMaskProvider layerMaskProvider) : base(collider, body, layerMaskProvider)
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
        Array.Clear(Colliders, 0, Colliders.Length);
        (var p1, var p2) = GetCapsulePoints(pos);
        Physics.OverlapCapsuleNonAlloc(p1, p2, Collider.radius, Colliders, CollisionMask);

        return Colliders;
    }

    private (Vector3 point1, Vector3 point2) GetCapsulePoints(Vector3 pos)
    {
        var p1 = pos + Collider.center + Vector3.up * (-Collider.height + Collider.radius*2)* 0.5f ;
        var p2 = p1 + Vector3.up * (Collider.height * Body.Scale.y - Collider.radius*2);

        return (p1, p2);
    }
}

public class BoxCollision2D : CollisionCompute2D<BoxCollider2D>
{
    public BoxCollision2D(BoxCollider2D collider, Body2D body, ILayerMaskProvider layerMaskProvider) : base(collider, body, layerMaskProvider)
    {
    }

    public override HitInfo GetHit(Vector3 pos, Vector3 dir, float dist)
    {
        return Physics2D.BoxCast(pos, Collider.bounds.size, Body.Angle, dir, dist, CollisionMask).ToHitInfo(Collider);
    }
}

public class CapsuleCollision2D : CollisionCompute2D<CapsuleCollider2D>
{
    public CapsuleCollision2D(CapsuleCollider2D collider, Body2D body, ILayerMaskProvider layerMaskProvider) : base(collider, body, layerMaskProvider)
    {
    }

    public override HitInfo GetHit(Vector3 pos, Vector3 dir, float dist)
    {
        var raycastHit = Physics2D.CapsuleCast(pos, Collider.bounds.size, Collider.direction, 0, dir, dist, CollisionMask);

        return raycastHit.ToHitInfo(Collider);
    }
}

public class CircleCollision2D : CollisionCompute2D<CircleCollider2D>
{
    public CircleCollision2D(CircleCollider2D collider, Body2D body, ILayerMaskProvider layerMaskProvider) : base(collider, body, layerMaskProvider)
    {
    }

    public override HitInfo GetHit(Vector3 pos, Vector3 dir, float dist)
    {
        return Physics2D.CircleCast(pos, Collider.radius * GetScale(), dir, dist, CollisionMask).ToHitInfo(Collider);
    }

    private float GetScale()
    {
        return Body.Scale.GetMax();
    }
}
