using UnityEngine;

namespace PlotvaIzLodzya.Player.Movement.CollideAndSlide.CollisionDetection
{
    public static class RaycastHitExtensions
    {
        public static HitInfo ToHitInfo(this RaycastHit hit)
        {
            HitInfo hitInfo = new HitInfo()
            {
                HaveHit = hit.collider!=null,
                normal = hit.normal,
                position = hit.point,
                distance = hit.distance,
            };

            return hitInfo;
        }

        public static HitInfo ToHitInfo(this RaycastHit2D hit)
        {
            HitInfo hitInfo = new HitInfo()
            {
                HaveHit = hit.collider!=null,
                normal = hit.normal,
                position = hit.point,
                distance = hit.distance,
            };

            return hitInfo;
        }
    }

    public struct HitInfo
    {
        public bool HaveHit;
        public Vector3 normal;
        public Vector3 position;
        public float distance;
    }

    public interface ICollisionHandler
    {
        CollisionConfig Config { get;}
        bool IsCollide(Vector3 pos, Vector3 dir, out HitInfo hit, float dist);
        bool IsCollide(Vector3 dir, out HitInfo hit, float dist);
        bool IsCollide(Vector3 pos);
    }
}

