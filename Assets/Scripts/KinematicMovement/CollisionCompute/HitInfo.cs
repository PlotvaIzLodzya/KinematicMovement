using UnityEngine;

namespace PlotvaIzLodzya.KinematicMovement.CollisionCompute
{
    public struct HitInfo
    {
        public Vector3 Point;
        public Vector3 HitNormal;
        public float Distance;
        public float ColliderDistance;
        public bool HaveHit;
        public Transform Transform;

        public HitInfo(Vector3 point, Vector3 normal, float distance, float colliderDistance, bool haveHit, Transform transform)
        {
            Point = point;
            HitNormal = normal;
            Distance = distance;
            ColliderDistance = colliderDistance + MovementConfig.ContactOffset;
            HaveHit = haveHit;
            Transform = transform;
        }
    }
}