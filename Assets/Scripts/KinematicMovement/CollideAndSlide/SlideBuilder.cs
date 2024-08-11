using PlotvaIzLodzya.KinematicMovement.Body;
using PlotvaIzLodzya.KinematicMovement.CollisionCompute;
using PlotvaIzLodzya.KinematicMovement.StateHandle;

namespace PlotvaIzLodzya.KinematicMovement.CollideAndSlide
{
    public static class SlideBuilder
    {
        public static ISlide Create(IBody body, ICollision collisionCompute, IMovementState state) => body switch
        {
            Body2D => new SlideAlongSurface2D(collisionCompute, state),
            Body3D => new SlideAlongSurface3D(collisionCompute, state),
            _ => throw new System.MissingMemberException($"Unsuportted body type")
        };
    }
}