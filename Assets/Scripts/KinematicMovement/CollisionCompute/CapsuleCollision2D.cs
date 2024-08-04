using PlotvaIzLodzya.Extensions;
using PlotvaIzLodzya.KinematicMovement.Body;
using PlotvaIzLodzya.KinematicMovement.StateHandle;
using UnityEngine;

namespace PlotvaIzLodzya.KinematicMovement.CollisionCompute
{
    public class CapsuleCollision2D : CollisionCompute2D<CapsuleCollider2D>
    {
        public CapsuleCollision2D(CapsuleCollider2D collider, Body2D body, ILayerMaskProvider layerMaskProvider) : base(collider, body, layerMaskProvider)
        {
        }

        public override HitInfo GetHit(Vector3 pos, Vector3 dir, float dist)
        {
            var raycastHit = Physics2D.CapsuleCast(pos, Collider.bounds.size, Collider.direction, 0, dir, dist, CollisionMask);

            return raycastHit.ToHitInfo(Collider);
        }
    }
}