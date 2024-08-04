using PlotvaIzLodzya.KinematicMovement.ExternalMovement;
using PlotvaIzLodzya.KinematicMovement.StateHandle;
using UnityEngine;

namespace PlotvaIzLodzya.KinematicMovement.VelocityCompute
{
    public class VelocityHandler
    {
        private MovementState _state;
        private VelocityCompute _velocity;
        private AirborneVelocityCompute _airborneVelocity;
        private PlatformJumpVelocity _platformJumpVelocity;
        private IVelocityCompute _current;

        public VelocityHandler(MovementState state, MovementConfig movementConfig, IPlatformProvider provider)
        {
            _state = state;
            _velocity = new VelocityCompute(state, movementConfig);
            _airborneVelocity = new FullControlAirborneVelocity(state, movementConfig);
            _platformJumpVelocity = new PlatformJumpVelocity(_airborneVelocity, provider, movementConfig);
        }

        public IVelocityCompute GetVelocityCompute<T>() where T : IVelocityCompute
        {
            return true switch
            {
                true when typeof(T) == typeof(VelocityCompute) => _velocity,
                true when typeof(T) == typeof(PlatformJumpVelocity) => _platformJumpVelocity,
                true when typeof(T) == typeof(AirborneVelocityCompute) => _airborneVelocity,
                _ => throw new System.NotImplementedException(),
            };
        }

        public IVelocityCompute GetVelocityCompute()
        {
            var prev = _current;

            if (_state.IsOnPlatform && _state.IsJumping)
                _current = _platformJumpVelocity;
            else if (_state.Grounded)
                _current = _velocity;
            else if (_state.Grounded == false)
                _current = _airborneVelocity;
            if (_current != prev)
            {
                prev?.Exit();
                var prevVel = prev == null ? Vector3.zero : prev.Velocity;
                _current.Enter(prevVel);
            }

            return _current;
        }
    }
}