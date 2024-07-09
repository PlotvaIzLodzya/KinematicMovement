using UnityEngine;
using UnityEngine.UIElements;

public class Movement : MonoBehaviour
{
    [SerializeField] private LayerMask _groundMask;
    [SerializeField] private float _speed = 15f;
    [SerializeField] private float _dist = 0.015f;
    [SerializeField] private float _jumpHeight = 2f;
    [SerializeField] private float _jumpTime = 0.1f;
    [SerializeField] private float _maxSlopeAngle = 45f;

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
        Time.fixedDeltaTime = 1f /frameRate;        
        _rb = BodyBuilder.Create(gameObject);
        _collision = CollisionBuilder.Create(gameObject);
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

        Move(Time.deltaTime);
    }

    private void FixedUpdate()
    {
        //Move(Time.fixedDeltaTime);
    }

    private void Move(float deltaTime)
    {
        var pos = transform.position + VelocityAccumalator.TotalVelocity * deltaTime;
        SetPosition(pos);
        HandleOverlap();
        VerticalVelocity = CalculateVerticalSpeed(VerticalVelocity, deltaTime);

        var totalVelocity = CalculateVelocity(transform.position, deltaTime);
        var nextPos = transform.position + totalVelocity;
        SetPosition(nextPos);

        UpdateState(totalVelocity);
    }

    private void HandleOverlap()
    {
        var counter = 0;
        var hit = _collision.GetHit(transform.position, Vector2.zero, 0f, _groundMask);
        if (hit.HaveHit)
        {
            while (counter < 5)
            {
                hit = _collision.GetHit(transform.position, Vector2.zero, 0f, _groundMask);
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
        var wasGrounded = Grounded;
        var wasOnTooSteepSlope = OnTooSteepSlope;
        Grounded = velCheck || downCheck;
        OnTooSteepSlope = IsOnTooSteepSlope();
        var exitSteepSlope = wasOnTooSteepSlope && OnTooSteepSlope == false;
        BecomeGrounded = Grounded && (wasGrounded == false || exitSteepSlope);
    }

    private Vector3 CalculateVelocity(Vector3 pos, float deltaTime)
    {
        var totalVelocity = CalculateHorizontalVelocity(pos, deltaTime);
        totalVelocity = AlignToSurface(totalVelocity);
        var nextPosAlongSurface = pos + totalVelocity;
        totalVelocity += CalculateVerticalVelocity(nextPosAlongSurface, deltaTime);

        return totalVelocity;
    }

    private void SetPosition(Vector3 pos)
    {
        transform.position = pos;
        _rb.Position = pos;
    }

    private Vector3 CalculateHorizontalVelocity(Vector3 pos, float deltaTime)
    {
        var horVelocity = _direction * _speed;
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
            vel = Vector3.ProjectOnPlane(vel, _groundHit.normal);
        }

        return vel;
    }

    public Vector3 CollideAndSlide_recursive(Vector3 vel, Vector3 currentPos, bool gravity, int currentDepth = 0)
    {
        if (currentDepth >= 5)
            return Vector3.zero;

        float dist = vel.magnitude;
        var dir = vel.normalized;

        var hit = _collision.GetHit(currentPos, dir, dist, _groundMask);
        var hitDist = hit.distance;

        float angle = Vector3.Angle(Vector3.up, hit.normal);
        var tooStep = IsSlopeTooSteep(angle);
        if (tooStep && gravity == false)
        {
            var dis = hit.ColliderDistance;
            return vel.normalized * (dis);
        }

        if (gravity && tooStep == false)
        {
            currentDepth=5;
        }

        if (hit.HaveHit)
        {
            var velToNextStep = dir * (hitDist - _dist);
            var leftOverVel = vel - velToNextStep;
            var nextPos = currentPos + velToNextStep;

            var projectedleftOverVel = Vector3.ProjectOnPlane(leftOverVel, hit.normal);

            vel = velToNextStep + CollideAndSlide_recursive(projectedleftOverVel, nextPos, gravity, ++currentDepth);
            return vel;
        }

        return vel;
    }



    private void Jump()
    {
        VerticalVelocity = CalculateJumpSpeed();
    }

    public float CalculateJumpSpeed()
    {
        var acceleration = CalculateVerticalAcceleration();
        var speed = Mathf.Sqrt(2 * acceleration * _jumpHeight);

        return speed;
    }

    public float CalculateVerticalAcceleration()
    {
        return _jumpHeight / (_jumpTime * _jumpTime);
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
        return angle >= _maxSlopeAngle;
    }

    private float GetGroundAngle()
    {
        var hit = _groundHit;
        float angle = Vector3.Angle(Vector3.up, hit.normal);

        return angle;
    }

    private bool Check(Vector3 dir, Vector3 currentPos)
    {
        var hit = _collision.GetHit(currentPos, dir, _dist, _groundMask);
        if (hit.HaveHit)
        {
            if (currentPos.y - hit.point.y > 0.1f)
            {
                _groundHit = hit;
                return true;
            }

            Debug.DrawRay(hit.point, Vector2.right, Color.red, 0.1f);
        }

        return false;
    }

    private Vector3 ScaleHorizontalVelocity(Vector3 vel, Vector3 projectedVel, Vector3 surfaceNormal)
    {
        surfaceNormal.y = 0;
        float scale = 1 + Vector3.Dot(vel.normalized, surfaceNormal.normalized);
        //Debug.Log($" surface: {surfaceNormal}, vel {vel.normalized} scale: {scale}");
        var scaledVel = projectedVel * scale;

        return scaledVel;
    }
}
