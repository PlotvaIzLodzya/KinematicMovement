using TMPro;
using UnityEngine;

namespace PlotvaIzLodzya.KinematicMovement.Body
{
    public interface IBody
    {
        Quaternion Rotation { get; set; }
        Quaternion LocalRotation { get; set; }
        Vector3 LocalScale { get; set; }
        Vector3 LossyScale { get; }
        Vector3 Position { get; set; }

        void MovePosition(Vector3 position);
    }
}