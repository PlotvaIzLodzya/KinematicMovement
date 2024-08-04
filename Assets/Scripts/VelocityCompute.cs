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
    private KeepMomentumAirborneVelocity _airborneVelocity;
    private PlatformJumpVelocity _platformJumpVelocity;
    private IVelocityCompute _current;

    public VelocityHandler(MovementState state, MovementConfig movementConfig, IPlatformProvider provider)
    {
        _state = state;
        _velocity = new VelocityCompute(state, movementConfig);
        _airborneVelocity = new KeepMomentumAirborneVelocity(_velocity, state);
        _platformJumpVelocity = new PlatformJumpVelocity(_airborneVelocity, provider, movementConfig);
    }

    public IVelocityCompute GetVelocityCompute<T>() where T : IVelocityCompute
    {        
        return true switch
        {
            true when typeof(T) == typeof(PlatformJumpVelocity) => _platformJumpVelocity,   
            true when typeof(T) == typeof(KeepMomentumAirborneVelocity) => _airborneVelocity,   
            true when typeof(T) == typeof(VelocityCompute) => _velocity,
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
    private KeepMomentumAirborneVelocity _airborneVelocity;
    private MovementConfig _config;
    private Vector3 _velocity;
    private float _maxMagnitude;
    private Vector3 _minVelocity;
    
    public Vector3 Velocity => _velocity;

    public PlatformJumpVelocity(KeepMomentumAirborneVelocity airborneVelocity, IPlatformProvider platformProvider, MovementConfig config)
    {
        _minVelocity = Vector3.zero;
        _airborneVelocity = airborneVelocity;
        _platformProvider = platformProvider;
        _config = config;
    }


    public Vector3 CalculateHorizontalSpeed(Vector3 dir, float deltaTime)
    {
        var maxVel = dir * _maxMagnitude;
        var vel = Velocity.Horizontal();

        //if (dir.sqrMagnitude > 0)
        //    _velocity = Vector3.MoveTowards(vel, maxVel, _config.Acceleration * deltaTime);
        //else
        //    _velocity = Vector3.MoveTowards(vel, _minVelocity, _config.Decceleration * deltaTime);

        return _airborneVelocity.CalculateHorizontalSpeed(dir, deltaTime);
        return vel;
    }

    public float CalculateVerticalSpeed(float deltaTime)
    {
        var vel = _velocity;
        vel.y = _airborneVelocity.CalculateVerticalSpeed(deltaTime);
        _velocity = vel;
        return _velocity.y;
    }

    public void Enter(Vector3 currentVelocity)
    {
        var platformVelocity = _platformProvider.Platform.UnscaledVelocity;
        _velocity = currentVelocity + platformVelocity;
        _maxMagnitude = platformVelocity.magnitude + 15f;
    }

    public void Exit()
    {
        _velocity = Vector3.zero;
    }

    public void Jump(float speed)
    {
        _airborneVelocity.Jump(speed);
    }
}

public class KeepMomentumAirborneVelocity : IVelocityCompute
{
    private VelocityCompute _velocity;
    private IJumpState _state;

    public Vector3 Velocity => _velocity.Velocity;

    public KeepMomentumAirborneVelocity(VelocityCompute velocity, IJumpState state)
    {
        _velocity = velocity;
        _state = state;
    }

    public Vector3 CalculateHorizontalSpeed(Vector3 dir, float deltaTime)
    {
        if (dir.sqrMagnitude > 0)
            return _velocity.CalculateHorizontalSpeed(dir, deltaTime);
        else
            return _velocity.Velocity.Horizontal();
        //return _velocity.CalculateHorizontalSpeed(dir, deltaTime);
    }

    public float CalculateVerticalSpeed(float deltaTime)
    {
        return _velocity.CalculateVerticalSpeed(deltaTime);
    }

    public void Enter(Vector3 currentVelocity)
    {
        
    }

    public void Exit()
    {
        
    }

    public void Jump(float speed)
    {
        _velocity.Jump(speed);
    }
}

public class VelocityCompute: IVelocityCompute
{
    private Vector3 _minVelocity;
    private IMovementState _state;
    private MovementConfig MovementConfig;
    private Vector3 _velocity;
    private float _vertSpeed;
    public Vector3 Velocity
    {
        get
        {
            var totalVel = _velocity;
            totalVel.y = _vertSpeed;
            return totalVel;
        }
    }

    public VelocityCompute(IMovementState state, MovementConfig movementConfig)
    {
        _state = state;
        _minVelocity = Vector3.zero;
        MovementConfig = movementConfig;
    }

    public Vector3 CalculateHorizontalSpeed(Vector3 dir, float deltaTime)
    {
        var maxVel = dir * MovementConfig.Speed;
        if (_state.CrashedIntoWall)
        {            
            return Vector3.zero;
        }

        var hor = _velocity.Horizontal();
        if (dir.sqrMagnitude > 0)
            _velocity = Vector3.MoveTowards(hor, maxVel, MovementConfig.Acceleration * deltaTime);
        else
            _velocity = Vector3.MoveTowards(hor, _minVelocity, MovementConfig.Decceleration * deltaTime);

        return _velocity;
    }

    public float CalculateVerticalSpeed(float deltaTime)
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

    public void Enter(Vector3 currentVelocity)
    {
        _velocity = currentVelocity;
    }

    public void Exit()
    {
        
    }

    public void Jump(float speed)
    {
        _vertSpeed = speed;
    }
}
