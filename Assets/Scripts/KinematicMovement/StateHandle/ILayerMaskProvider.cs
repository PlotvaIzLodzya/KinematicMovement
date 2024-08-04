using UnityEngine;

namespace PlotvaIzLodzya.KinematicMovement.StateHandle
{
    public interface ILayerMaskProvider
    {
        LayerMask GroundMask { get; }
    }
}