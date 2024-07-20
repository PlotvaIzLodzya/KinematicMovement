using UnityEngine;

public class Movement : MonoBehaviour
{
    [field: SerializeField] public MovementConfig MovementConfig { get; private set; }

    public bool Grounded;
    public bool OnTooSteepSlope;
    public bool BecomeGrounded;
    private Vector3 _direction;
    private HitInfo _groundHit;
    private IBody _rb;
    private ICollision _collision;
    public float VerticalVelocity;

    public ExteranlVelocityAccumalator VelocityAccumalator { get; private set; }

    private void Awake()
    {
        var frameRate = 144;
        Application.targetFrameRate = frameRate;
        Time.fixedDeltaTime = 1f / frameRate;

        _rb = BodyBuilder.Create(gameObject);
        _collision = CollisionBuilder.Create(gameObject, MovementConfig);
        VelocityAccumalator = new();
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

        _direction = _direction.normalized;
        if (Input.GetKeyDown(KeyCode.Space))
            Jump();
    }

    private void FixedUpdate()
    {
        Move(Time.fixedDeltaTime);
    }

    private void Move(float deltaTime)
    {
        var pos = transform.position + VelocityAccumalator.TotalVelocity * deltaTime;
        SetPosition(pos);
        Depenetrate();
        VerticalVelocity = CalculateVerticalSpeed(VerticalVelocity, deltaTime);

        var totalVelocity = CalculateVelocity(transform.position, deltaTime);
        var nextPos = transform.position + totalVelocity;
        SetPosition(nextPos);
        
        UpdateState(totalVelocity);
    }

    private void Depenetrate()
    {
        var counter = 0;
        var hit = _collision.GetHit();
        if (hit.HaveHit)
        {
            while (counter < 5)
            {
                hit = _collision.GetHit();
                if (hit.HaveHit)
                {
                    var targetPos = _collision.GetClosestPositionTo(hit);
                    SetPosition(targetPos);
                }
                counter++;
            }
        }
    }

    private void UpdateState(Vector3 totalVelocity)
    {
        bool velCheck = Check(totalVelocity.normalized + Vector3.down, transform.position);
        bool downCheck = Check(Vector3.down, transform.position);
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

        if(totalVelocity.y<0)
            totalVelocity = AlignToSurface(totalVelocity);

        return totalVelocity;
    }

    private void SetPosition(Vector3 pos)
    {
        transform.position = pos;
        _rb.Position = pos;
    }

    private Vector3 CalculateHorizontalVelocity(Vector3 pos, float deltaTime)
    {
        var horVelocity = _direction * MovementConfig.Speed;
        var vel = CollideAndSlide_recursive(horVelocity * deltaTime, transform.position, false);

        return vel;
    }

    private Vector3 CalculateVerticalVelocity(Vector3 pos, float deltaTime)
    {
        var vertVel = Vector3.up * VerticalVelocity;
        var vel = CollideAndSlide_recursive(vertVel * deltaTime, pos, true);
        return vel;
    }

    public float CalculateVerticalSpeed(float currentSpeed, float deltaTime)
    {
        var vertAccel = 0f;
        if (Grounded == false || OnTooSteepSlope)
        {
            vertAccel = CalculateVerticalAcceleration();
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

    public Vector3 CollideAndSlide_recursive(Vector3 vel, Vector3 currentPos, bool gravity, int currentDepth = 0)
    {
        if (currentDepth >= 5)
            return Vector3.zero;

        float dist = vel.magnitude;
        var dir = vel.normalized;

        var hit = _collision.GetHit(currentPos, dir, dist);
        var hitDist = hit.Distance;

        float angle = Vector3.Angle(Vector3.up, hit.Normal);
        var tooStep = IsSlopeTooSteep(angle);
        if (tooStep && gravity == false)
        {
            var dis = hit.ColliderDistance;
            vel = vel.normalized * dis;
        }

        if (gravity && tooStep == false)
        {
            currentDepth = 5;
        }
        if (hit.HaveHit)
        {
            var velToNextStep = dir * (hitDist - MovementConfig.ContactOffset);
            var leftOverVel = vel - velToNextStep;
            var nextPos = currentPos + velToNextStep;

            var projectedleftOverVel = Vector3.ProjectOnPlane(leftOverVel, hit.Normal);

            vel = velToNextStep + CollideAndSlide_recursive(projectedleftOverVel, nextPos, gravity, ++currentDepth);
            return vel;
        }

        return vel;
    }

    public void Jump()
    {
        VerticalVelocity = CalculateJumpSpeed();
    }

    public float CalculateJumpSpeed()
    {
        var acceleration = CalculateVerticalAcceleration();
        var speed = Mathf.Sqrt(2 * acceleration * MovementConfig.JumpHeight);

        return speed;
    }

    public float CalculateVerticalAcceleration()
    {
        return MovementConfig.JumpHeight / (MovementConfig.JumpTime * MovementConfig.JumpTime);
    }

    public bool IsOnTooSteepSlope()
    {
        if (Grounded == false)
            return false;

        var angle = GetGroundAngle();
        return IsSlopeTooSteep(angle);
    }

    private bool IsSlopeTooSteep(float angle)
    {
        return angle >= MovementConfig.MaxSlopeAngle;
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
