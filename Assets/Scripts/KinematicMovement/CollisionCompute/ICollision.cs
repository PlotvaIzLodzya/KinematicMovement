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
        bool CheckDirection(Vector3 direction, out HitInfo hit);
        HitInfo GetHit(Vector3 position);
        HitInfo GetHit();
        Vector3 GetClosestPositionTo(HitInfo hitInfo);
    }
}