using UnityEngine;

public class Movement : MonoBehaviour
{
    [SerializeField] private LayerMask _groundMask;
    [SerializeField] private float _vertSpeed;
    [SerializeField] private float _speed;
    [SerializeField] private float _dist;
    [SerializeField] private float _jumpHeight;
    [SerializeField] private float _jumpTime;
    [SerializeField] private float _maxSlopeAngle;

    public bool Grounded;
    public bool OnTooSteepSlope;
    public bool BecomeGrounded;
    private Vector2 _direction;
    private RaycastHit2D _groundHit;
    private CircleCollider2D _collider;
    private Rigidbody2D _rb;
    public float VerticalVelocity;

    public ExteranlVelocityAccumalator VelocityAccumalator { get; private set; }

    private void Awake()
    {
        var frameRate = 144;
        Application.targetFrameRate = frameRate;
        Time.fixedDeltaTime = 1f /frameRate;        
        _collider = GetComponent<CircleCollider2D>();
        _rb = GetComponent<Rigidbody2D>();
        VelocityAccumalator = new();
    }

    private void Update()
    {
        _direction = Vector2.zero;

        if (Input.GetKey(KeyCode.D))
            _direction = Vector2.right;

        if (Input.GetKey(KeyCode.A))
            _direction = Vector2.left;

        if (Input.GetKeyDown(KeyCode.Space))
            Jump();
    }

    private void FixedUpdate()
    {
        Move(Time.fixedDeltaTime);
    }

    private void Move(float deltaTime)
    {
        transform.position += VelocityAccumalator.TotalVelocity * deltaTime;
        _rb.position += (Vector2)VelocityAccumalator.TotalVelocity * deltaTime;
        var counter = 0;
        var hit = GetHit(transform.position, _collider.radius, Vector2.zero, 0f, _groundMask);
        var haveHit = hit.collider != null;
        if(haveHit)
        {
            while (counter < 5)
            {
                hit = GetHit(transform.position, _collider.radius, Vector2.zero, 0f, _groundMask);
                haveHit = hit.collider != null;
                if (haveHit)
                {
                    //var colDist = hit.collider.Distance(_collider);
                    //var hitDist = colDist.distance + _dist;
                    //var targetPos = transform.position - (Vector3)colDist.normal * hitDist;
                    var targetPos = GetClosestPosition(hit);
                    _rb.position = targetPos;
                    transform.position = targetPos;
                }
                counter++;
            }
        }

        var vertAccel = 0f;
        if (Grounded == false || OnTooSteepSlope)
        {
            vertAccel = CalculateVerticalAcceleration();
        }

        if (BecomeGrounded)
        {
            VerticalVelocity = -9.8f;
        }

        VerticalVelocity -= vertAccel * deltaTime;
        VerticalVelocity = Mathf.Clamp(VerticalVelocity, -40, 100);
        var vertVel = Vector3.up * VerticalVelocity;
        var horVelocity = _direction * _speed;
        var vel = CollideAndSlide_recursive(horVelocity * deltaTime, transform.position, false);
        vel = AlignToSurface(vel);
        var nextPosAlongSurface = transform.position + vel;
        vel += CollideAndSlide_recursive(vertVel * deltaTime, nextPosAlongSurface, true);

        var nextPos = transform.position + vel;
        _rb.position = nextPos;
        transform.position = nextPos;

        bool velCheck = Check(vel.normalized+Vector3.down, transform.position);
        bool downCheck = Check(Vector3.down, transform.position);
        var wasGrounded = Grounded;
        var wasOnTooSteepSlope = OnTooSteepSlope;
        Grounded = velCheck || downCheck;
        OnTooSteepSlope = IsOnTooSteepSlope();
        var exitSteepSlope = wasOnTooSteepSlope && OnTooSteepSlope == false;
        BecomeGrounded = Grounded && (wasGrounded == false || exitSteepSlope);
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

        var hit = GetHit(currentPos, _collider.radius, dir, dist, _groundMask);
        var hitDist = hit.distance;

        float angle = Vector3.Angle(Vector3.up, hit.normal);
        var tooStep = IsSlopeTooSteep(angle);
        if (tooStep && gravity == false)
        {
            var dis = GetDistance(hit);
            return vel.normalized * (dis);
        }

        if (gravity && tooStep == false)
        {
            currentDepth=5;
        }

        if (hit.collider != null)
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
        var hit = GetHit(dir, currentPos);
        var haveHit = hit.collider != null;
        if (haveHit)
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

    public RaycastHit2D GetHit(Vector3 pos, float radius, Vector3 dir, float dist, LayerMask mask)
    {
        return Physics2D.CircleCast(pos, radius, dir, dist, mask);

        //Physics.SphereCast(pos, radius, dir, out RaycastHit hit, dist, mask);
        //return hit;      
    }

    public float GetDistance(Vector3 pos, float radius, Vector3 dir, float dist, LayerMask mask)
    {
        var hit = GetHit(pos, radius, dir, dist, mask);
        var distance = GetDistance(hit);
        return distance;
    }

    public float GetDistance(RaycastHit2D hit)
    {
        var d = hit.collider.Distance(_collider);
        var dis = d.distance;
        return dis;
    }

    public Vector3 GetClosestPosition(RaycastHit2D hit)
    {
        var deltaPos = GetDeltaPositionToHit(hit);
        var targetPos = transform.position - deltaPos;

        return targetPos;
    }

    public Vector3 GetDeltaPositionToHit(RaycastHit2D hit)
    {
        var colDist = hit.collider.Distance(_collider);
        var hitDist = GetDistance(hit);
        var deltaPos = (Vector3)colDist.normal * hitDist;
        return deltaPos;
    }


    private RaycastHit2D GetHit(Vector3 dir, Vector3 currentPos)
    {
        return Physics2D.CircleCast(currentPos, _collider.radius, dir, _dist, _groundMask);
    }
}
