using PlotvaIzLodzya.Extensions;
using PlotvaIzLodzya.KinematicMovement.StateHandle;
using UnityEngine;

namespace PlotvaIzLodzya.KinematicMovement.VelocityCompute
{
    [CreateAssetMenu(fileName = nameof(GroundVelocity), menuName = "SO/" + nameof(VelocityComputation) + "/" + nameof(GroundVelocity), order = 2)]
    public class GroundVelocity : VelocityComputation
    {
        private float _vertSpeed;
        private float _horSpeed;
        private float _minHorSpeed;
        private Vector3 _velocity;

        private MovementConfig MovementConfig;

        public override bool CanTransit => State.Grounded;
        public override Vector3 Velocity
        {
            get
            {
                var totalVel = _velocity;
                totalVel.y = _vertSpeed;
                return totalVel;
            }
            set
            {
                _velocity = value;
                _vertSpeed = value.y;
            }
        }

        protected Vector3 Direction { get; set; }
        protected virtual float MaxHorizontalSpeed => MovementConfig.Speed;

        public virtual void Init(IMovementState state, MovementConfig movementConfig)
        {
            State = state;
            _vertSpeed = 0f;
            _horSpeed = 0f;
            _minHorSpeed = 0f;
            _velocity = Vector3.zero;
            Direction = Vector3.zero;
            State = state;
            _minHorSpeed = 0;
            MovementConfig = movementConfig;
        }

        public override Vector3 CalculateHorizontalSpeed(Vector3 dir, float deltaTime)
        {
            Direction = dir;
            if (State.CrashedIntoWall)
            {
                _velocity = Vector3.zero;
                _horSpeed = 0f;
                return Vector3.zero;
            }

            if (Direction.sqrMagnitude > 0)
            {
                _horSpeed = Mathf.MoveTowards(_horSpeed, MaxHorizontalSpeed, MovementConfig.Acceleration * deltaTime);
                _velocity = Direction.GetHorizontal() * _horSpeed;
            }
            else
            {
                _horSpeed = Mathf.MoveTowards(_horSpeed, _minHorSpeed, MovementConfig.Decceleration * deltaTime);
                _velocity = _velocity.GetHorizontal().ClampMagnitude(0, _horSpeed);
            }
            
            return _velocity;
        }

        public override float CalculateVerticalSpeed(float deltaTime)
        {
            var vertAccel = 0f;
            if (State.Grounded == false || State.OnTooSteepSlope)
            {
                vertAccel = MovementConfig.VerticalAcceleration;
            }

            if (State.LeftGround && State.IsJumping == false)
            {
                _vertSpeed = -9.8f;
            }

            if (State.BecomeCeiled && State.IsJumping)
            {
                _vertSpeed = 0f;
            }

            _vertSpeed -= vertAccel * deltaTime;
            _vertSpeed = Mathf.Clamp(_vertSpeed, MovementConfig.MinVerticalSpeed, MovementConfig.MaxVertiaclSpeed);
            _velocity.y = _vertSpeed;
            return _velocity.y;
        }

        public override void Enter(Vector3 currentVelocity) => SetVelocity(currentVelocity);
        public override void Exit() => Direction = Vector3.zero;
        public override void Jump(float speed) => _vertSpeed = speed;

        public override void SetVelocity(Vector3 currentVelocity)
        {
            Velocity = currentVelocity;
            _horSpeed = currentVelocity.GetHorizontal().magnitude;
            _vertSpeed = currentVelocity.y;
        }
    }
}
