using PlotvaIzLodzya.KinematicMovement.Jump;
using PlotvaIzLodzya.KinematicMovement.VelocityCompute;

namespace PlotvaIzLodzya.KinematicMovement
{
    public abstract class JumpBehaviour : IJumpBehaviour
    {
        private IVeloictyComputeProvider _velocityProvider;
        protected IVelocityCompute Velocity => _velocityProvider.Current;

        protected JumpBehaviour(IVeloictyComputeProvider velocityProvider)
        {
            _velocityProvider = velocityProvider;
        }

        public abstract void CancelJump();
        public abstract void Jump(float speed);
    }
}