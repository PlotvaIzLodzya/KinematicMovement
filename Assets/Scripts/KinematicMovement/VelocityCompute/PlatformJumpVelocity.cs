using PlotvaIzLodzya.KinematicMovement.ExternalMovement;
using PlotvaIzLodzya.KinematicMovement.StateHandle;
using UnityEngine;

namespace PlotvaIzLodzya.KinematicMovement.VelocityCompute
{
    [CreateAssetMenu(fileName = nameof(PlatformJumpVelocity), menuName = "SO/" + nameof(VelocityComputation) + "/" + nameof(PlatformJumpVelocity), order = 2)]
    public class PlatformJumpVelocity : VelocityComputation
    {
        private IPlatformProvider _platformProvider;
        private AirborneVelocityCompute _airborneVelocity;
        public override bool CanTransit => State.IsOnPlatform && State.IsJumping;

        public override Vector3 Velocity
        {
            get
            {
                return _airborneVelocity.Velocity;
            }
            set
            {
                _airborneVelocity.Velocity = value;
            }
        }

        public void Init(IMovementState state, IPlatformProvider platformProvider, AirborneVelocityCompute airborneVelocity)
        {
            State = state;
            _airborneVelocity = airborneVelocity;
            _platformProvider = platformProvider;
        }

        public override void Enter(Vector3 currentVelocity)
        {
            var platformVelocity = Vector3.zero;

            //Some wierd bug when you touch platform after jumping off it
            if(_platformProvider.Platform != null)
                platformVelocity = _platformProvider.Platform.UnscaledVelocity;

            _airborneVelocity.AddMaxHorSpeed(platformVelocity.magnitude);
            _airborneVelocity.Enter(currentVelocity + platformVelocity);
        }

        public override Vector3 CalculateHorizontalSpeed(Vector3 dir, float deltaTime)=> _airborneVelocity.CalculateHorizontalSpeed(dir, deltaTime);
        public override float CalculateVerticalSpeed(float deltaTime) => _airborneVelocity.CalculateVerticalSpeed(deltaTime);
        public override void Exit() => _airborneVelocity.Exit();
        public override void Jump(float speed) => _airborneVelocity.Jump(speed);
        public override void SetVelocity(Vector3 velocity) => _airborneVelocity.SetVelocity(velocity);
    }
}