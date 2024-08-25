using PlotvaIzLodzya.KinematicMovement.Jump;
using PlotvaIzLodzya.KinematicMovement.VelocityCompute;
using UnityEngine;

namespace PlotvaIzLodzya.KinematicMovement
{
    [CreateAssetMenu(fileName = nameof(CancelableJump), menuName = "SO/Jump/" + nameof(CancelableJump), order = 1)]
    public class CancelableJump : ScriptableObject, IJumpBehaviour
    {
        private CancelableJumpBehaviour _jumpBehaviour;

        public void Init(IVeloictyComputeProvider veloictyComputeProvider)
        {
            _jumpBehaviour = new CancelableJumpBehaviour(veloictyComputeProvider);
        }

        public void Jump(float speed)
        {
            _jumpBehaviour.Jump(speed);
        }

        public void CancelJump()
        {
            _jumpBehaviour.CancelJump();
        }
    }
}