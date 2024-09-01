using PlotvaIzLodzya.Extensions;
using PlotvaIzLodzya.KinematicMovement.Body;
using PlotvaIzLodzya.KinematicMovement.CollideAndSlide;
using PlotvaIzLodzya.KinematicMovement.CollisionCompute;
using PlotvaIzLodzya.KinematicMovement.ExternalMovement;
using PlotvaIzLodzya.KinematicMovement.Jump;
using PlotvaIzLodzya.KinematicMovement.StateHandle;
using PlotvaIzLodzya.KinematicMovement.VelocityCompute;
using System;
using UnityEngine;

namespace PlotvaIzLodzya.KinematicMovement
{
    public class Movement : MonoBehaviour, ICoroutineRunner
    {
        [SerializeField] private JumpBehaviour _jumpBehaviour;
        [SerializeField] private VelocityHandler _velocityHandler;

        [field: SerializeField] public MovementConfig MovementConfig { get; private set; }

        private Vector3 _direction;
        private IBody _body;
        private ICollision _collision;
        private IVelocityCompute _velocityCompute;
        private ISlide _slide;
        private float elapsedTime;
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
            _velocityHandler = VelocityHandlerBuilder.Create(_velocityHandler, MovementConfig, State, ExteranalMovementAccumulator);
            _velocityCompute = _velocityHandler.GetVelocityCompute<GroundVelocity>();
            _jumpBehaviour = JumpBuilder.Create(_jumpBehaviour, _velocityHandler, this);
        }

        private void FixedUpdate()
        {
            UpdateBody(Time.fixedDeltaTime);
        }

        public void Move(Vector3 direction)
        {
            _direction = direction;
        }

        public void Jump()
        {
            Jump(MovementConfig.JumpSpeed);
        }

        public void CancelJump()
        {
            _jumpBehaviour.CancelJump();
        }

        public void Jump(float speed)
        {
            if (State.Ceiled)
                return;
            try
            {
                _jumpBehaviour.Jump(speed);
            }
            catch (NullReferenceException)
            {
                Debug.LogError($"Perhaps {_jumpBehaviour.GetType().Name} was not initialized properly");
                _jumpBehaviour = JumpBuilder.Create(_jumpBehaviour, _velocityHandler, this);
            }

            State.SetJumping(true);
        }

        private void UpdateBody(float deltaTime)
        {
            _velocityCompute = _velocityHandler.GetVelocityCompute();
            _body.Position = transform.position;
            _body.Rotation = transform.rotation;
            _body.Position = HandleExternalMovement(_body.Position);
            _collision.Depenetrate();
            var velocity = CalculateVelocity(_body.Position, deltaTime);
            var nextPos = _body.Position + velocity;
            
            _body.Position = nextPos;

            _body.LocalScale = transform.localScale;

            Velocity = _velocityCompute.Velocity;
            State.Update(_direction, Velocity);

            if(State.LeftGround)
            {
                elapsedTime = 0f;
            }
            elapsedTime += deltaTime;
            if (State.BecomeGrounded)
                Debug.Log(elapsedTime);
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
            var vel = _slide.SlideByMovement_recursive(horVelocity, _direction, pos);

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