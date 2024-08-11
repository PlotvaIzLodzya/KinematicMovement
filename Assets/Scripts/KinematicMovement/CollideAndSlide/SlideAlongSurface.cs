using PlotvaIzLodzya.KinematicMovement.CollisionCompute;
using PlotvaIzLodzya.KinematicMovement.StateHandle;
using UnityEngine;

namespace PlotvaIzLodzya.KinematicMovement.CollideAndSlide
{
    public abstract class SlideAlongSurface : ISlide
    {        
        private ICollision _collision;
        private IMovementState _movementState;

        public SlideAlongSurface(ICollision collision, IMovementState movementState)
        {
            _collision = collision;
            _movementState = movementState;
        }

        protected abstract Vector3 GetSurfaceNormal(HitInfo hit, Vector3 direction);

        public Vector3 SlideByMovement_recursive(Vector3 vel, Vector3 currentPos, int currentDepth = 0)
        {
            if (currentDepth >= 5)
                return Vector3.zero;

            (var isTooSteep, var hit, var dir) = GetHitData(vel, currentPos, MovementConfig.CollisionCheckDistance * 10);

            if (isTooSteep)
            {
                (dir, hit) = HandleSteepSlope(hit, dir, vel);
            }

            if (hit.HaveHit)
            {
                (Vector3 velToNextStep, Vector3 projectedleftOverVel, Vector3 nextPos) slideData = GetSlideData(dir, vel, currentPos, hit.HitNormal, hit.Distance);
                
                vel = slideData.velToNextStep + SlideByMovement_recursive(slideData.projectedleftOverVel, slideData.nextPos, ++currentDepth);

                return vel;
            }

            return vel;
        }

        public Vector3 SlideByGravity_recursive(Vector3 vel, Vector3 currentPos, int currentDepth = 0)
        {
            if (currentDepth >= 5)
                return Vector3.zero;

            (var isTooSteep, var hit, var dir) = GetHitData(vel, currentPos, vel.magnitude);

            if (isTooSteep == false)
                currentDepth = 5;

            if (hit.HaveHit)
            {
                (Vector3 velToNextStep, Vector3 projectedleftOverVel, Vector3 nextPos) = GetSlideData(dir, vel, currentPos, hit.HitNormal, hit.Distance);
                vel = velToNextStep + SlideByGravity_recursive(projectedleftOverVel, nextPos, ++currentDepth);

                return vel;
            }

            return vel;
        }

        private (bool isTooSteep, HitInfo hit, Vector3 direction) GetHitData(Vector3 vel, Vector3 currentPos, float dist)
        {
            var dir = vel.normalized;

            var hit = _collision.GetHit(currentPos, dir, dist);
            float angle = Vector3.Angle(Vector3.up, hit.HitNormal);
            var tooSteep = _movementState.IsSlopeTooSteep(angle) && _movementState.Grounded;

            return (tooSteep, hit, dir);
        }

        private (Vector3 velToNextStep, Vector3 projectedleftOverVel, Vector3 nextPos) GetSlideData(Vector3 dir, Vector3 vel, Vector3 currentPos, Vector3 hitNormal, float hitDist)
        {
            var dist = hitDist - MovementConfig.ContactOffset;
            var velToNextStep = dir * dist;
            var leftOverVel = vel - velToNextStep;
            var nextPos = currentPos + velToNextStep;

            var projectedleftOverVel = Vector3.ProjectOnPlane(leftOverVel, hitNormal);

            return (velToNextStep, projectedleftOverVel, nextPos);
        }

        private (Vector3 dir, HitInfo hit) HandleSteepSlope(HitInfo hit, Vector3 dir, Vector3 vel)
        {
            var surfaceNormal = GetSurfaceNormal(hit, dir);
            var angle = Vector3.Angle(Vector3.up, surfaceNormal);
            var multiplier = Mathf.Lerp(20, 1, angle / 90);
            surfaceNormal.y = 0f;
            hit.HitNormal = surfaceNormal.normalized;
            hit.Distance = Mathf.Clamp(hit.Distance, MovementConfig.ContactOffset, vel.magnitude + MovementConfig.ContactOffset);

            if (hit.Distance <= MovementConfig.ContactOffset * multiplier)
            {
                dir = Vector3.ProjectOnPlane(dir.normalized, surfaceNormal).normalized;
            }

            return (dir, hit);
        }        
    }
}