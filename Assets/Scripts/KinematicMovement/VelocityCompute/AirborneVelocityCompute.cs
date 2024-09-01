using PlotvaIzLodzya.Extensions;

namespace PlotvaIzLodzya.KinematicMovement.VelocityCompute
{
    public class AirborneVelocityCompute : GroundVelocity
    {
        private float _addedSpeed;

        protected override float MaxHorizontalSpeed => base.MaxHorizontalSpeed + _addedSpeed;
        public override bool CanTransit => State.Grounded == false;

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