using PlotvaIzLodzya.Extensions;
using PlotvaIzLodzya.KinematicMovement.StateHandle;
using UnityEngine;

namespace PlotvaIzLodzya.KinematicMovement.VelocityCompute
{

    public class VelocityComputation : IVelocityCompute
    {
        private Vector3 _minVelocity;
        private Vector3 _velocity;
        private IMovementState _state;
        private MovementConfig MovementConfig;
        private float _vertSpeed;

        protected Vector3 Direction { get; set; }
        protected virtual float MaxHorizontalSpeed => MovementConfig.Speed;

        public Vector3 Velocity
        {
            get
            {
                var totalVel = _velocity;
                totalVel.y = _vertSpeed;
                return totalVel;
            }
            protected set
            {
                _velocity = value;
                _vertSpeed = value.y;
            }
        }

        public VelocityComputation(IMovementState state, MovementConfig movementConfig)
        {
            _state = state;
            _minVelocity = Vector3.zero;
            MovementConfig = movementConfig;
        }

        public virtual Vector3 CalculateHorizontalSpeed(Vector3 dir, float deltaTime)
        {
            var maxVel = dir * MaxHorizontalSpeed;
            Direction = dir;
            _velocity = maxVel;
            return maxVel;
            if (_state.CrashedIntoWall)
            {
                _velocity = Vector3.zero;
                return Vector3.zero;
            }

            //if (_state.HaveWallCollision)
            //{
            //    //_velocity = Vector3.zero;
            //    _velocity = Vector3.ProjectOnPlane(_velocity, _state.WallNormal);
                
            //}
            var hor = _velocity.Horizontal();
            if (dir.sqrMagnitude > 0)
                _velocity = Vector3.MoveTowards(hor, maxVel, MovementConfig.Acceleration * deltaTime);
            else
                _velocity = Vector3.MoveTowards(hor, _minVelocity, MovementConfig.Decceleration * deltaTime);

            return _velocity;
        }

        public virtual float CalculateVerticalSpeed(float deltaTime)
        {
            var vertAccel = 0f;
            if (_state.Grounded == false || _state.OnTooSteepSlope)
            {
                vertAccel = MovementConfig.VerticalAcceleration;
            }

            if (_state.LeftGround && _state.IsJumping == false)
            {
                _vertSpeed = -9.8f;
            }

            if (_state.BecomeCeiled && _state.IsJumping)
            {
                _vertSpeed = 0f;
            }

            _vertSpeed -= vertAccel * deltaTime;
            _vertSpeed = Mathf.Clamp(_vertSpeed, MovementConfig.MinVerticalSpeed, MovementConfig.MaxVertiaclSpeed);

            return _vertSpeed;
        }

        public virtual void Enter(Vector3 currentVelocity)
        {
            Velocity = currentVelocity;
        }

        public virtual void Exit()
        {
            Direction = Vector3.zero;
        }

        public virtual void Jump(float speed)
        {
            _vertSpeed = speed;
        }
    }
}
