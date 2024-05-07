using PlotvaIzLodzya.Player.Movement.CollideAndSlide;
using PlotvaIzLodzya.Player.Movement.CollideAndSlide.CollisionDetection;
using System;
using UnityEngine;

namespace PlotvaIzLodzya.Player.Movement
{
    public interface IMovable
    {
        void Move(Vector3 direction);
    }

    public class Movement : MonoBehaviour, IMovable
    {
        [SerializeField] private bool _enableGravity;
        [SerializeField] private CollisionConfig _collisionConfig;

        [field: SerializeField] public MovementConfig MovementConfig { get; private set; }

        private bool _moveRequested;
        private int _collideDepth;
        private Vector3 _exteranalForce;
        private Vector3 _currentHorizontalVelocity;
        private Vector3 _currentVerticalVelocity;
        private Vector3 _desiredVelocity;
        private ICollisionHandler _collisionHandler;
        private Transform _transform;
        private WorldConfig _wordlConfig;
        private Velocity _velocity;

        public float Speed { get; private set; }
        public MovementState State { get; private set; }
        public bool IsGrounded { get; private set; }

        private void Awake()
        {
            _transform = transform;
            _wordlConfig = new (Vector3.down*14f, Vector3.up);
            _collisionHandler = CollisionHandlerBuilder.Create(gameObject, _collisionConfig);
            State = new(_collisionHandler, _transform);
            _velocity = new(MovementConfig);
            _collideDepth = 5;
            Speed = 0f;
        }

        private void Update()
        {
            State.Update();

            _currentHorizontalVelocity = _velocity.CalculateHorizontal(_currentHorizontalVelocity, _desiredVelocity, _moveRequested);

            var vel = _currentHorizontalVelocity;
            
            
            vel += _exteranalForce;

            if (IsOnTooSteepSlope() == false)
                vel = AlignToSurface(vel);

            if (_enableGravity && IsGrounded == false)
            {
                _currentVerticalVelocity = ApplyGravity();
                vel.y += _currentVerticalVelocity.y;
            }

            Translate(vel);

            _desiredVelocity = Vector3.zero;
            _moveRequested = false;
            IsGrounded = State.Grounded.IsInState;
        }

        public void Move(Vector3 direction)
        {
            _moveRequested = true;
            _desiredVelocity = direction * MovementConfig.Speed;
        }

        public void Jump()
        {
            IsGrounded = false;

            _currentVerticalVelocity.y = MovementConfig.GetJumpSpeed(); 
        }

        private Vector3 ApplyGravity()
        {
            var terminalVerticalVelocity = MovementConfig.FallMaxSpeed * -_wordlConfig.WorldUp;
            if (IsGrounded)
            {
                terminalVerticalVelocity = MovementConfig.FallStartSpeed * -_wordlConfig.WorldUp;
            }

            var vel = _velocity.CalculateVertical(_currentVerticalVelocity, terminalVerticalVelocity);
            return vel;
        }

        private void Translate(Vector3 vel)
        {
            vel = CollideAndSlide_recursive(vel * Time.deltaTime, _transform.position);
            _transform.position += vel;
        }

        private Vector3 CollideAndSlide_recursive(Vector3 vel, Vector3 currentPos, int currentDepth = 0)
        {
            if (currentDepth >= _collideDepth)
                return Vector3.zero;

            float dist = vel.magnitude + _collisionConfig.ClipPreventingValue;
            var dir = vel.normalized;

            if (_collisionHandler.IsCollide(currentPos, dir, out HitInfo hit, dist))
            {
                var velToNextStep = dir * (hit.distance - _collisionConfig.ClipPreventingValue);
                var leftOverVel = vel - velToNextStep;

                var nextPos = currentPos + velToNextStep;

                float angle = Vector3.Angle(_wordlConfig.WorldUp, hit.normal);

                var projectedleftOverVel = Vector3.ProjectOnPlane(leftOverVel, hit.normal);

                projectedleftOverVel = HandleSlope(angle, vel, projectedleftOverVel, hit.normal);

                vel = velToNextStep + CollideAndSlide_recursive(projectedleftOverVel, nextPos, ++currentDepth);

                return vel;
            }

            return vel;
        }

        private bool IsSlopeTooSteep(float angle)
        {
            return angle >= MovementConfig.MaxSlopeAngle;
        }

        private bool IsOnTooSteepSlope()
        {
            var angle = GetSurfaceAngle(-_collisionConfig.ObjectUp);

            return IsSlopeTooSteep(angle);
        }

        private Vector3 HandleSlope(float slopeAngle, Vector3 vel, Vector3 projectedleftOverVel, Vector3 surfaceNormal)
        {
            if (IsGrounded == false)
                return projectedleftOverVel;

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

        private Vector3 ScaleHorizontalVelocity(Vector3 vel, Vector3 projectedVel, Vector3 surfaceNormal)
        {
            vel.y = 0;
            surfaceNormal.y = 0;
            projectedVel.y = vel.y;
            float scale = 1 + Vector3.Dot(vel.normalized, surfaceNormal.normalized);

            var scaledVel = projectedVel * scale;

            return scaledVel;
        }

        private Vector3 AlignToSurface(Vector3 vel)
        {
            if (IsGrounded)
            {
                vel = ProjectVelocityOnSurface(vel, State.Grounded.CollisionInfo.Hit.normal);
            }
                
            return vel;
        }

        private Vector3 ProjectVelocityOnSurface(Vector3 vel, Vector3 normal)
        {
            vel.y = 0;
            vel = Vector3.ProjectOnPlane(vel, normal);
            return vel;
        }


        /// <summary>
        /// Rerturn 0 if there is no surface or is on the ground
        /// </summary>
        private float GetSurfaceAngle(Vector3 directionToSurface)
        {
            State.HaveCollision(directionToSurface.normalized, out HitInfo hit);
            float angle = Vector3.Angle(_wordlConfig.WorldUp, hit.normal);

            return angle;
        }
    }
}

