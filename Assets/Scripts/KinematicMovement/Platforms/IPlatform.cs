using PlotvaIzLodzya.KinematicMovement.ExternalMovement;
using UnityEngine;

namespace PlotvaIzLodzya.KinematicMovement.Platforms
{
    public interface IPlatform : IExternalMovement
    {
        Vector3 Position { get; }
        Vector3 CollisionPoint { get; }
        Quaternion RotationVelocity { get; }
    }
}