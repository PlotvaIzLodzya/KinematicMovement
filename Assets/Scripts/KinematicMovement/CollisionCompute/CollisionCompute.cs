using PlotvaIzLodzya.KinematicMovement.Body;
using PlotvaIzLodzya.KinematicMovement.StateHandle;
using UnityEngine;

namespace PlotvaIzLodzya.KinematicMovement.CollisionCompute
{

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
            var deltaPos = hit.HitNormal * hitDist;

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

        public bool CheckForSurface(Vector3 direction)
        {
            return CheckForSurface(direction, out var hit);
        }

        public bool CheckForSurface(Vector3 direction, out HitInfo hit)
        {
            var haveHit = TryGetHit(Body.Position, direction, MovementConfig.ContactOffset * 2, out hit);
            hit.HitNormal = GetSurfaceNormal(hit, direction);
            return haveHit;
        }

        protected Vector3 GetSurfaceNormal(HitInfo hit, Vector3 direction)
        {
            var ray = new Ray(hit.Point + Vector3.down * 0.001f - direction * 0.01f, direction);
            Physics.Raycast(ray, out var result, 0.011f);
            return result.normal;
        }

        public virtual float GetDistanceToBounds(HitInfo hitInfo)
        {
            return 0f;
        }
    }
}