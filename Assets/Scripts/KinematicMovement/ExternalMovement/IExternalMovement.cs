using UnityEngine;

namespace PlotvaIzLodzya.KinematicMovement.ExternalMovement
{
    public interface IExternalMovement
    {
        Vector3 Velocity { get; }
        Vector3 UnscaledVelocity { get; }
    }
}