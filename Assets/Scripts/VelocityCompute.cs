using System;
using UnityEngine;

public interface IVelocityCompute
{
    Vector3 Velocity { get; }

    void Jump(float speed);
    Vector3 CalculateHorizontalSpeed(Vector3 dir, float deltaTime);
    float CalculateVerticalSpeed(float deltaTime);
    void Exit();
    void Enter(Vector3 currentVelocity);
}

public class VelocityHandler
{
    private MovementState _state;
    private VelocityCompute _velocity;
    private AiborneVelocityCompute _airborneVelocity;
    private PlatformJumpVelocity _platformJumpVelocity;
    private IVelocityCompute _current;

    public VelocityHandler(MovementState state, MovementConfig movementConfig, IPlatformProvider provider)
    {
        _state = state;
        _velocity = new VelocityCompute(state, movementConfig);
        _airborneVelocity = new FullControlAirborneVelocity(state, movementConfig);
        _platformJumpVelocity = new PlatformJumpVelocity(_airborneVelocity, provider, movementConfig);
    }

    public IVelocityCompute GetVelocityCompute<T>() where T : IVelocityCompute
    {        
        return true switch
        {
            true when typeof(T) == typeof(VelocityCompute) => _velocity,
            true when typeof(T) == typeof(PlatformJumpVelocity) => _platformJumpVelocity,   
            true when typeof(T) == typeof(AiborneVelocityCompute) => _airborneVelocity,   
            _ => throw new System.NotImplementedException(),
        };
    }

    public IVelocityCompute GetVelocityCompute()
    {
        var prev = _current;

        if (_state.IsOnPlatform && _state.IsJumping)
            _current = _platformJumpVelocity;
        else if (_state.Grounded)
            _current = _velocity;
        else if(_state.Grounded == false)
            _current = _airborneVelocity;
        
        if (_current != prev)
        {
            prev?.Exit();
            var prevVel = prev == null ? Vector3.zero : prev.Velocity;
            _current.Enter(prevVel);
        }

        return _current;
    }
}

public class PlatformJumpVelocity : IVelocityCompute
{
    private IPlatformProvider _platformProvider;
    private AiborneVelocityCompute _airborneVelocity;
    
    public Vector3 Velocity => _airborneVelocity.Velocity;

    public PlatformJumpVelocity(AiborneVelocityCompute airborneVelocity, IPlatformProvider platformProvider, MovementConfig config)
    {
        _airborneVelocity = airborneVelocity;
        _platformProvider = platformProvider;
    }

    public Vector3 CalculateHorizontalSpeed(Vector3 dir, float deltaTime)
    {
        return _airborneVelocity.CalculateHorizontalSpeed(dir, deltaTime);        
    }

    public float CalculateVerticalSpeed(float deltaTime)
    {
        return _airborneVelocity.CalculateVerticalSpeed(deltaTime);
    }

    public void Enter(Vector3 currentVelocity)
    {
        var platformVelocity = _platformProvider.Platform.UnscaledVelocity;
        _airborneVelocity.Enter(currentVelocity + platformVelocity);
        _airborneVelocity.AddMaxHorSpeed(platformVelocity.magnitude);
    }

    public void Exit()
    {
        _airborneVelocity.Exit();
    }

    public void Jump(float speed)
    {
        _airborneVelocity.Jump(speed);
    }
}

public class FullControlAirborneVelocity : AiborneVelocityCompute
{
    public FullControlAirborneVelocity(IMovementState state, MovementConfig movementConfig) : base(state, movementConfig)
    {
    }
}

public class KeepMomentumAirborneVelocity : AiborneVelocityCompute
{
    public KeepMomentumAirborneVelocity(IMovementState state, MovementConfig movementConfig) : base(state, movementConfig)
    {
    }

    public override Vector3 CalculateHorizontalSpeed(Vector3 dir, float deltaTime)
    {
        if (dir.sqrMagnitude > 0)
            return base.CalculateHorizontalSpeed(dir, deltaTime);
        else
            return Velocity.Horizontal();
    }
}

public class AiborneVelocityCompute : VelocityCompute
{
    private float _addedSpeed;

    protected override float MaxHorizontalSpeed => base.MaxHorizontalSpeed + _addedSpeed;

    public AiborneVelocityCompute(IMovementState state, MovementConfig movementConfig) : base(state, movementConfig)
    {
    }

    public void AddMaxHorSpeed(float maxHorSpeed)
    {
        _addedSpeed = maxHorSpeed;
    }

    public override void Exit()
    {
        _addedSpeed = 0f;
        if(Direction.sqrMagnitude == 0)
            Velocity = Vector3.zero;

        base.Exit();
    }
}

public class VelocityCompute: IVelocityCompute
{
    private Vector3 _minVelocity;
    private Vector3 _velocity;
    private IMovementState _state;
    private MovementConfig MovementConfig;
    private float _vertSpeed;

    protected Vector3 Direction { get; set; }
    protected virtual float MaxHorizontalSpeed => MovementConfig.Speed;

    public Vector3 Velocity
    {
        get
        {
            var totalVel = _velocity;
            totalVel.y = _vertSpeed;
            return totalVel;
        }
        protected set
        {
            _velocity = value;
            _vertSpeed = value.y;
        }
    }

    public VelocityCompute(IMovementState state, MovementConfig movementConfig)
    {
        _state = state;
        _minVelocity = Vector3.zero;
        MovementConfig = movementConfig;
    }

    public virtual Vector3 CalculateHorizontalSpeed(Vector3 dir, float deltaTime)
    {
        var maxVel = dir * MaxHorizontalSpeed;
        Direction = dir;
        if (_state.CrashedIntoWall)
        {
            return Vector3.zero;
        }
        Debug.Log(_state.HaveWallCollision);
        var hor = _velocity.Horizontal();
        if (dir.sqrMagnitude > 0)
            _velocity = Vector3.MoveTowards(hor, maxVel, MovementConfig.Acceleration * deltaTime);
        else
            _velocity = Vector3.MoveTowards(hor, _minVelocity, MovementConfig.Decceleration * deltaTime);
        
        return _velocity;
    }

    public virtual float CalculateVerticalSpeed(float deltaTime)
    {
        var vertAccel = 0f;
        if (_state.Grounded == false || _state.OnTooSteepSlope)
        {
            vertAccel = MovementConfig.VerticalAcceleration;
        }

        if (_state.LeftGround && _state.IsJumping == false)
        {
            _vertSpeed = -9.8f;
        }

        if (_state.BecomeCeiled && _state.IsJumping)
        {
            _vertSpeed = 0f;
        }

        _vertSpeed -= vertAccel * deltaTime;
        _vertSpeed = Mathf.Clamp(_vertSpeed, MovementConfig.MinVerticalSpeed, MovementConfig.MaxVertiaclSpeed);

        return _vertSpeed;
    }

    public virtual void Enter(Vector3 currentVelocity)
    {
        Velocity = currentVelocity;
    }

    public virtual void Exit()
    {
        Direction = Vector3.zero;
    }

    public virtual void Jump(float speed)
    {
        _vertSpeed = speed;
    }
}
