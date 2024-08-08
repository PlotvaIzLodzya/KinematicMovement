using PlotvaIzLodzya.KinematicMovement.CollisionCompute;
using PlotvaIzLodzya.KinematicMovement.StateHandle;
using UnityEngine;

namespace PlotvaIzLodzya.KinematicMovement.CollideAndSlide
{
    public interface ISlide
    {
        Vector3 SlideByMovement_recursive(Vector3 vel, Vector3 currentPos, int currentDepth = 0);
        public Vector3 SlideByGravity_recursive(Vector3 vel, Vector3 currentPos, int currentDepth = 0);
    }

    public class SlideAlongSurface:ISlide
    {
        private ICollision _collision;
        private IMovementState _movementState;

        public SlideAlongSurface(ICollision collision, IMovementState movementState)
        {
            _collision = collision;
            _movementState = movementState;
        }

        public Vector3 SlideByMovement_recursive(Vector3 vel, Vector3 currentPos, int currentDepth = 0)
        {
            if (currentDepth >= 5)
                return Vector3.zero;
            
            (var isTooSteep, var hit, var dir) = GetHitData(vel, currentPos);

            if (isTooSteep)
            {
                var dis = Mathf.Clamp(hit.ColliderDistance, MovementConfig.CollisionCheckDistance, hit.ColliderDistance);
                vel = dir.normalized * dis;
            }

            if (hit.HaveHit)
            {
                (Vector3 velToNextStep, Vector3 projectedleftOverVel, Vector3 nextPos) = GetSlideData(dir, vel, currentPos, hit.Normal, hit.Distance);

                vel = velToNextStep + SlideByMovement_recursive(projectedleftOverVel, nextPos, ++currentDepth);

                return vel;
            }

            return vel;
        }

        private (bool isTooSteep, HitInfo hit, Vector3 direction) GetHitData(Vector3 vel, Vector3 currentPos)
        {
            float dist = Mathf.Clamp(vel.magnitude, MovementConfig.ContactOffset, vel.magnitude);

            var dir = vel.normalized;

            var hit = _collision.GetHit(currentPos, dir, MovementConfig.ContactOffset*10);
            float angle = Vector3.Angle(Vector3.up, hit.Normal);
            
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

        private (Vector3 velToNextStep, Vector3 projectedleftOverVel, Vector3 nextPos) GetSlideData2(Vector3 dir, Vector3 vel, Vector3 currentPos, Vector3 hitNormal, float hitDist)
        {
            var dist = hitDist - MovementConfig.ContactOffset;
            //Debug.Log(hitDist);
            var velToNextStep = dir * dist;
            var leftOverVel = vel - velToNextStep;
            var nextPos = currentPos + velToNextStep;

            var projectedleftOverVel = Vector3.ProjectOnPlane(leftOverVel, hitNormal);
            var hit = _collision.GetHit(currentPos, dir, MovementConfig.ContactOffset*2);
            float angle = Vector3.Angle(Vector3.up, hit.Normal);
            if (_movementState.IsSlopeTooSteep(angle))
            {
                velToNextStep = Vector3.zero;
                projectedleftOverVel = Vector3.zero;
                nextPos = currentPos;
            }

            return (velToNextStep, projectedleftOverVel, nextPos);
        }

        public Vector3 SlideByGravity_recursive(Vector3 vel, Vector3 currentPos, int currentDepth = 0)
        {
            if (currentDepth >= 5)
                return Vector3.zero;

            (var isTooSteep, var hit, var dir) = GetHitData(vel, currentPos);

            if (isTooSteep == false)
                currentDepth = 5;

            if (hit.HaveHit)
            {
                (Vector3 velToNextStep, Vector3 projectedleftOverVel, Vector3 nextPos) = GetSlideData(dir, vel, currentPos, hit.Normal, hit.Distance);
                vel = velToNextStep + SlideByGravity_recursive(projectedleftOverVel, nextPos, ++currentDepth);

                return vel;
            }

            return vel;
        }
    }
}