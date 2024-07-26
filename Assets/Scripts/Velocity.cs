using UnityEngine;

public class Velocity
{
    private IMovementState _state;
    private MovementConfig MovementConfig;
    private Vector3 _currentSpeed;
    private Vector3 _minVelocity;

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
            _currentSpeed = Vector3.zero;
            return Vector3.zero;
        }

        if (dir.sqrMagnitude > 0)
            _currentSpeed = Vector3.MoveTowards(_currentSpeed, maxVel, MovementConfig.Acceleration*deltaTime);
        else
            _currentSpeed = Vector3.MoveTowards(_currentSpeed, _minVelocity, MovementConfig.Decceleration*deltaTime);

        return _currentSpeed;
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
            currentSpeed = -9.8f;
        }

        if (_state.BecomeCeiled && _state.IsJumping)
            currentSpeed = 0f;

        currentSpeed -= vertAccel * deltaTime;
        currentSpeed = Mathf.Clamp(currentSpeed, -40, 100);

        return currentSpeed;
    }
}
