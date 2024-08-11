using PlotvaIzLodzya.Extensions;
using PlotvaIzLodzya.KinematicMovement.StateHandle;

namespace PlotvaIzLodzya.KinematicMovement.VelocityCompute
{
    public class AirborneVelocityCompute : VelocityComputation
    {
        private float _addedSpeed;

        protected override float MaxHorizontalSpeed => base.MaxHorizontalSpeed + _addedSpeed;

        public AirborneVelocityCompute(IMovementState state, MovementConfig movementConfig) : base(state, movementConfig)
        {
        }

        public void AddMaxHorSpeed(float maxHorSpeed)
        {
            _addedSpeed = maxHorSpeed;
        }

        public override void Exit()
        {
            _addedSpeed = 0f;
            if (Direction.sqrMagnitude == 0)
            {
                Velocity = Velocity.SetHorizontalToZero();
            }

            base.Exit();
        }
    }
}