using PlotvaIzLodzya.Extensions;
using PlotvaIzLodzya.KinematicMovement.Body;
using PlotvaIzLodzya.KinematicMovement.StateHandle;
using System;
using UnityEngine;

namespace PlotvaIzLodzya.KinematicMovement.CollisionCompute
{
    public class CapsuleCollision3D : CollisionCompute3D<CapsuleCollider>
    {
        private const float MaxHeight = 100f;
        private const float MinHeight = 1f;


        public CapsuleCollision3D(CapsuleCollider collider, Body3D body, ILayerMaskProvider layerMaskProvider) : base(collider, body, layerMaskProvider)
        {
        }

        public override HitInfo GetHit(Vector3 pos, Vector3 dir, float dist)
        {
            (var p1, var p2) = GetCapsulePoints(pos);
            Physics.CapsuleCast(p1, p2, Collider.ScaledRadius(), dir, out RaycastHit raycastHit, dist, CollisionMask);

            return raycastHit.ToHitInfo();
        }

        public override Collider[] Overlap(Vector3 pos)
        {
            Array.Clear(Colliders, 0, Colliders.Length);
            (var p1, var p2) = GetCapsulePoints(pos);
            Physics.OverlapCapsuleNonAlloc(p1, p2, Collider.ScaledRadius(), Colliders, CollisionMask);

            return Colliders;
        }

        private (Vector3 point1, Vector3 point2) GetCapsulePoints(Vector3 pos)
        {
            var capsulHeight = Mathf.Clamp(Collider.height, MinHeight, MaxHeight);
            var p1 = pos + Collider.center + Vector3.up * (-capsulHeight * Body.LocalScale.y + Collider.ScaledRadius() * 2) * 0.5f;
            var p2 = p1 + Vector3.up * (capsulHeight * Body.LocalScale.y - Collider.ScaledRadius() * 2);

            return (p1, p2);
        }
    }
}