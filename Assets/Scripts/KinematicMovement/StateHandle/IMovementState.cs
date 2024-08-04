using UnityEngine;

namespace PlotvaIzLodzya.KinematicMovement.StateHandle
{
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
}