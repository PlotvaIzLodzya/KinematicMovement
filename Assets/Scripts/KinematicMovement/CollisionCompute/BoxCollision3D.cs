using PlotvaIzLodzya.Extensions;
using PlotvaIzLodzya.KinematicMovement.Body;
using PlotvaIzLodzya.KinematicMovement.StateHandle;
using System;
using UnityEngine;

namespace PlotvaIzLodzya.KinematicMovement.CollisionCompute
{
    public class BoxCollision3D : CollisionCompute3D<BoxCollider>
    {
        public BoxCollision3D(BoxCollider collider, Body3D body, ILayerMaskProvider layerMaskProvider) : base(collider, body, layerMaskProvider)
        {
        }

        public override HitInfo GetHit(Vector3 pos, Vector3 dir, float dist)
        {
            Physics.BoxCast(pos, Collider.ScaledSize()*0.5f, dir, out RaycastHit raycastHit, Body.Rotation, dist, CollisionMask);

            return raycastHit.ToHitInfo();
        }

        public override Collider[] Overlap(Vector3 pos)
        {
            Array.Clear(Colliders, 0, Colliders.Length);
            Physics.OverlapBoxNonAlloc(pos, Collider.ScaledSize()*0.5f, Colliders, Body.Rotation, CollisionMask);

            return Colliders;
        }
    }
}