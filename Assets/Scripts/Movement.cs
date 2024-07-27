using TMPro;
using UnityEngine;

public class Movement : MonoBehaviour
{
    [field: SerializeField] public MovementConfig MovementConfig { get; private set; }

    private float _verticalSpeed;
    private Vector3 _direction;
    private IBody _body;
    private ICollision _collision;
    private Velocity _velocity;
    private SlideAlongSurface _slide;
    private Camera _camera;

    public Vector3 Velocity { get; private set; }
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
        ExteranalMovementAccumulator = new (State);
        _slide = new SlideAlongSurface(_collision, State);
        _velocity = new Velocity(State, MovementConfig);
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

        if(_direction.sqrMagnitude > 0)
            _direction = _direction.normalized;
    }

    private void FixedUpdate()
    {
        Move(Time.fixedDeltaTime);
    }

    public void Jump(float speed)
    {
        _verticalSpeed = speed;
        State.SetJumping(true);
    }

    private void Move(float deltaTime)
    {
        _body.Position = transform.position;
        _body.Position = HandleExternalMovement(_body.Position);
        _collision.Depenetrate();
        _verticalSpeed = _velocity.CalculateVerticalSpeed(_verticalSpeed, deltaTime);

        var velocity = CalculateVelocity(_body.Position, deltaTime);
        Velocity = velocity / deltaTime;
        var nextPos = _body.Position + velocity;
        _body.Position = nextPos;

        State.Update(_direction);
    }

    private Vector3 HandleExternalMovement(Vector3 position)
    {
        if (State.IsOnTooSteepSlope() == false && State.Grounded)
            position = ExteranalMovementAccumulator.GetPositionByRotation(position);

        position += ExteranalMovementAccumulator.TotalVelocity;
        return position;
    }

    private Vector3 CalculateVelocity(Vector3 pos, float deltaTime)
    {
        var totalVelocity = CalculateHorizontalVelocity(pos, deltaTime);
        var nextPosAlongSurface = pos + totalVelocity;
        totalVelocity += CalculateVerticalVelocity(nextPosAlongSurface, deltaTime);

        if (totalVelocity.magnitude > 0)
            totalVelocity = totalVelocity.ClampMagnitude(MovementConfig.MinDistanceTravel);

        return totalVelocity;
    }

    private Vector3 CalculateHorizontalVelocity(Vector3 pos, float deltaTime)
    {
        var horVelocity = _velocity.CalculateHorizontalSpeed(_direction, deltaTime) * deltaTime;
        var vel = _slide.SlideByMovement_recursive(horVelocity, pos);

        return vel;
    }

    private Vector3 CalculateVerticalVelocity(Vector3 pos, float deltaTime)
    {
        var vertVel = Vector3.up * _verticalSpeed * deltaTime;
        var vel = _slide.SlideByGravity_recursive(vertVel, pos);

        return vel;
    }
}
