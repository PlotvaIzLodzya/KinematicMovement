using UnityEngine;

namespace PlotvaIzLodzya.KinematicMovement.CollideAndSlide
{
    public interface ISlide
    {
        Vector3 SlideByMovement_recursive(Vector3 vel, Vector3 currentPos, int currentDepth = 0);
        public Vector3 SlideByGravity_recursive(Vector3 vel, Vector3 currentPos, int currentDepth = 0);

        public void SetDesireDir(Vector3 dir);
    }
}