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
                Normal = hit.normal,
                Position = hit.point,
                Distance = hit.distance - CollisionConfig.ClipPreventingValue,
                Transform = hit.transform,
            };

            return hitInfo;
        }

        public static HitInfo ToHitInfo(this RaycastHit2D hit, Collider2D collider)
        {
            var dist = hit.distance;
            var haveHit = hit.collider != null;
            if (haveHit) 
            {
                dist = hit.collider.Distance(collider).distance;
            }
            HitInfo hitInfo = new HitInfo()
            {
                HaveHit = haveHit,
                Normal = hit.normal,
                Position = hit.point,
                Distance = dist,
                Transform = hit.transform,
            };

            return hitInfo;
        }
    }

    public struct HitInfo
    {
        public bool HaveHit;
        public Vector3 Normal;
        public Vector3 Position;
        public float Distance;
        public Transform Transform;
    }

    public interface ICollisionHandler
    {
        CollisionConfig Config { get;}
        bool IsCollide(Vector3 pos, Vector3 dir, out HitInfo hit, float dist);
        bool IsCollide(Vector3 dir, out HitInfo hit, float dist);
        bool IsCollide(Vector3 pos);
        bool IsCollide(Vector3 pos, out HitInfo hit);
        bool IsCollide(Vector3 pos, float dist);
    }
}

