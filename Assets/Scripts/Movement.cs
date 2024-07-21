using UnityEngine;

public class Movement : MonoBehaviour
{
    [field: SerializeField] public MovementConfig MovementConfig { get; private set; }

    private Vector3 _direction;
    private IBody _body;
    private ICollision _collision;
    private SlideAlongSurface _slide;
    public float VerticalVelocity;

    public Vector3 Velocity { get; private set; }
    public Speed Speed { get; private set; }
    public MovementState State { get; private set; }
    public ExteranlVelocityAccumulator VelocityAccumulator { get; private set; }

    private void Awake()
    {
        var frameRate = 144;
        Application.targetFrameRate = frameRate;
        Time.fixedDeltaTime = 1f / frameRate;

        _body = BodyBuilder.Create(gameObject);
        _collision = CollisionBuilder.Create(gameObject, _body, MovementConfig);
        VelocityAccumulator = new();
        _slide = new SlideAlongSurface(_collision, MovementConfig);
        State = new MovementState(_body, _collision, MovementConfig);
        Speed = new Speed(State, MovementConfig);
    }

    private void Update()
    {
        _direction = Vector2.zero;

        if (Input.GetKey(KeyCode.D))
            _direction += Vector3.right;

        if (Input.GetKey(KeyCode.A))
            _direction += Vector3.left;

        if (Input.GetKey(KeyCode.S))
            _direction += Vector3.back;

        if (Input.GetKey(KeyCode.W))
            _direction += Vector3.forward;

        if (Input.GetKeyDown(KeyCode.Space))
            Jump();

        _direction = _direction.normalized;
    }

    private void FixedUpdate()
    {
        Move(Time.fixedDeltaTime);
    }

    public void Jump()
    {
        VerticalVelocity = MovementConfig.JumpSpeed;
        State.SetJumping();
    }

    private void Move(float deltaTime)
    {
        var pos = _body.Position + VelocityAccumulator.TotalVelocity * deltaTime;
        _body.Position = pos;
        _collision.Depenetrate();
        VerticalVelocity = Speed.CalculateVerticalSpeed(VerticalVelocity, deltaTime);

        var velocity = CalculateVelocity(_body.Position, deltaTime);
        Velocity = velocity / deltaTime;
        var nextPos = _body.Position + velocity;
        _body.Position = nextPos;
        
        State.Update(velocity);
    }

    private Vector3 CalculateVelocity(Vector3 pos, float deltaTime)
    {
        var totalVelocity = CalculateHorizontalVelocity(pos, deltaTime);
        var nextPosAlongSurface = pos + totalVelocity;
        totalVelocity += CalculateVerticalVelocity(nextPosAlongSurface, deltaTime);

        return totalVelocity;
    }

    private Vector3 CalculateHorizontalVelocity(Vector3 pos, float deltaTime)
    {
        var horVelocity = Speed.CalculateHorizontalSpeed(_direction, deltaTime) * deltaTime;
        var vel = _slide.SlideByMovement_recursive(horVelocity, pos);

        return vel;
    }

    private Vector3 CalculateVerticalVelocity(Vector3 pos, float deltaTime)
    {
        var vertVel = Vector3.up * VerticalVelocity * deltaTime;
        var vel = _slide.SlideByGravity_recursive(vertVel, pos);

        return vel;
    }
}
