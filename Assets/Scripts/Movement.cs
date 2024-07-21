using UnityEngine;
public class Movement : MonoBehaviour
{
    [field: SerializeField] public MovementConfig MovementConfig { get; private set; }

    public bool Grounded;
    public bool OnTooSteepSlope;
    public bool BecomeGrounded;
    private Vector3 _direction;
    private HitInfo _groundHit;
    private IBody _body;
    private ICollision _collision;
    private SlideAlongSurface _slide;
    public float VerticalVelocity;

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

    private void Move(float deltaTime)
    {
        var pos = _body.Position + VelocityAccumulator.TotalVelocity * deltaTime;
        _body.Position = pos;
        _collision.Depenetrate();
        VerticalVelocity = CalculateVerticalSpeed(VerticalVelocity, deltaTime);

        var totalVelocity = CalculateVelocity(_body.Position, deltaTime);
        var nextPos = _body.Position + totalVelocity;
        _body.Position = nextPos;
        
        UpdateState(totalVelocity);
    }

    private void UpdateState(Vector3 totalVelocity)
    {
        bool velCheck = Check(totalVelocity.normalized + Vector3.down, _body.Position);
        bool downCheck = Check(Vector3.down, _body.Position);
        var wasGrounded = velCheck || Grounded;
        var wasOnTooSteepSlope = OnTooSteepSlope;
        Grounded = downCheck;
        OnTooSteepSlope = IsOnTooSteepSlope();
        var exitSteepSlope = wasOnTooSteepSlope && OnTooSteepSlope == false;
        BecomeGrounded = Grounded && (wasGrounded == false || exitSteepSlope);
    }

    private Vector3 CalculateVelocity(Vector3 pos, float deltaTime)
    {
        var totalVelocity = CalculateHorizontalVelocity(pos, deltaTime);
        var nextPosAlongSurface = pos + totalVelocity;
        totalVelocity += CalculateVerticalVelocity(nextPosAlongSurface, deltaTime);

        if(totalVelocity.y < 0)
            totalVelocity = AlignToSurface(totalVelocity);

        return totalVelocity;
    }

    private Vector3 CalculateHorizontalVelocity(Vector3 pos, float deltaTime)
    {
        var horVelocity = _direction * MovementConfig.Speed * deltaTime;
        var vel = _slide.SlideByMovement_recursive(horVelocity, _body.Position);

        return vel;
    }

    private Vector3 CalculateVerticalVelocity(Vector3 pos, float deltaTime)
    {
        var vertVel = Vector3.up * VerticalVelocity * deltaTime;
        var vel = _slide.SlideByGravity_recursive(vertVel, pos);
        return vel;
    }

    public float CalculateVerticalSpeed(float currentSpeed, float deltaTime)
    {
        var vertAccel = 0f;
        if (Grounded == false || OnTooSteepSlope)
        {
            vertAccel = MovementConfig.VerticalAcceleration;
        }

        if (BecomeGrounded)
        {
            //currentSpeed = -9.8f;
        }

        currentSpeed -= vertAccel * deltaTime;
        currentSpeed = Mathf.Clamp(currentSpeed, -40, 100);

        return currentSpeed;    
    }

    private Vector3 AlignToSurface(Vector3 vel)
    {
        if (Grounded)
        {
            vel = Vector3.ProjectOnPlane(vel, _groundHit.Normal);
        }

        return vel;
    }

    public void Jump()
    {
        VerticalVelocity = MovementConfig.JumpSpeed;
    }

    public bool IsOnTooSteepSlope()
    {
        if (Grounded == false)
            return false;

        var angle = GetGroundAngle();
        return MovementConfig.IsSlopeTooSteep(angle);
    }

    private float GetGroundAngle()
    {
        var hit = _groundHit;
        float angle = Vector3.Angle(Vector3.up, hit.Normal);

        return angle;
    }

    private bool Check(Vector3 dir, Vector3 currentPos)
    {
        var hit = _collision.GetHit(currentPos, dir, MovementConfig.GroundCheckDistance);
        if (hit.HaveHit)
        {
            if (currentPos.y - hit.Point.y > 0.1f)
            {
                _groundHit = hit;
                return true;
            }
        }

        return false;
    }
}
