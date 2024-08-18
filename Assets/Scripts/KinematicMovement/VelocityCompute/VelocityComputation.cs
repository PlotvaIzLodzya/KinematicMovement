using PlotvaIzLodzya.Extensions;
using PlotvaIzLodzya.KinematicMovement.StateHandle;
using UnityEngine;

namespace PlotvaIzLodzya.KinematicMovement.VelocityCompute
{

    public class VelocityComputation : IVelocityCompute
    {
        private float _minHorSpeed;
        private Vector3 _velocity;
        private IMovementState _state;
        private MovementConfig MovementConfig;
        private float _vertSpeed;
        private float _horSpeed;

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
            _minHorSpeed = 0;
            MovementConfig = movementConfig;
        }

        public virtual Vector3 CalculateHorizontalSpeed(Vector3 dir, float deltaTime)
        {
            Direction = dir;
            if (_state.CrashedIntoWall)
            {
                _velocity = Vector3.zero;
                return Vector3.zero;
            }

            if (Direction.sqrMagnitude > 0)
                _horSpeed = Mathf.MoveTowards(_horSpeed, MaxHorizontalSpeed , MovementConfig.Acceleration * deltaTime);
            else
                _horSpeed = Mathf.MoveTowards(_horSpeed, _minHorSpeed, MovementConfig.Decceleration * deltaTime);

            _velocity = Direction.GetHorizontal() * _horSpeed;
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
