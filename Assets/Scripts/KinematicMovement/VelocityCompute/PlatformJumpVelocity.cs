using PlotvaIzLodzya.KinematicMovement.ExternalMovement;
using PlotvaIzLodzya.KinematicMovement.StateHandle;
using UnityEngine;

namespace PlotvaIzLodzya.KinematicMovement.VelocityCompute
{
    [CreateAssetMenu(fileName = nameof(PlatformJumpVelocity), menuName = "SO/" + nameof(VelocityComputation) + "/" + nameof(PlatformJumpVelocity), order = 2)]
    public class PlatformJumpVelocity : KeepMomentumAirborneVelocity
    {
        private IPlatformProvider _platformProvider;

        public override bool CanTransit => State.IsOnPlatform && State.IsJumping;

        public void Init(IPlatformProvider platformProvider, IMovementState state, MovementConfig movementConfig)
        {
            base.Init(state, movementConfig);
            _platformProvider = platformProvider;
        }

        public override void Enter(Vector3 currentVelocity)
        {
            var platformVelocity = _platformProvider.Platform.UnscaledVelocity;
            AddMaxHorSpeed(platformVelocity.magnitude);
            base.Enter(currentVelocity + platformVelocity);
        }
    }
}