using PlotvaIzLodzya.Extensions;
using PlotvaIzLodzya.KinematicMovement.StateHandle;
using UnityEngine;

namespace PlotvaIzLodzya.KinematicMovement.VelocityCompute
{
    public class KeepMomentumAirborneVelocity : AirborneVelocityCompute
    {
        public KeepMomentumAirborneVelocity(IMovementState state, MovementConfig movementConfig) : base(state, movementConfig)
        {
        }

        public override Vector3 CalculateHorizontalSpeed(Vector3 dir, float deltaTime)
        {
            if (dir.sqrMagnitude > 0)
                return base.CalculateHorizontalSpeed(dir, deltaTime);
            else
                return Velocity.GetHorizontal();
        }
    }
}