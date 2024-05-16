using PlotvaIzLodzya.Movement.Platforms;
using PlotvaIzLodzya.Player.Movement.CollideAndSlide;
using PlotvaIzLodzya.Player.Movement.CollideAndSlide.CollisionDetection;
using Unity.VisualScripting;
using UnityEngine;

namespace PlotvaIzLodzya.Player.Movement
{
    public interface IMovable
    {
        public Vector3 ExternalVelocity { get; set; }

        void Move(Vector3 direction);
        void SetOnPlatoform(MovingPlatform platoform);
        void LeavePlatoform(MovingPlatform platoform);
    }

    public class Movement : MonoBehaviour, IMovable
    {
        [SerializeField] private bool _enableGravity;
        [SerializeField] private CollisionConfig _collisionConfig;

        [field: SerializeField] public MovementConfig MovementConfig { get; private set; }

        private bool _jumpRequested;
        private bool _moveRequested;
        private int _collideDepth;
        private Vector3 _currentHorizontalVelocity;
        private Vector3 _currentVerticalVelocity;
        private Vector3 _desiredVelocity;
        private ICollisionHandler _collisionHandler;
        private Transform _transform;
        private Velocity _velocity;
        private IBody _rigidbody;
        public MovementState State { get; private set; }
        public bool IsGrounded { get; private set; }
        public Vector3 ExternalVelocity { get; set; }

        private void Awake()
        {
            _transform = transform;
            _collisionHandler = CollisionHandlerBuilder.Create(gameObject, _collisionConfig);
            _rigidbody = BodyBuilder.Create(gameObject);   
            State = new(_collisionHandler, _transform);
            _velocity = new(MovementConfig);
            _collideDepth = 5;
        }

        public void Update()
        {
            _currentHorizontalVelocity = _velocity.CalculateHorizontal(_currentHorizontalVelocity, _desiredVelocity, _moveRequested);
            var onTooSteepSlope = false; 
            var vel = _currentHorizontalVelocity;
            vel += ExternalVelocity;
            State.Update(vel.normalized);
            vel = HandleWall(vel);
            onTooSteepSlope = IsOnTooSteepSlope();


            if (_enableGravity)
            {
                vel.y = ApplyGravity(_currentVerticalVelocity);
            }

            if (onTooSteepSlope == false)
            {
                vel = AlignToSurface(vel);
            }

            if (_jumpRequested)
            {
                vel.y = _velocity.CalculateJumpSpeed();
                _currentVerticalVelocity.y = vel.y;
                _jumpRequested = false;
            }

            if (State.Ceiled.IsEnterState)
            {
                vel.y = 0;
                _currentVerticalVelocity.y = 0;
            }

            Translate(vel);


            IsGrounded = State.Grounded.IsInState;
            _desiredVelocity = Vector3.zero;
            _moveRequested = false;
        }

        private void Translate(Vector3 vel)
        {
            vel = CollideAndSlide_recursive(vel * Time.deltaTime, _rigidbody.Position);
            _rigidbody.MovePosition(_rigidbody.Position + vel);
        }

        public void SetOnPlatoform(MovingPlatform platoform)
        {
            
        }

        public void LeavePlatoform(MovingPlatform platoform)
        {
            
        }

        public void Move(Vector3 direction)
        {
            _moveRequested = true;
            _desiredVelocity = direction * MovementConfig.Speed;
        }

        public void Jump()
        {
            IsGrounded = false;
            _jumpRequested = true;
        }
        
        private Vector3 HandleWall(Vector3 vel)
        {
            var angle = GetSurfaceAngle(vel.normalized);
                
            if (State.CurrentCollision.IsEnterState && IsSlopeTooSteep(angle))
            {
                vel = Vector3.zero;
                _currentHorizontalVelocity = Vector3.zero;
            }

            return vel;
        }

        private float ApplyGravity(Vector3 velocity)
        {
            var vel = _velocity.CalculateVertical(velocity);
            _currentVerticalVelocity = vel;
            return vel.y;
        }

        private Vector3 CollideAndSlide_recursive(Vector3 vel, Vector3 currentPos, int currentDepth = 0)
        {
            if (currentDepth >= _collideDepth)
                return Vector3.zero;

            float dist = vel.magnitude + CollisionConfig.ClipPreventingValue;

            var dir = vel.normalized;

            if (_collisionHandler.IsCollide(currentPos, dir, out HitInfo hit, dist))
            {
                var velToNextStep = dir * (hit.Distance - CollisionConfig.ClipPreventingValue);
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

        private bool IsSlopeTooSteep(float angle)
        {
            return angle >= MovementConfig.MaxSlopeAngle;
        }

        private bool IsOnTooSteepSlope()
        {
            var angle = GetSurfaceAngle(Vector3.down);

            return IsSlopeTooSteep(angle);
        }

        private Vector3 HandleSlope(float slopeAngle, Vector3 vel, Vector3 projectedleftOverVel, Vector3 surfaceNormal)
        {
            if (IsGrounded == false)
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
                vel = ProjectVelocityOnSurface(vel, State.Grounded.CollisionInfo.Hit.Normal);
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
            float angle = Vector3.Angle(Vector3.up, hit.Normal);

            return angle;
        }
    }
}

