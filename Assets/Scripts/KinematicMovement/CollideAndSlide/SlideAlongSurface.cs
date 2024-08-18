using PlotvaIzLodzya.Extensions;
using PlotvaIzLodzya.KinematicMovement.CollisionCompute;
using PlotvaIzLodzya.KinematicMovement.StateHandle;
using UnityEngine;

namespace PlotvaIzLodzya.KinematicMovement.CollideAndSlide
{
    public abstract class SlideAlongSurface : ISlide
    {        
        private ICollision _collision;
        private IMovementState _movementState;
        private float _foreseeDistance;

        public SlideAlongSurface(ICollision collision, IMovementState movementState)
        {            
            _collision = collision;
            _movementState = movementState;
            _foreseeDistance = MovementConfig.ContactOffset * 10;
        }

        protected abstract Vector3 GetSurfaceNormal(HitInfo hit, Vector3 direction);

        public Vector3 SlideByMovement_recursive(Vector3 vel, Vector3 desiredDirection, Vector3 currentPos, int currentDepth = 0)
        {
            if (currentDepth >= 5)
                return Vector3.zero;
            (var isTooSteep, var hit, var dir, var surfaceAngle) = GetHorizontalHitData(vel.normalized, currentPos, _foreseeDistance);
            if (isTooSteep)
            {
                (dir, hit, vel) = HandleSteepSlope(hit, dir, vel);

                if (IsSameDirection(dir, desiredDirection) == false)
                {
                    return Vector3.zero;
                }
            }

            (vel, dir, hit.HitNormal) = HandleStep(vel, dir, currentPos, hit, surfaceAngle);

            if (hit.HaveHit)
            {
                var dist = hit.Distance - MovementConfig.ContactOffset;
                dist = Mathf.Clamp(dist, 0, Mathf.Abs(dist));

                (Vector3 velToNextStep, Vector3 projectedleftOverVel, Vector3 nextPos) slideData = GetSlideData2(dir, vel, currentPos, hit.HitNormal, dist);
                if (IsOnTooSteep(slideData.nextPos) && _movementState.OnTooSteepSlope == false)
                {
                    vel = AdjustVelocityToSoroundings(slideData.velToNextStep, currentPos, _foreseeDistance);
                    return vel;
                }

                vel = slideData.velToNextStep + SlideByMovement_recursive(slideData.projectedleftOverVel, desiredDirection, slideData.nextPos, ++currentDepth);

                return vel;
            }
            return vel;
        }

        private bool IsOnTooSteep(Vector3 pos)
        {
            var hit = _collision.GetHit(pos, Vector3.down, MovementConfig.ContactOffset);
            var surfaceNormal = GetSurfaceNormal(hit, Vector3.down);
            float angle = Vector3.Angle(Vector3.up, surfaceNormal);            
            var tooSteep = _movementState.IsSlopeTooSteep(angle);

            return tooSteep;
        }

        public Vector3 SlideByGravity_recursive(Vector3 vel, Vector3 currentPos, int currentDepth = 0)
        {
            if (currentDepth >= 5)
                return Vector3.zero;

            (var isTooSteep, var hit, var dir, var surfaceAngle) = GetHitData(vel.normalized, currentPos, vel.magnitude);

            if (isTooSteep == false)
                currentDepth = 5;

            if (hit.HaveHit)
            {
                var dist = hit.Distance - MovementConfig.ContactOffset;
                (Vector3 velToNextStep, Vector3 projectedleftOverVel, Vector3 nextPos) = GetSlideData2(dir, vel, currentPos, hit.HitNormal, dist);

                vel = velToNextStep + SlideByGravity_recursive(projectedleftOverVel, nextPos, ++currentDepth);

                return vel;
            }

            return vel;
        }

        private (bool isTooSteep, HitInfo hit, Vector3 direction, float surfaceAngle) GetHorizontalHitData(Vector3 dir, Vector3 currentPos, float dist)
        {
            var hit = _collision.GetHit(currentPos, dir, dist);

            var surfaceNormal = GetSurfaceNormal(hit, dir);
            float angle = Vector3.Angle(Vector3.up, surfaceNormal);
            var tooSteep = _movementState.IsSlopeTooSteep(angle) && _movementState.Grounded;

            return (tooSteep, hit, dir, angle);
        }

