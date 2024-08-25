using PlotvaIzLodzya.KinematicMovement.VelocityCompute;

namespace PlotvaIzLodzya.KinematicMovement
{
    public class DefaultJump : JumpBehaviour
    {
        public DefaultJump(IVeloictyComputeProvider velocityProvider) : base(velocityProvider)
        {
        }
        public override void Jump(float speed)
        {
            Velocity.Jump(speed);
        }

        public override void CancelJump()
        {
            
        }
    }
}