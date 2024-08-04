using PlotvaIzLodzya.Extensions;
using PlotvaIzLodzya.KinematicMovement.Body;
using PlotvaIzLodzya.KinematicMovement.StateHandle;
using System;
using UnityEngine;

namespace PlotvaIzLodzya.KinematicMovement.CollisionCompute
{
    public class SphereCollision3D : CollisionCompute3D<SphereCollider>
    {
        public SphereCollision3D(SphereCollider collider, Body3D body, ILayerMaskProvider layerMaskProvider) : base(collider, body, layerMaskProvider)
        {
        }   

        public override HitInfo GetHit(Vector3 pos, Vector3 dir, float dist)
        {
            Physics.SphereCast(pos, Collider.ScaledRadius(), dir, out RaycastHit hit, dist, CollisionMask);

            return hit.ToHitInfo();
        }

        public override Collider[] Overlap(Vector3 pos)
        {
            Array.Clear(Colliders, 0, Colliders.Length);
            Physics.OverlapSphereNonAlloc(pos, Collider.ScaledRadius(), Colliders, CollisionMask);

            return Colliders;
        }
    }
}