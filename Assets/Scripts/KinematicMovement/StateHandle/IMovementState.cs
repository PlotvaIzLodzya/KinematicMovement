using UnityEngine;

namespace PlotvaIzLodzya.KinematicMovement.StateHandle
{
    public interface IMovementState
    {
        bool IsOnPlatform { get; }
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
        Vector3 WallNormal { get; }

        bool Check(Vector3 velocity);
        bool IsSlopeTooSteep(float angle);
    }
}