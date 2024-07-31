using UnityEngine;

public class VelocityCompute
{
    private Vector3 _velocityHorizontal;
    private float _speedVertical;
    private Vector3 _minVelocity;
    private IMovementState _state;
    private MovementConfig MovementConfig;

    public Vector3 Velocity
    {
        get
        {
            var totalVelocity = _velocityHorizontal;
            totalVelocity.y = _speedVertical;
            return totalVelocity;
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
            _velocityHorizontal = Vector3.zero;
            return Vector3.zero;
        }

        if (dir.sqrMagnitude > 0)
            _velocityHorizontal = Vector3.MoveTowards(_velocityHorizontal, maxVel, MovementConfig.Acceleration * deltaTime);
        else
            _velocityHorizontal = Vector3.MoveTowards(_velocityHorizontal, _minVelocity, MovementConfig.Decceleration * deltaTime);

        return _velocityHorizontal;
    }

    public void SetVerticalSpeed(float speed)
    {
        _speedVertical = speed;
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
            _speedVertical = -9.8f;
        }

        if (_state.BecomeCeiled && _state.IsJumping)
        {
            _speedVertical = 0f;
        }

        _speedVertical -= vertAccel * deltaTime;
        _speedVertical = Mathf.Clamp(_speedVertical, MovementConfig.MinVerticalSpeed, MovementConfig.MaxVertiaclSpeed);
        return _speedVertical;
    }
}
