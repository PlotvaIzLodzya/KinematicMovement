using UnityEngine;
using UnityEngine.UIElements;

public interface ICollision
{
    bool TryGetHit(Vector3 pos, Vector3 dir, float dist, LayerMask mask, out HitInfo hit);
    HitInfo GetHit(Vector3 pos, Vector3 dir, float dist, LayerMask mask);
    HitInfo GetHit(LayerMask mask);
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

    public HitInfo(Vector3 point, Vector3 normal, float distance, float colliderDistance, bool haveHit, Transform transform)
    {
        this.point = point;
        this.normal = normal;
        this.distance = distance;
        ColliderDistance = colliderDistance;
        HaveHit = haveHit;
        Transform = transform;
    }
}

public abstract class CollisionCompute: ICollision
{
    protected Transform Transform;

    public CollisionCompute(Transform transform)
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

    public abstract HitInfo GetHit(LayerMask mask);

    public bool TryGetHit(Vector3 pos, Vector3 dir, float dist, LayerMask mask, out HitInfo hit)
    {
        hit = GetHit(pos, dir, dist, mask);
        return hit.HaveHit;
    }
}

public class SphereCollision3D : CollisionCompute
{
    private SphereCollider _collider;
    private Rigidbody _rb;

    public SphereCollision3D(SphereCollider sphereCollider, Transform transform, Rigidbody rb) : base(transform)
    {
        _collider = sphereCollider;
        _rb = rb;
    }

    public override HitInfo GetHit(Vector3 pos, Vector3 dir, float dist, LayerMask mask)
    {
        //_rb.SweepTest(dir, out var hit, dist);
        Physics.SphereCast(pos, _collider.radius, dir, out RaycastHit hit, dist, mask);

        var hitInfo = hit.ToHitInfo();

        return hitInfo;
    }

    public HitInfo GetHit(Vector3 position, LayerMask layerMask)
    {
        var colliders = Physics.OverlapSphere(position, _collider.radius, layerMask);

        var hitInfo = GetHitInfo(colliders, position);
        return hitInfo;
    }

    public override HitInfo GetHit(LayerMask mask)
    {
        return GetHit(Transform.position, mask);
    }

    public HitInfo GetHitInfo(Collider[] colliders, Vector3 position)
    {
        var closestDistance = float.MinValue;

        HitInfo hitInfo = new HitInfo();
        foreach (var collider in colliders)
        {

            Physics.ComputePenetration(collider, collider.transform.position, collider.transform.rotation, _collider, position, Transform.rotation, out Vector3 dir, out float distance);
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

public class CircleCollision2D : CollisionCompute
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

    public override HitInfo GetHit(LayerMask mask)
    {
        return GetHit(Transform.position, Vector3.zero, Movement.ContactOffset, mask);
    }
}
