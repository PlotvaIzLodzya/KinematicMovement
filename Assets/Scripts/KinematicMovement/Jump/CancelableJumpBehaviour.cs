using System.Collections;
using UnityEngine;

namespace PlotvaIzLodzya.KinematicMovement.Jump
{
    [CreateAssetMenu(fileName = nameof(CancelableJumpBehaviour), menuName = "SO/Jump/" + nameof(CancelableJumpBehaviour), order = 1)]
    public class CancelableJumpBehaviour : JumpBehaviour
    {
        [SerializeField] private float _cancelDuration = 0.15f;

        private Coroutine _canceling;

        public override void Jump(float speed)
        {
            if(_canceling != null)
            {
                StopCoroutine(_canceling);
            }
            VelocityCompute.Jump(speed);
        }

        public override void CancelJump()
        {
            if (VelocityCompute.Velocity.y > 0)
            {
                _canceling = StartCoroutine(Canceling(_cancelDuration));
            }
        }

        private IEnumerator Canceling(float duration)
        {
            float elapsedTime = 0f;
            var startY = VelocityCompute.Velocity.y;

            while (elapsedTime < duration)
            {
                elapsedTime += Time.deltaTime;

                var vel = VelocityCompute.Velocity;
                vel.y = Mathf.Lerp(startY, 0, elapsedTime/duration);
                VelocityCompute.SetVelocity(vel);
                yield return null;
            }
        }
    }
}