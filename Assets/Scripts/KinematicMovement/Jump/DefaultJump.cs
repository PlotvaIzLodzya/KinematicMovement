using UnityEngine;

namespace PlotvaIzLodzya.KinematicMovement.Jump
{
    [CreateAssetMenu(fileName = nameof(DefaultJump), menuName = "SO/Jump/" + nameof(DefaultJump), order = 1)]
    public class DefaultJump : JumpBehaviour
    {
        public override void Jump(float speed)
        {
            VelocityCompute.Jump(speed);
        }

        public override void CancelJump()
        {
            
        }
    }
}