using UnityEngine;

namespace PlotvaIzLodzya.Player.Movement.CollideAndSlide.CollisionDetection
{
    public abstract class ColliderHandler<T> : ICollisionHandler where T : Collider
    {
        protected CharacterConfig Config;
        protected T Collider;

        protected ColliderHandler(CharacterConfig config, T collider)
        {
            Config = config;
            Collider = collider;
        }

        public abstract bool IsCollide(Vector3 pos, Vector3 dir, out RaycastHit hit, float dist);
    }

    public class BoxColliderHandler : ColliderHandler<BoxCollider>
    {
        private Transform _transform;

        public BoxColliderHandler(CharacterConfig config, BoxCollider collider) : base(config, collider)
        {
            _transform = collider.transform;
        }

        public override bool IsCollide(Vector3 pos, Vector3 dir, out RaycastHit hit, float dist)
        {
            return Physics.BoxCast(pos, Collider.size / 2, dir, out hit, _transform.localRotation, dist);
        }
    }

    public class SphereColliderHandler : ColliderHandler<SphereCollider>
    {
        public SphereColliderHandler(CharacterConfig config, SphereCollider collider) : base(config, collider)
        {
        }

        public override bool IsCollide(Vector3 pos, Vector3 dir, out RaycastHit hit, float dist)
        {
            return Physics.SphereCast(pos, Collider.radius, dir, out hit, dist);
        }
    }


    public class CapsuleCollisionHandler: ColliderHandler<CapsuleCollider>
    {
        public CapsuleCollisionHandler(CharacterConfig config, CapsuleCollider collider) : base(config, collider)
        {
        }

        public override bool IsCollide(Vector3 pos, Vector3 dir, out RaycastHit hit, float dist)
        {
            return CapsuleCast(pos, dir, out hit, dist);
        }

        private bool CapsuleCast(Vector3 pos, Vector3 dir, out RaycastHit hit, float dist)
        {
            var p1 = pos + Collider.center + Config.CharacterUp * -Collider.height * 0.5f;
            var p2 = p1 + Config.CharacterUp * Collider.height;

            return Physics.CapsuleCast(p1, p2, Collider.radius, dir, out hit, dist);
        }
    }
}

