using UnityEngine;

public interface IVelocityCompute
{
    Vector3 CalculateHorizontalSpeed(Vector3 dir, Vector3 horVelocity, float deltaTime);
    float CalculateVerticalSpeed(float verticalSpeed, float deltaTime);
}

public class VelocityHandler
{
    private IMovementState _state;
    private VelocityCompute _velocity;
    private AirborneVelocity _airborneVelocity;

    public VelocityHandler(IMovementState state, MovementConfig movementConfig)
    {
        _state = state;
        _velocity = new VelocityCompute(state, movementConfig);
        _airborneVelocity = new AirborneVelocity(_velocity);
    }

    public IVelocityCompute GetVelocity<T>() where T : IVelocityCompute
    {        
        return true switch
        {
            true when typeof(T) == typeof(VelocityCompute) => _velocity,
            true when typeof(T) == typeof(AirborneVelocity) => _airborneVelocity,   
            _ => throw new System.NotImplementedException(),
        };
    }

    public IVelocityCompute GetVelocity()
    {
        if (_state.Grounded)
            return _velocity;
            
        return _airborneVelocity;
    }
}

public class AirborneVelocity : IVelocityCompute
{
    public Vector3 Current { get; private set;}
    private VelocityCompute _velocity;

    public AirborneVelocity(VelocityCompute velocity)
    {
        _velocity = velocity;
    }

    public Vector3 CalculateHorizontalSpeed(Vector3 dir, Vector3 horVelocity, float deltaTime)
    {        
        return _velocity.CalculateHorizontalSpeed(dir,horVelocity, deltaTime);
    }

    public float CalculateVerticalSpeed(float verticalSpeed, float deltaTime)
    {
        return _velocity.CalculateVerticalSpeed(verticalSpeed, deltaTime);
    }
}

public class VelocityCompute: IVelocityCompute
{
    private Vector3 _minVelocity;
    private IMovementState _state;
    private MovementConfig MovementConfig;

    public VelocityCompute(IMovementState state, MovementConfig movementConfig)
    {
        _state = state;
        _minVelocity = Vector3.zero;
        MovementConfig = movementConfig;
    }

    public Vector3 CalculateHorizontalSpeed(Vector3 dir, Vector3 horizontalVelocity, float deltaTime)
    {
        var maxVel = dir * MovementConfig.Speed;

        if (_state.CrashedIntoWall)
        {            
            return Vector3.zero;
        }

        if (dir.sqrMagnitude > 0)
            horizontalVelocity = Vector3.MoveTowards(horizontalVelocity, maxVel, MovementConfig.Acceleration * deltaTime);
        else
            horizontalVelocity = Vector3.MoveTowards(horizontalVelocity, _minVelocity, MovementConfig.Decceleration * deltaTime);

        return horizontalVelocity;
    }

    public float CalculateVerticalSpeed(float verticalSpeed, float deltaTime)
    {
        var vertAccel = 0f;
        if (_state.Grounded == false || _state.OnTooSteepSlope)
        {
            vertAccel = MovementConfig.VerticalAcceleration;
        }

        if (_state.LeftGround && _state.IsJumping == false)
        {
            verticalSpeed = -9.8f;
        }

        if (_state.BecomeCeiled && _state.IsJumping)
        {
            verticalSpeed = 0f;
        }

        verticalSpeed -= vertAccel * deltaTime;
        verticalSpeed = Mathf.Clamp(verticalSpeed, MovementConfig.MinVerticalSpeed, MovementConfig.MaxVertiaclSpeed);
        return verticalSpeed;
    }
}
