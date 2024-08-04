using PlotvaIzLodzya.KinematicMovement.ExternalMovement;
using UnityEngine;

namespace PlotvaIzLodzya.KinematicMovement.VelocityCompute
{
    public class PlatformJumpVelocity : IVelocityCompute
    {
        private IPlatformProvider _platformProvider;
        private AirborneVelocityCompute _airborneVelocity;

        public Vector3 Velocity => _airborneVelocity.Velocity;

        public PlatformJumpVelocity(AirborneVelocityCompute airborneVelocity, IPlatformProvider platformProvider, MovementConfig config)
        {
            _airborneVelocity = airborneVelocity;
            _platformProvider = platformProvider;
        }

        public Vector3 CalculateHorizontalSpeed(Vector3 dir, float deltaTime)
        {
            return _airborneVelocity.CalculateHorizontalSpeed(dir, deltaTime);
        }

        public float CalculateVerticalSpeed(float deltaTime)
        {
            return _airborneVelocity.CalculateVerticalSpeed(deltaTime);
        }

        public void Enter(Vector3 currentVelocity)
        {
            var platformVelocity = _platformProvider.Platform.UnscaledVelocity;
            _airborneVelocity.Enter(currentVelocity + platformVelocity);
            _airborneVelocity.AddMaxHorSpeed(platformVelocity.magnitude);
        }

        public void Exit()
        {
            _airborneVelocity.Exit();
        }

        public void Jump(float speed)
        {
            _airborneVelocity.Jump(speed);
        }
    }
}