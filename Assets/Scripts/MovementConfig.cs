using System;
using UnityEngine;

[Serializable]
public class MovementConfig: ILayerMaskProvider
{
    public const float ContactOffset = 0.015f;
    public const float CollisionCheckDistance = ContactOffset * 1.2f;
    public const float MinDistanceTravel = 0.0001f;

    public const float MinVerticalSpeed = -40f;
    public const float MaxVertiaclSpeed = 100f;

    [field: SerializeField] public LayerMask GroundMask { get; private set; }
    [field: SerializeField] public float Speed { get; private set; }
    [field: SerializeField] public float AccelerationTime { get; private set; }
    [field: SerializeField] public float DeccelerationTime { get; private set; }
    [field: SerializeField] public float JumpHeight { get; private set; }
    [field: SerializeField] public float JumpTime { get; private set; }
    [field: SerializeField] public float MaxSlopeAngle { get; private set; }

    public float Acceleration => Speed / AccelerationTime;
    public float Decceleration => Speed / DeccelerationTime;
    public float VerticalAcceleration => JumpHeight / (JumpTime * JumpTime);
    public float JumpSpeed => Mathf.Sqrt(2 * VerticalAcceleration * JumpHeight);

    public MovementConfig()
    {
       Speed = 15f;
       JumpHeight = 2f;
       JumpTime = 0.2f;
       MaxSlopeAngle = 45f;
       AccelerationTime = 0.3f;
       DeccelerationTime = 0.05f;
    }

    public MovementConfig(LayerMask groundMask, float speed, float jumpHeight, float jumpTime, float maxSlopeAngle)
    {
        GroundMask = groundMask;
        Speed = speed;
        JumpHeight = jumpHeight;
        JumpTime = jumpTime;
        MaxSlopeAngle = maxSlopeAngle;
    }

    public bool IsSlopeTooSteep(float angle)
    {
        return angle >= MaxSlopeAngle;
    }
}
