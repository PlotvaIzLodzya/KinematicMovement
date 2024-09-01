using PlotvaIzLodzya.KinematicMovement.ExternalMovement;
using UnityEngine;

namespace PlotvaIzLodzya.KinematicMovement.VelocityCompute
{
    [CreateAssetMenu(fileName = nameof(PlatformJumpVelocity), menuName = "SO/" + nameof(VelocityComputation) + "/" + nameof(PlatformJumpVelocity), order = 2)]
    public class PlatformJumpVelocity : AirborneVelocityCompute
    {
        private IPlatformProvider _platformProvider;

        public override bool CanTransit => State.IsOnPlatform && State.IsJumping;

        public void Init(IPlatformProvider platformProvider, MovementConfig config)
        {            
            _platformProvider = platformProvider;
        }

        public override void Enter(Vector3 currentVelocity)
        {
            var platformVelocity = _platformProvider.Platform.UnscaledVelocity;
            base.Enter(currentVelocity + platformVelocity);
            AddMaxHorSpeed(platformVelocity.magnitude);
        }
    }
}