using PlotvaIzLodzya.Extensions;
using PlotvaIzLodzya.KinematicMovement.Body;
using PlotvaIzLodzya.KinematicMovement.StateHandle;
using UnityEngine;

namespace PlotvaIzLodzya.KinematicMovement.CollisionCompute
{

    public class CircleCollision2D : CollisionCompute2D<CircleCollider2D>
    {
        public CircleCollision2D(CircleCollider2D collider, Body2D body, ILayerMaskProvider layerMaskProvider) : base(collider, body, layerMaskProvider)
        {
        }

        public override HitInfo GetHit(Vector3 pos, Vector3 dir, float dist)
        {
            return Physics2D.CircleCast(pos, Collider.radius * GetScale(), dir, dist, CollisionMask).ToHitInfo(Collider);
        }

        private float GetScale()
        {
            return Body.LocalScale.GetMax();
        }
    }
}