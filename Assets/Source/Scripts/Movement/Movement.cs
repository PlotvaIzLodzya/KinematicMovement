using PlotvaIzLodzya.Movement.Platforms;
using PlotvaIzLodzya.Player.Movement.CollideAndSlide;
using PlotvaIzLodzya.Player.Movement.CollideAndSlide.CollisionDetection;
using UnityEngine;

namespace PlotvaIzLodzya.Player.Movement
{
    public interface IMovable
    {
        public Vector3 ExternalVelocity { get; set; }

        void Move(Vector3 direction);
    }

    public class Movement : MonoBehaviour, IMovable
    {
        [SerializeField] private bool _enableGravity;
        [SerializeField] private CollisionConfig _collisionConfig;

        [field: SerializeField] public MovementConfig MovementConfig { get; private set; }

        private bool _jumpRequested;
        private bool _jumpEnded;
        private bool _moveRequested;
        private Vector3 _currentHorizontalVelocity;
        private Vector3 _currentVerticalVelocity;
        private Vector3 _desiredVelocity;
        private ICollisionHandler _collisionHandler;
        private Transform _transform;
        private Velocity _velocity;
        private IBody _rigidbody;
        private Slide _slide;
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
            _slide = new(_collisionHandler, collideDepth:5, MovementConfig, State);
        }

        public void Update()
        {
            _currentHorizontalVelocity = _velocity.CalculateHorizontal(_currentHorizontalVelocity, _desiredVelocity, _moveRequested);
            _currentHorizontalVelocity = _slide.HandleWall(_currentHorizontalVelocity);
            var vel = _currentHorizontalVelocity;

            vel = ApplyExternalForces(vel);
            State.Update(vel.normalized);

            vel.y = ApplyGravity(_currentVerticalVelocity);
            vel = _slide.AlignToSurface(vel);
            vel = ApplyJump(vel);
            vel = ApplyCeiling(vel);

            Translate(vel);

            IsGrounded = State.Grounded.IsInState;
            _desiredVelocity = Vector3.zero;
            _moveRequested = false;
        }

        public void Move(Vector3 direction)
        {
            _moveRequested = true;
            _desiredVelocity = direction * MovementConfig.Speed;
        }

        public void Jump()
        {
            _jumpEnded = false;
            IsGrounded = false;
            _jumpRequested = true;
        }

        private Vector3 ApplyCeiling(Vector3 vel)
        {
            if (State.Ceiled.IsEnterState)
            {
                vel.y = 0;
                _currentVerticalVelocity.y = 0;
            }

            return vel;
        }

        private Vector3 ApplyJump(Vector3 vel)
        {
            if (_jumpRequested)
            {
                vel.y = _velocity.CalculateJumpSpeed();
                _currentVerticalVelocity.y = vel.y;
                _jumpRequested = false;
            }

            return vel;
        }

        private Vector3 ApplyExternalForces(Vector3 velocity)
        {
            velocity += ExternalVelocity;
            
            return velocity;
        }

        private void Translate(Vector3 vel)
        {
            vel = _slide.CollideAndSlide_recursive(vel * Time.deltaTime, _rigidbody.Position);
            _rigidbody.MovePosition(_rigidbody.Position + vel);
        }

        private float ApplyGravity(Vector3 velocity)
        {
            if (_enableGravity)
            {
                velocity = _velocity.CalculateVertical(velocity);
                _currentVerticalVelocity = velocity;

                //if(IsGrounded && _jumpEnded)
                //{
                //    velocity.y *= -100f;
                //}

                if (State.Grounded.IsEnterState)
                {
                    _jumpEnded = true;
                }

                //if (State.Grounded.IsExitState && _jumpEnded)
                //{
                //    _currentVerticalVelocity = Vector3.zero;
                //    velocity = _currentVerticalVelocity;
                //}
            }

            return velocity.y;
        }
    }
}

