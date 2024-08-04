using PlotvaIzLodzya.KinematicMovement.StateHandle;

namespace PlotvaIzLodzya.KinematicMovement.VelocityCompute
{
    public class FullControlAirborneVelocity : AirborneVelocityCompute
    {
        public FullControlAirborneVelocity(IMovementState state, MovementConfig movementConfig) : base(state, movementConfig)
        {
        }
    }
}