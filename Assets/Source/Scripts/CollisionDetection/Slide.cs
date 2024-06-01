using PlotvaIzLodzya.Player.Movement.CollideAndSlide.CollisionDetection;
using UnityEngine;

namespace PlotvaIzLodzya.Player.Movement.CollideAndSlide
{
    public class Slide
    {
        private ICollisionHandler _collisionHandler;
        private int _collideDepth;
        private MovementConfig _movementConfig;
        private MovementState _movementState;

        public Slide(ICollisionHandler collisionHandler, int collideDepth, MovementConfig movementConfig, MovementState movementState)
        {
            _collisionHandler = collisionHandler;
            _collideDepth = collideDepth;
            _movementConfig = movementConfig;
            _movementState = movementState;
        }

        public Vector3 CollideAndSlide_recursive(Vector3 vel, Vector3 currentPos, int currentDepth = 0)
        {
            if (currentDepth >= _collideDepth)
                return Vector3.zero;

            float dist = vel.magnitude + CollisionConfig.ClipPreventingValue;

            var dir = vel.normalized;

            if (_collisionHandler.IsCollide(currentPos, dir, out HitInfo hit, dist))
            { 
                var velToNextStep = dir * hit.Distance;
                var leftOverVel = vel - velToNextStep;
                var nextPos = currentPos + velToNextStep;

                float angle = Vector3.Angle(Vector3.up, hit.Normal);
                var projectedleftOverVel = Vector3.ProjectOnPlane(leftOverVel, hit.Normal);

                projectedleftOverVel = HandleSlope(angle, vel, projectedleftOverVel, hit.Normal);

                vel = velToNextStep + CollideAndSlide_recursive(projectedleftOverVel, nextPos, ++currentDepth);
                return vel;
            }

            return vel;
        }

        public bool IsSlopeTooSteep(float angle)
        {
            return angle >= _movementConfig.MaxSlopeAngle;
        }

        public Vector3 HandleWall(Vector3 vel)
        {
            var angle = GetSurfaceAngle(vel.normalized);

            //Debug.Log($"asdss {_movementState.DirectionCollision.IsInState} {angle}");
            if (_movementState.DirectionCollision.IsEnterState && IsSlopeTooSteep(angle))
            {
                vel = Vector3.zero;
            }

            return vel;
        }

        private Vector3 HandleSlope(float slopeAngle, Vector3 vel, Vector3 projectedleftOverVel, Vector3 surfaceNormal)
        {
            if (_movementState.Grounded.IsInState == false || _movementState.JumpStatus.Ended == false)
            {
                return projectedleftOverVel;
            }

            if (IsSlopeTooSteep(slopeAngle))
            {
                projectedleftOverVel = ScaleHorizontalVelocity(vel, projectedleftOverVel, surfaceNormal);
            }
            else
            {
                vel.y = projectedleftOverVel.y;
                projectedleftOverVel = vel.normalized * projectedleftOverVel.magnitude;
            }

            return projectedleftOverVel;
        }

        public Vector3 ProjectVelocityOnSurface(Vector3 vel, Vector3 normal)
        {
            vel.y = 0;
            vel = Vector3.ProjectOnPlane(vel, normal);
            
            return vel;
        }


        public Vector3 AlignToSurface(Vector3 vel)
        {
            if (_movementState.Grounded.IsInState && IsOnTooSteepSlope() == false)
            {
                vel = ProjectVelocityOnSurface(vel, _movementState.Grounded.CollisionInfo.Hit.Normal);
            }

            return vel;
        }

        public bool IsOnTooSteepSlope()
        {
            var angle = GetGroundAngle();
            return IsSlopeTooSteep(angle);
        }

        /// <summary>
        /// Rerturn 0 if there is no surface or is on the ground
        /// </summary>
        public float GetSurfaceAngle(Vector3 directionToSurface)
        {
            _movementState.HaveCollision(directionToSurface.normalized, out HitInfo hit);
            float angle = Vector3.Angle(Vector3.up, hit.Normal);

            return angle;
        }

        /// <summary>
        /// Rerturn 0 if there is no surface or is on the ground
        /// </summary>
        public float GetGroundAngle()
        {
            var hit = _movementState.Grounded.CollisionInfo.Hit;
            float angle = Vector3.Angle(Vector3.up, hit.Normal);

            return angle;
        }

        private Vector3 ScaleHorizontalVelocity(Vector3 vel, Vector3 projectedVel, Vector3 surfaceNormal)
        {
            surfaceNormal.y = 0;
            vel.y = Mathf.Clamp(vel.y, -1000, 0);
            float scale = 1 + Vector3.Dot(vel.normalized, surfaceNormal.normalized);
            //Debug.Log($" surface: {surfaceNormal}, vel {vel.normalized} scale: {scale}");
            var scaledVel = projectedVel * scale;

            return scaledVel;
        }
    }
}

