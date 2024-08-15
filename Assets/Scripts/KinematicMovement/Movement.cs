using PlotvaIzLodzya.KinematicMovement.Body;
using PlotvaIzLodzya.KinematicMovement.CollideAndSlide;
using PlotvaIzLodzya.KinematicMovement.CollisionCompute;
using PlotvaIzLodzya.KinematicMovement.ExternalMovement;
using PlotvaIzLodzya.KinematicMovement.StateHandle;
using PlotvaIzLodzya.KinematicMovement.VelocityCompute;
using UnityEngine;

namespace PlotvaIzLodzya.KinematicMovement
{
    public class Movement : MonoBehaviour
    {
        [field: SerializeField] public MovementConfig MovementConfig { get; private set; }
        public Vector3 vel;
        private Vector3 _direction;
        private IBody _body;
        private ICollision _collision;
        private IVelocityCompute _velocityCompute;
        private ISlide _slide;
        private VelocityHandler _velocityHandler;


        public Vector3 Velocity { get; private set; }
        public Vector3 AngularVelocity { get; private set; }
        public MovementState State { get; private set; }
        public ExteranlVelocityAccumulator ExteranalMovementAccumulator { get; private set; }


        private void Awake()
        {
            var frameRate = 144;
            Application.targetFrameRate = frameRate;
            Time.fixedDeltaTime = 1f / frameRate;
            _body = BodyBuilder.Create(gameObject);
            _collision = CollisionBuilder.Create(gameObject, _body, MovementConfig);
            State = new MovementState(_body, _collision, MovementConfig);
            ExteranalMovementAccumulator = new(State);
            _slide = SlideBuilder.Create(_body, _collision, State);
            _velocityHandler = new(State, MovementConfig, ExteranalMovementAccumulator);
            _velocityCompute = _velocityHandler.GetVelocityCompute<VelocityComputation>();
        }

        private void Update()
        {
            Move(vel);
        }

        private void FixedUpdate()
        {
            UpdateBody(Time.fixedDeltaTime);
        }

        public void Move(Vector3 direction)
        {
            _slide.SetDesireDir(direction);
            _direction = direction;
        }

        public void Jump()
        {
            Jump(MovementConfig.JumpSpeed);
        }

        public void Jump(float speed)
        {
            if (State.Ceiled)
                return;

            _velocityCompute.Jump(speed);

            State.SetJumping(true);
        }

        private void UpdateBody(float deltaTime)
        {
            _body.Position = transform.position;
            _body.Rotation = transform.rotation;
            _body.Position = HandleExternalMovement(_body.Position);
            _collision.Depenetrate();
            var velocity = CalculateVelocity(_body.Position, deltaTime);
            var nextPos = _body.Position + velocity;
            _body.Position = nextPos;
            //_body.Velocity = velocity / deltaTime;

            _body.LocalScale = transform.localScale;

            _velocityCompute = _velocityHandler.GetVelocityCompute();
            Velocity = _velocityCompute.Velocity;
            State.Update(_direction);
        }

        private Vector3 HandleExternalMovement(Vector3 position)
        {
            var prevPosition = position;

            if (State.IsOnTooSteepSlope() == false && State.Grounded)
                position = ExteranalMovementAccumulator.GetPositionByRotation(position);

            AngularVelocity = position - prevPosition;

            position += ExteranalMovementAccumulator.TotalVelocity;
            return position;
        }

        private Vector3 CalculateVelocity(Vector3 pos, float deltaTime)
        {
            var totalVelocity = CalculateHorizontalVelocity(pos, deltaTime);
            var nextPosAlongSurface = pos + totalVelocity;
            totalVelocity += CalculateVerticalVelocity(nextPosAlongSurface, deltaTime);

            if (totalVelocity.magnitude <= MovementConfig.MinDistanceTravel)
                totalVelocity = Vector3.zero;

            return totalVelocity;
        }

        private Vector3 CalculateHorizontalVelocity(Vector3 pos, float deltaTime)
        {
            var horVelocity = _velocityCompute.CalculateHorizontalSpeed(_direction, deltaTime);
            horVelocity *= deltaTime;
            var vel = _slide.SlideByMovement_recursive(horVelocity, pos);
            //Debug.Log(vel);

            return vel;
        }

        private Vector3 CalculateVerticalVelocity(Vector3 pos, float deltaTime)
        {
            var verSpeed = _velocityCompute.CalculateVerticalSpeed(deltaTime);

            var vertVel = Vector3.up * verSpeed * deltaTime;
            var vel = _slide.SlideByGravity_recursive(vertVel, pos);

            return vel;
        }
    }
}