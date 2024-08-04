using PlotvaIzLodzya.KinematicMovement.Body;
using PlotvaIzLodzya.KinematicMovement.StateHandle;
using UnityEngine;

namespace PlotvaIzLodzya.KinematicMovement.CollisionCompute
{
    public interface ICollision
    {
        void Depenetrate();
        bool TryGetHit(Vector3 pos, Vector3 dir, float dist, out HitInfo hit);
        bool TryGetHit(out HitInfo hit);
        HitInfo GetHit(Vector3 pos, Vector3 dir, float dist);
        bool CheckDirection(Vector3 direction);
        HitInfo GetHit(Vector3 position);
        HitInfo GetHit();
        Vector3 GetClosestPositionTo(HitInfo hitInfo);
    }

    public abstract class CollisionCompute<T> : ICollision where T : IBody
    {
        private const int DepenetrationIteration = 10;

        protected T Body;
        private ILayerMaskProvider _layerMaskProvider;

        public LayerMask CollisionMask => _layerMaskProvider.GroundMask;

        public CollisionCompute(T body, ILayerMaskProvider layerMaskProvider)
        {
            Body = body;
            _layerMaskProvider = layerMaskProvider;
        }

        public abstract HitInfo GetHit(Vector3 pos, Vector3 dir, float dist);

        public Vector3 GetClosestPositionTo(HitInfo hitInfo)
        {
            var deltaPos = GetDeltaPositionToHit(hitInfo);
            var targetPos = Body.Position - deltaPos;

            return targetPos;
        }

        public Vector3 GetDeltaPositionToHit(HitInfo hit)
        {
            var hitDist = hit.ColliderDistance;
            var deltaPos = hit.Normal * hitDist;

            return deltaPos;
        }

        public virtual HitInfo GetHit(Vector3 position)
        {
            return GetHit(Body.Position, Vector3.zero, MovementConfig.ContactOffset);
        }

        public virtual HitInfo GetHit()
        {
            return GetHit(Body.Position);
        }

        public bool TryGetHit(Vector3 pos, Vector3 dir, float dist, out HitInfo hit)
        {
            hit = GetHit(pos, dir, dist);
            return hit.HaveHit;
        }

        public void Depenetrate()
        {
            var iteration = 0;
            while (iteration <= DepenetrationIteration)
            {
                if (TryGetHit(out HitInfo hit))
                {
                    var closestPosition = GetClosestPositionTo(hit);
                    Body.Position = closestPosition;
                }
                else
                {
                    break;
                }

                iteration++;
            }
        }

        public bool TryGetHit(out HitInfo hit)
        {
            hit = GetHit();

            return hit.HaveHit;
        }

        public bool CheckDirection(Vector3 direction)
        {
            return TryGetHit(Body.Position, direction, MovementConfig.ContactOffset, out var hit);
        }
    }
}