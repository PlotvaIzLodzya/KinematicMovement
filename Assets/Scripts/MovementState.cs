using System;
using System.Net.NetworkInformation;
using Unity.VisualScripting;
using UnityEngine;


public interface IMovementState
{    
    bool HaveWallCollision { get; }
    bool CrashedIntoWall { get; }
    bool IsJumping { get; }
    bool LeftGround { get; }
    bool Grounded { get; }
    bool BecomeGrounded { get; }
    bool OnTooSteepSlope { get; }
    bool Ceiled { get; }
    bool BecomeCeiled { get; }
    Vector3 GroundNormal { get; }

    bool Check(Vector3 velocity);
    bool IsSlopeTooSteep(float angle);
}

public interface IExteranlMovementState
{
    bool IsEnteredPlatform { get; }
    bool IsOnPlatform { get; }
    bool IsLeftPlatform { get;}

    public bool TrySetOnPlatform(IPlatform platform);
    public void LeavePlatform(IPlatform platform);
}

[Serializable]
public class MovementState: IMovementState, IExteranlMovementState
{
    private bool _previousVelCheck;
    private HitInfo _groundHit;
    private IBody _body;
    private ICollision _collision;
    private MovementConfig _movementConfig;
    private bool _stickFromAnySide;

    public bool CanAccumulatePlafromVelocity { get; private set; }
    public bool IsOnPlatform { get; private set; }
    public bool IsEnteredPlatform { get; private set; }
    public bool IsLeftPlatform { get; private set; }
    public bool HaveWallCollision { get; private set; }
    public bool CrashedIntoWall { get; private set; }
    public bool IsJumping { get; private set; }
    public bool LeftGround { get; private set; }
    public bool Grounded { get; private set; }
    public bool BecomeGrounded { get; private set; }
    public bool OnTooSteepSlope { get; private set; }
    public bool Ceiled { get; private set; }
    public bool BecomeCeiled { get; private set; }
    public Vector3 GroundNormal => _groundHit.Normal;

    public MovementState(IBody body, ICollision collision, MovementConfig movementConfig)
    {
        _stickFromAnySide = false;
        _body = body;
        _collision = collision;
        _movementConfig = movementConfig;
    }

    public void Update(Vector3 movementDirection)
    {
        bool haveWallCollision = Check(movementDirection);
        bool velrGroundCheck = Check(movementDirection + Vector3.down, _body.Position);
        bool downCheck = Check(Vector3.down, _body.Position);
        bool upCheck = Check(Vector3.up, _body.Position);
        var wasGrounded = _previousVelCheck || Grounded;
        var wasOnTooSteepSlope = OnTooSteepSlope;

        CrashedIntoWall = haveWallCollision && HaveWallCollision == false;
        HaveWallCollision = haveWallCollision;

        BecomeCeiled = upCheck && Ceiled == false;
        Ceiled = upCheck;

        Grounded = downCheck;
        OnTooSteepSlope = IsOnTooSteepSlope();
        var exitSteepSlope = wasOnTooSteepSlope && OnTooSteepSlope == false;
        BecomeGrounded = Grounded && (wasGrounded == false || exitSteepSlope);
        LeftGround = wasGrounded && Grounded == false;
        _previousVelCheck = velrGroundCheck;

        if(BecomeGrounded)
            IsJumping = false;
    }

    public void SetJumping()
    {
        IsJumping = true;
    }

    public bool Check(Vector3 velocity)
    {
        return _collision.CheckDirection(velocity);
    }

    public bool TrySetOnPlatform(IPlatform platform)
    {
        if (IsOnPlatform)
            return false;

        if(CanSetOnPlatform(platform))
            IsOnPlatform = true;

        return IsOnPlatform;
    }

    public void LeavePlatform(IPlatform platform)
    {
        IsOnPlatform = false;
    }

    public bool CanSetOnPlatform( IPlatform platform)
    {
        return _stickFromAnySide || (platform.CollisionPoint.y - _groundHit.Point.y) <= MovementConfig.CollisionCheckDistance;
    }

    private bool Check(Vector3 dir, Vector3 currentPos)
    {
        var hit = _collision.GetHit(currentPos, dir, MovementConfig.CollisionCheckDistance);
        if (hit.HaveHit)
        {
            _groundHit = hit;
            return true;
        }

        return false;
    }

    public bool IsOnTooSteepSlope()
    {
        if (Grounded == false)
            return false;

        var angle = GetGroundAngle();
        return _movementConfig.IsSlopeTooSteep(angle);
    }

    public bool IsSlopeTooSteep(float angle)
    {
        return _movementConfig.IsSlopeTooSteep(angle);
    }

    public float GetGroundAngle()
    {
        var hit = _groundHit;
        float angle = Vector3.Angle(Vector3.up, hit.Normal);

        return angle;
    }
}