        private Vector3 AdjustVelocityToSoroundings(Vector3 vel, Vector3 currentPos, float dist)
        {
            var dirZ = Vector3.forward * Mathf.Sign(vel.z);
            (var distZ, var zTooSteep) = CheckDirection(currentPos, dirZ, dist);
            var dirX = Vector3.right * Mathf.Sign(vel.x);
            (var distX, var xTooSteep) = CheckDirection(currentPos, dirX, dist);

            if (zTooSteep)
                vel.z = Mathf.Clamp(vel.z, -100, distZ);
            if (xTooSteep)
                vel.x = Mathf.Clamp(vel.x, -100, distX);

            return vel;
        }

        private (float distance, bool tooSteep) CheckDirection(Vector3 currentPos, Vector3 dir, float checkDist)
        {
            var hit = _collision.GetHit(currentPos, dir, checkDist);
            var dist = _collision.GetDistanceToBounds(hit);
            var tooSteep = IsSurfaceTooSteep(hit, dir);
            return (dist, tooSteep);
        }

        private bool IsSurfaceTooSteep(HitInfo hit, Vector3 dir)
        {
            var surfaceNormal = GetSurfaceNormal(hit, dir);
            float angle = Vector3.Angle(Vector3.up, surfaceNormal);
            var tooSteep = _movementState.IsSlopeTooSteep(angle) && _movementState.Grounded;

            return tooSteep;
        }

        private (bool isTooSteep, HitInfo hit, Vector3 direction, float angle) GetHitData(Vector3 dir, Vector3 currentPos, float dist)
        {
            var hit = _collision.GetHit(currentPos, dir, dist);
            var surfaceNormal = GetSurfaceNormal(hit, dir);
            float angle = Vector3.Angle(Vector3.up, surfaceNormal);
            var tooSteep = _movementState.IsSlopeTooSteep(angle) && _movementState.Grounded;

            return (tooSteep, hit, dir, angle);
        }

        private (Vector3 velToNextStep, Vector3 projectedleftOverVel, Vector3 nextPos) GetSlideData2(Vector3 dir, Vector3 vel, Vector3 currentPos, Vector3 hitNormal, float dist)
        {
            var velToNextStep = dir * dist;
            
            var leftOverVel = vel - velToNextStep;
            var nextPos = currentPos + velToNextStep;

            var projectedleftOverVel = Vector3.ProjectOnPlane(leftOverVel, hitNormal);

            return (velToNextStep, projectedleftOverVel, nextPos);
        }

        private (Vector3 dir, HitInfo hit, Vector3 vel) HandleSteepSlope(HitInfo hit, Vector3 dir, Vector3 vel)
        {
            var surfaceNormal = GetSurfaceNormal(hit, dir);
            var angle = Vector3.Angle(Vector3.up, surfaceNormal);            
            var multiplier = Mathf.Lerp(20f, 1f, angle / 90);
            surfaceNormal = surfaceNormal.GetHorizontal();
            hit.HitNormal = hit.HitNormal.GetHorizontal().normalized;
            hit.Distance = Mathf.Clamp(hit.Distance, MovementConfig.ContactOffset, vel.magnitude + MovementConfig.ContactOffset);            
            if (hit.Distance <= MovementConfig.ContactOffset * multiplier)
            {
                dir = Vector3.ProjectOnPlane(dir.normalized, surfaceNormal).normalized;
            }
            return (dir, hit,vel);
        }

        private (Vector3 velocity, Vector3 direction, Vector3 hitNormal) HandleStep(Vector3 vel, Vector3 dir, Vector3 currentPos, HitInfo hit, float surfaceAngle)
        {
            var lowestPoint = currentPos + Vector3.down;
            var stepHeight = hit.Point.y - lowestPoint.y;
            var maxStepHeight = 0.1f + MovementConfig.ContactOffset;
            if (stepHeight > maxStepHeight && surfaceAngle >= 90)
            {
                vel = vel.GetHorizontal();
                dir = dir.GetHorizontal().normalized;
                hit.HitNormal = hit.HitNormal.GetHorizontal().normalized;
            }

            return (vel, dir, hit.HitNormal);
        }

        private bool IsSameDirection(Vector3 dir, Vector3 desiredDirection)
        {
            var normal = dir.GetHorizontal().normalized;
            var angle = Vector3.Angle(normal, desiredDirection);            
            return angle < 90;

        }
    }
}