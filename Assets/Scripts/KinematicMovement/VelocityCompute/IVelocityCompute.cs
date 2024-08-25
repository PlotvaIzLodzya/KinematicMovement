using UnityEngine;

namespace PlotvaIzLodzya.KinematicMovement.VelocityCompute
{
    public interface IVelocityCompute
    {
        Vector3 Velocity { get; }
        
        void SetVelocity(Vector3 velocity);
        void Jump(float speed);
        Vector3 CalculateHorizontalSpeed(Vector3 dir, float deltaTime);
        float CalculateVerticalSpeed(float deltaTime);
        void Exit();
        void Enter(Vector3 currentVelocity);
    }
}
