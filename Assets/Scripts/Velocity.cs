using UnityEngine;

public class Velocity
{
 
    private Vector3 _minVelocity;
    private IMovementState _state;
    private MovementConfig MovementConfig;

    public Vector3 Horizontal { get; private set; }

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
            Horizontal = Vector3.zero;
            return Vector3.zero;
        }

        if (dir.sqrMagnitude > 0)
            Horizontal = Vector3.MoveTowards(Horizontal, maxVel, MovementConfig.Acceleration*deltaTime);
        else
            Horizontal = Vector3.MoveTowards(Horizontal, _minVelocity, MovementConfig.Decceleration * deltaTime);

        return Horizontal;
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
            currentSpeed = 0;
        }

        if (_state.BecomeCeiled && _state.IsJumping)
        {
            currentSpeed = 0;
        }

        currentSpeed -= vertAccel * deltaTime;
        currentSpeed = Mathf.Clamp(currentSpeed, MovementConfig.MinVerticalSpeed, MovementConfig.MaxVertiaclSpeed);
        return currentSpeed;
    }
}
