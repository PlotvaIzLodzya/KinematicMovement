using PlotvaIzLodzya.KinematicMovement.VelocityCompute;

namespace PlotvaIzLodzya.KinematicMovement
{
    public class CancelableJumpBehaviour : JumpBehaviour
    {
        private float _smoothStopMultiplier;

        public CancelableJumpBehaviour(IVeloictyComputeProvider velocityCompute) : base(velocityCompute)
        {
            _smoothStopMultiplier = 0.25f;
        }

        public override void Jump(float speed)
        {
            Velocity.Jump(speed);
        }

        public override void CancelJump()
        {
            if (Velocity.Velocity.y > 0)
            {
                var vel = Velocity.Velocity;
                vel.y *= _smoothStopMultiplier;
                Velocity.SetVelocity(vel);
            }
        }
    }
}