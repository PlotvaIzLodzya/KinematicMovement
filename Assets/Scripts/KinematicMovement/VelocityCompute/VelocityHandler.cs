using PlotvaIzLodzya.KinematicMovement.ExternalMovement;
using PlotvaIzLodzya.KinematicMovement.StateHandle;
using UnityEngine;

namespace PlotvaIzLodzya.KinematicMovement.VelocityCompute
{

    public class VelocityHandler : IVeloictyComputeProvider
    {
        private MovementState _state;
        private VelocityComputation _velocity;
        private AirborneVelocityCompute _airborneVelocity;
        private PlatformJumpVelocity _platformJumpVelocity;

        public IVelocityCompute Current { get; private set; }

        public VelocityHandler(MovementState state, MovementConfig movementConfig, IPlatformProvider provider)
        {
            _state = state;
            _velocity = new VelocityComputation(state, movementConfig);
            _airborneVelocity = new FullControlAirborneVelocity(state, movementConfig);
            _platformJumpVelocity = new PlatformJumpVelocity(_airborneVelocity, provider, movementConfig);
        }

        public IVelocityCompute GetVelocityCompute<T>() where T : IVelocityCompute
        {
            return true switch
            {
                true when typeof(T) == typeof(VelocityComputation) => _velocity,
                true when typeof(T) == typeof(PlatformJumpVelocity) => _platformJumpVelocity,
                true when typeof(T) == typeof(AirborneVelocityCompute) => _airborneVelocity,
                _ => throw new System.NotImplementedException(),
            };
        }

        public IVelocityCompute GetVelocityCompute()
        {
            var prev = Current;

            if (_state.IsOnPlatform && _state.IsJumping)
                Current = _platformJumpVelocity;
            else if (_state.Grounded)
                Current = _velocity;
            else if (_state.Grounded == false)
                Current = _airborneVelocity;
            if (Current != prev)
            {
                prev?.Exit();
                var prevVel = prev == null ? Vector3.zero : prev.Velocity;
                Current.Enter(prevVel);
            }

            return Current;
        }
    }
}