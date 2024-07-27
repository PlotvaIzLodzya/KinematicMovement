using UnityEngine;

public class Velocity
{
    private const float MinVerticalSpeed = -40;
    private const float MaxVertiaclSpeed = 100;
    private const float GroundedVerticalSpeed = -9.8f;

    private Vector3 _currentVelocity;
    private Vector3 _minVelocity;
    private IMovementState _state;
    private MovementConfig MovementConfig;

    public Velocity(IMovementState state, MovementConfig movementConfig)
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
            _currentVelocity = Vector3.zero;
            return Vector3.zero;
        }

        if (dir.sqrMagnitude > 0)
            _currentVelocity = Vector3.MoveTowards(_currentVelocity, maxVel, MovementConfig.Acceleration*deltaTime);
        else
            _currentVelocity = Vector3.MoveTowards(_currentVelocity, _minVelocity, MovementConfig.Decceleration*deltaTime);

        return _currentVelocity;
    }

    public float CalculateVerticalSpeed(float currentSpeed, float deltaTime)
    {
        var vertAccel = 0f;
        if (_state.Grounded == false || _state.OnTooSteepSlope)
        {
            vertAccel = MovementConfig.VerticalAcceleration;
        }

        if (_state.LeftGround && _state.IsJumping == false)
        {
            currentSpeed = GroundedVerticalSpeed;
        }

        if (_state.BecomeCeiled && _state.IsJumping)
        {
            currentSpeed = 0f;
        }

        currentSpeed -= vertAccel * deltaTime;
        currentSpeed = Mathf.Clamp(currentSpeed, MinVerticalSpeed, MaxVertiaclSpeed);

        return currentSpeed;
    }
}
