using PlotvaIzLodzya.Extensions;
using UnityEngine;

namespace PlotvaIzLodzya.KinematicMovement.VelocityCompute
{
    [CreateAssetMenu(fileName = nameof(KeepMomentumAirborneVelocity), menuName = "SO/" + nameof(VelocityComputation) + "/" + nameof(KeepMomentumAirborneVelocity), order = 2)]
    public class KeepMomentumAirborneVelocity : AirborneVelocityCompute
    {
        public override Vector3 CalculateHorizontalSpeed(Vector3 dir, float deltaTime)
        {
            if (dir.sqrMagnitude > 0)
                return base.CalculateHorizontalSpeed(dir, deltaTime);
            else
                return Velocity.GetHorizontal();
        }
    }
}