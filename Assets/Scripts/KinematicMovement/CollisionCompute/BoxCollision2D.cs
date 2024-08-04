using PlotvaIzLodzya.Extensions;
using PlotvaIzLodzya.KinematicMovement.Body;
using PlotvaIzLodzya.KinematicMovement.StateHandle;
using UnityEngine;

namespace PlotvaIzLodzya.KinematicMovement.CollisionCompute
{
    public class BoxCollision2D : CollisionCompute2D<BoxCollider2D>
    {
        public BoxCollision2D(BoxCollider2D collider, Body2D body, ILayerMaskProvider layerMaskProvider) : base(collider, body, layerMaskProvider)
        {
        }

        public override HitInfo GetHit(Vector3 pos, Vector3 dir, float dist)
        {
            return Physics2D.BoxCast(pos, Collider.bounds.size, Body.Angle, dir, dist, CollisionMask).ToHitInfo(Collider);
        }
    }
}