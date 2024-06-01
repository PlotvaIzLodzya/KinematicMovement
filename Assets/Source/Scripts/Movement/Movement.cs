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

    public interface IJumpStatus
    {
        bool Requested { get; }
        bool Ended { get; }
    }

    public class Jump : IJumpStatus
    {
        public bool Requested {get; set; }

        public bool Ended { get; set; }
    }

    public class Movement : MonoBehaviour, IMovable
    {
        [SerializeField] private bool _enableGravity;
        [SerializeField] private CollisionConfig _collisionConfig;

        [field: SerializeField] public MovementConfig MovementConfig { get; private set; }

        private bool _moveRequested;
        private Vector3 _currentHorizontalVelocity;
        private Vector3 _currentVerticalVelocity;
        private Vector3 _desiredVelocity;
        private ICollisionHandler _collisionHandler;
        private Transform _transform;
        private Velocity _velocity;
        private IBody _rigidbody;
        private Jump _jump;
        
        private Slide _slide;
        public MovementState State { get; private set; }
        public Vector3 ExternalVelocity { get; set; }

        private void Awake()
        {
            _transform = transform;
            _collisionHandler = CollisionHandlerBuilder.Create(gameObject, _collisionConfig);
            _rigidbody = BodyBuilder.Create(gameObject);   
            _jump = new();
            State = new(_collisionHandler, _transform, _jump);
            _velocity = new(MovementConfig);            
            _slide = new(_collisionHandler, collideDepth:5, MovementConfig, State);
        }

        public void Update()
        {
            if (_collisionHandler.IsCollide(_transform.position, out HitInfo hit))
            {
                transform.position -= hit.Normal * hit.Distance;
            }

            _currentHorizontalVelocity = _velocity.CalculateHorizontal(_currentHorizontalVelocity, _desiredVelocity, _moveRequested);
            _currentHorizontalVelocity = _slide.HandleWall(_currentHorizontalVelocity);
            var vel = _currentHorizontalVelocity;
            //Debug.Log(State.Grounded.IsInState);            
            vel = _slide.HandleWall(vel);

            vel.y = ApplyGravity(_currentVerticalVelocity);

            if (_jump.Ended)
                vel = _slide.AlignToSurface(vel);

            vel = ApplyExternalForces(vel);
            vel = ApplyJump(vel);
            vel = ApplyCeiling(vel);

            State.Update(vel.normalized);
            Translate(vel);
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
            _jump.Ended = false;
            _jump.Requested = true;
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
            if (_jump.Requested)
            {
                vel.y = _velocity.CalculateJumpSpeed();
                _currentVerticalVelocity.y = vel.y;
                _jump.Requested = false;
            }

            return vel;
        }

        private Vector3 ApplyExternalForces(Vector3 velocity)
        {
            var externalVelocity = ExternalVelocity;

            if (externalVelocity.y < 0 && State.Grounded.IsInState)
                externalVelocity.y *= 10;

            velocity += externalVelocity;
            
            return velocity;
        }

        private void Translate(Vector3 vel)
        {
            vel = _slide.CollideAndSlide_recursive(vel * Time.deltaTime, _transform.position);
            _transform.position += vel;
        }

        private float ApplyGravity(Vector3 velocity)
        {
            if (_enableGravity)
            {
                velocity = _velocity.CalculateVertical(velocity);
                _currentVerticalVelocity = velocity;

                if (State.Grounded.IsEnterState)
                {
                    _jump.Ended = true;
                }
            }

            return velocity.y;
        }
    }
}

