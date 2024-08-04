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

        private Vector3 _direction;
        private IBody _body;
        private ICollision _collision;
        private IVelocityCompute _velocityCompute;
        private VelocityHandler _velocityHandler;
        private SlideAlongSurface _slide;
        private Camera _camera;

        public Vector3 Velocity { get; private set; }
        public Vector3 AngularVelocity { get; private set; }
        public MovementState State { get; private set; }
        public ExteranlVelocityAccumulator ExteranalMovementAccumulator { get; private set; }

        private void Awake()
        {
            var frameRate = 144;
            Application.targetFrameRate = frameRate;
            Time.fixedDeltaTime = 1f / frameRate;
            _camera = Camera.main;
            _body = BodyBuilder.Create(gameObject);
            _collision = CollisionBuilder.Create(gameObject, _body, MovementConfig);
            State = new MovementState(_body, _collision, MovementConfig);
            ExteranalMovementAccumulator = new(State);
            _slide = new SlideAlongSurface(_collision, State);
            _velocityHandler = new(State, MovementConfig, ExteranalMovementAccumulator);
            _velocityCompute = _velocityHandler.GetVelocityCompute<VelocityCompute.VelocityCompute>();
        }

        private void Update()
        {
            _direction = Vector2.zero;

            if (Input.GetKey(KeyCode.D))
                _direction += _camera.transform.right;

            if (Input.GetKey(KeyCode.A))
                _direction -= _camera.transform.right;

            if (Input.GetKey(KeyCode.S))
                _direction -= _camera.transform.forward;

            if (Input.GetKey(KeyCode.W))
                _direction += _camera.transform.forward;

            if (Input.GetKeyDown(KeyCode.Space))
                Jump(MovementConfig.JumpSpeed);

            if (Input.GetKey(KeyCode.Q))
                _body.Rotation *= Quaternion.Euler(Vector3.down * 90 * Time.deltaTime);
            if (Input.GetKey(KeyCode.E))
                _body.Rotation *= Quaternion.Euler(Vector3.up * 90 * Time.deltaTime);

            if (_direction.sqrMagnitude > 0)
                _direction = _direction.normalized;            
        }

        private void FixedUpdate()
        {
            UpdateBody(Time.fixedDeltaTime);
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
            _body.Position = HandleExternalMovement(_body.Position);
            _collision.Depenetrate();
            var velocity = CalculateVelocity(_body.Position, deltaTime);
            var nextPos = _body.Position + velocity;
            _body.Position = nextPos;

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