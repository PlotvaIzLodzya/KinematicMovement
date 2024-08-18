using PlotvaIzLodzya.KinematicMovement.CollisionCompute;
using PlotvaIzLodzya.KinematicMovement.StateHandle;
using UnityEngine;

namespace PlotvaIzLodzya.KinematicMovement.CollideAndSlide
{
    public class SlideAlongSurface3D : SlideAlongSurface
    {
        public SlideAlongSurface3D(ICollision collision, IMovementState movementState) : base(collision, movementState)
        {
        }

        protected override Vector3 GetSurfaceNormal(HitInfo hit, Vector3 direction)
        {
            var ray = new Ray(hit.Point + Vector3.down * 0.001f  - direction * 0.01f, direction);
            Physics.Raycast(ray, out var result, 0.1f);
            return result.normal;
        }
    }
}