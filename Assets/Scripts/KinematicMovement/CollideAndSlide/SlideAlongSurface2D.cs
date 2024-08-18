using PlotvaIzLodzya.KinematicMovement.CollisionCompute;
using PlotvaIzLodzya.KinematicMovement.StateHandle;
using UnityEngine;

namespace PlotvaIzLodzya.KinematicMovement.CollideAndSlide
{
    public class SlideAlongSurface2D : SlideAlongSurface
    {

        //private ICollision _collision;
        //private IMovementState _movementState;

        public SlideAlongSurface2D(ICollision collision, IMovementState movementState) : base(collision, movementState)
        {
        }

        protected override Vector3 GetSurfaceNormal(HitInfo hit, Vector3 direction)
        {
            return hit.HitNormal;
        }

        //public SlideAlongSurface2D(ICollision collision, IMovementState movementState)
        //{
        //    _collision = collision;
        //    _movementState = movementState;
        //}

        //public Vector3 SlideByMovement_recursive(Vector3 vel, Vector3 currentPos, int currentDepth = 0)
        //{
        //    if (currentDepth >= 5)
        //        return Vector3.zero;

        //    (var isTooSteep, var hit, var dir) = GetHitData(vel, currentPos, MovementConfig.ContactOffset * 10);

        //    if (isTooSteep)
        //    {
        //        //hit.Distance = Mathf.Clamp(hit.Distance, MovementConfig.ContactOffset, vel.magnitude + MovementConfig.ContactOffset);
        //        var dist = MovementConfig.ContactOffset;

        //        var surfaceNormal = hit.HitNormal;
        //        var angle = Vector3.Angle(Vector3.up, surfaceNormal);
        //        var multiplier = Mathf.Lerp(20, 1, angle / 90);
        //        surfaceNormal.y = 0f;
        //        hit.HitNormal = surfaceNormal.normalized;
        //        hit.Distance = Mathf.Clamp(hit.Distance, MovementConfig.ContactOffset, vel.magnitude + MovementConfig.ContactOffset);

        //        if (hit.Distance <= dist * multiplier)
        //        {
        //            dir = Vector3.ProjectOnPlane(dir.normalized, surfaceNormal).normalized;
        //        }
        //    }

        //    if (hit.HaveHit)
        //    {
        //        (Vector3 velToNextStep, Vector3 projectedleftOverVel, Vector3 nextPos) slideData = GetSlideData(dir, vel, currentPos, hit.HitNormal, hit.Distance);
        //        //slideData = HandleSlope(slideData, currentPos, dir);
        //        vel = slideData.velToNextStep + SlideByMovement_recursive(slideData.projectedleftOverVel, slideData.nextPos, ++currentDepth);

        //        return vel;
        //    }

        //    return vel;
        //}

        //public Vector3 SlideByGravity_recursive(Vector3 vel, Vector3 currentPos, int currentDepth = 0)
        //{
        //    if (currentDepth >= 5)
        //        return Vector3.zero;

        //    (var isTooSteep, var hit, var dir) = GetHitData(vel, currentPos, vel.magnitude);

        //    if (isTooSteep == false)
        //        currentDepth = 5;

        //    if (hit.HaveHit)
        //    {
        //        (Vector3 velToNextStep, Vector3 projectedleftOverVel, Vector3 nextPos) = GetSlideData(dir, vel, currentPos, hit.HitNormal, hit.Distance);
        //        vel = velToNextStep + SlideByGravity_recursive(projectedleftOverVel, nextPos, ++currentDepth);

        //        return vel;
        //    }

        //    return vel;
        //}

        //private (Vector3 velToNextStep, Vector3 projectedleftOverVel, Vector3 nextPos) GetSlideData(Vector3 dir, Vector3 vel, Vector3 currentPos, Vector3 hitNormal, float hitDist)
        //{
        //    var dist = hitDist - MovementConfig.ContactOffset;
        //    var velToNextStep = dir * dist;
        //    var leftOverVel = vel - velToNextStep;
        //    var nextPos = currentPos + velToNextStep;

        //    var projectedleftOverVel = Vector3.ProjectOnPlane(leftOverVel, hitNormal);
        //    return (velToNextStep, projectedleftOverVel, nextPos);
        //}

        //private (bool isTooSteep, HitInfo hit, Vector3 direction) GetHitData(Vector3 vel, Vector3 currentPos, float dist)
        //{
        //    var dir = vel.normalized;

        //    var hit = _collision.GetHit(currentPos, dir, dist);
        //    var isTooSteep = IsTooSteep(hit.HitNormal);
        //    return (isTooSteep, hit, dir);
        //}

        //private (Vector3 velToNextStep, Vector3 projectedleftOverVel, Vector3 nextPos) HandleSlope((Vector3 velToNextStep, Vector3 projectedleftOverVel, Vector3 pos) slideData, Vector3 currentPos, Vector3 direction)
        //{
        //    var hit = _collision.GetHit(currentPos, direction, MovementConfig.ContactOffset * 4);
        //    var angle = Vector3.Angle(Vector3.up, hit.HitNormal);

        //    if (IsTooSteep(hit.HitNormal) && IsWall(angle) == false)
        //    {
        //        slideData.velToNextStep = Vector3.zero;
        //        slideData.projectedleftOverVel = Vector3.zero;
        //        slideData.pos = currentPos;
        //    }

        //    return slideData;
        //}
        //private bool IsWall(float angle)
        //{
        //    return angle >= 90;
        //}

        //private bool IsTooSteep(Vector3 normal)
        //{
        //    float angle = Vector3.Angle(Vector3.up, normal);

        //    return IsTooSteep(angle);
        //}

        //private bool IsTooSteep(float angle)
        //{
        //    return _movementState.IsSlopeTooSteep(angle) && _movementState.Grounded;
        //}
    }
}