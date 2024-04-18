using UnityEngine;

namespace PlotvaIzLodzya.Player.Movement.CollideAndSlide.CollisionDetection
{
    public abstract class CollisionHandler<T> : ICollisionHandler where T : Collider
    {
        protected ShapeConfig Config;
        protected T Collider;

        protected CollisionHandler(ShapeConfig config, T collider)
        {
            Config = config;
            Collider = collider;
        }

        public abstract bool IsCollide(Vector3 pos, Vector3 dir, out RaycastHit hit, float dist);
    }


    public class BoxCollisionHandler : CollisionHandler<BoxCollider>
    {
        private Transform _transform;

        public BoxCollisionHandler(ShapeConfig config, BoxCollider collider) : base(config, collider)
        {
            _transform = collider.transform;
        }

        public override bool IsCollide(Vector3 pos, Vector3 dir, out RaycastHit hit, float dist)
        {
            return Physics.BoxCast(pos, Collider.size / 2, dir, out hit, _transform.localRotation, dist);
        }
    }

    public class SphereCollisionHandler : CollisionHandler<SphereCollider>
    {
        public SphereCollisionHandler(ShapeConfig config, SphereCollider collider) : base(config, collider)
        {
        }

        public override bool IsCollide(Vector3 pos, Vector3 dir, out RaycastHit hit, float dist)
        {
            return Physics.SphereCast(pos, Collider.radius, dir, out hit, dist);
        }
    }


    public class CapsuleCollisionHandler : CollisionHandler<CapsuleCollider>
    {
        public CapsuleCollisionHandler(ShapeConfig config, CapsuleCollider collider) : base(config, collider)
        {
        }

        public override bool IsCollide(Vector3 pos, Vector3 dir, out RaycastHit hit, float dist)
        {
            return CapsuleCast(pos, dir, out hit, dist);
        }

        private bool CapsuleCast(Vector3 pos, Vector3 dir, out RaycastHit hit, float dist)
        {
            var p1 = pos + Collider.center + Config.Up * -Collider.height * 0.5f;
            var p2 = p1 + Config.Up * Collider.height;

            return Physics.CapsuleCast(p1, p2, Collider.radius, dir, out hit, dist);
        }
    }
}

