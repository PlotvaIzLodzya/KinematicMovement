using PlotvaIzLodzya.Extensions;
using PlotvaIzLodzya.KinematicMovement.ExternalMovement;
using PlotvaIzLodzya.KinematicMovement.StateHandle;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace PlotvaIzLodzya.KinematicMovement.VelocityCompute
{

    [CreateAssetMenu(fileName = nameof(VelocityHandler), menuName = "SO/" + nameof(VelocityComputation) + "/" + nameof(VelocityHandler), order = 1)]
    public class VelocityHandler: ScriptableObject, IVeloictyComputeProvider
    {
        [SerializeField] private List<VelocityComputation> _defaultVelocitys;

        private List<VelocityComputation> _velocityComputations;
        private IPlatformProvider _platformProvider;
        private MovementConfig _movementConfig;
        private MovementState _state;
        private GroundVelocity _defaultVelocity;

        public IVelocityCompute Current { get; private set; }

        public void Init(MovementState state, MovementConfig movementConfig, IPlatformProvider provider)
        {
            _platformProvider = provider;
            _movementConfig = movementConfig;
            _state = state;
            _velocityComputations = CreateRuntimeComputations();
            Initialize();
            _defaultVelocity = GetVelocityCompute<GroundVelocity>();
        }

        public void AddVelocityCompute(VelocityComputation velocityComputation)
        {
            Initialize(velocityComputation);
            _velocityComputations.Add(velocityComputation);
        }

        public T GetVelocityCompute<T>() where T : IVelocityCompute
        {
            return (T)(_velocityComputations.First(v => v is T) as IVelocityCompute);
        }

        public IVelocityCompute GetVelocityCompute()
        {
            var prev = Current;

            Current = Transit();

            if (Current != prev)
            {
                prev?.Exit();
                var prevVel = prev == null ? Vector3.zero : prev.Velocity;
                Current.Enter(prevVel);
            }

            return Current;
        }

        private VelocityComputation Transit()
        {
            VelocityComputation current = null;
            try
            {
                current = _velocityComputations.FirstOrDefault(v => v.CanTransit);
            }
            catch (NullReferenceException)
            {
                Initialize();
                Debug.LogError($"{current.GetType().Name} probably was not initialized properly");
            }
            current ??= _defaultVelocity;
            return current;
        }

        private void Initialize() => _velocityComputations.ForEach(Initialize);

        private void Initialize(VelocityComputation velocity)
        {
            Action initizalization = null;
            initizalization = velocity switch
            {
                PlatformJumpVelocity platformJump => () => platformJump.Init(_state, _movementConfig),
                AirborneVelocityCompute air => () => air.Init(_state, _movementConfig),
                GroundVelocity ground => () => ground.Init(_state, _movementConfig),
                _ => throw new MissingReferenceException($"No matching pattern for initizalization {velocity.GetType().Name}")
            };

            initizalization?.Invoke();
        }

        private List<VelocityComputation> CreateRuntimeComputations()
        {
            _defaultVelocitys ??= CreateDefault();

            return new List<VelocityComputation>(_defaultVelocitys);
        }

        private List<VelocityComputation> CreateDefault()
        {
            _defaultVelocitys = new();
            var defaultVelocity = CreateInstance<GroundVelocity>();
            defaultVelocity.Init(_state, _movementConfig);
            _defaultVelocitys.Add(defaultVelocity);
            Debug.LogError($"{nameof(VelocityHandler)} default velocity was empty, created default version of {nameof(GroundVelocity)}");

            return _defaultVelocitys;
        }
    }
}