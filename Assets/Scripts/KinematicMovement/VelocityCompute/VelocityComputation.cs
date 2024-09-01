using PlotvaIzLodzya.KinematicMovement.StateHandle;
using UnityEngine;

namespace PlotvaIzLodzya.KinematicMovement.VelocityCompute
{
    public abstract class VelocityComputation : ScriptableObject, IVelocityCompute
    {
        public IMovementState State;

        public abstract Vector3 Velocity { get; set; }
        public abstract bool CanTransit { get; }
        public abstract Vector3 CalculateHorizontalSpeed(Vector3 dir, float deltaTime);
        public abstract float CalculateVerticalSpeed(float deltaTime);
        public abstract void Enter(Vector3 currentVelocity);
        public abstract void Exit();
        public abstract void Jump(float speed);
        public abstract void SetVelocity(Vector3 velocity);
    }
}
