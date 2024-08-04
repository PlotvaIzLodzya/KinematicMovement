using PlotvaIzLodzya.KinematicMovement.Body;
using PlotvaIzLodzya.KinematicMovement.StateHandle;
using UnityEngine;

namespace PlotvaIzLodzya.KinematicMovement.CollisionCompute
{
    public abstract class CollisionCompute3D<T> : CollisionCompute<Body3D> where T : Collider
    {
        public const int MaxCollisionCount = 32;

        protected Collider[] Colliders;
        protected T Collider;

        protected CollisionCompute3D(T collider, Body3D body, ILayerMaskProvider layerMaskProvider) : base(body, layerMaskProvider)
        {
            Colliders = new Collider[MaxCollisionCount];
            Collider = collider;
        }

        public abstract Collider[] Overlap(Vector3 pos);

        public override HitInfo GetHit(Vector3 pos)
        {
            var colliders = Overlap(pos);
            var hitInfo = GetClosestHitPosition(colliders, pos);

            return hitInfo;
        }

        public HitInfo GetClosestHitPosition(Collider[] colliders, Vector3 position)
        {
            var closestDistance = float.MinValue;

            HitInfo hitInfo = new HitInfo();

            foreach (var collider in colliders)
            {
                if (collider == null)
                    continue;

                Physics.ComputePenetration(collider, collider.transform.position, collider.transform.rotation, Collider, Body.Position, Body.Rotation, out Vector3 dir, out float distance);
                var dist = distance;

                if (dist > closestDistance)
                {
                    closestDistance = dist;
                    var closestPosition = Body.Position - dir.normalized * closestDistance;
                    hitInfo = new HitInfo(closestPosition, dir.normalized, distance, closestDistance, true, collider.transform);
                }
            }

            return hitInfo;
        }
    }
}