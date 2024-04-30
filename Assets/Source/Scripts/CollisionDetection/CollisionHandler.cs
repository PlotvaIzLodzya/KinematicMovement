using System;
using UnityEngine;

namespace PlotvaIzLodzya.Player.Movement.CollideAndSlide.CollisionDetection
{
    public abstract class CollisionHandler<T> : ICollisionHandler where T : Collider
    {
        public CollisionConfig Config { get; private set; }
        protected T Collider;

        protected CollisionHandler(CollisionConfig config, T collider)
        {
            Config = config ?? throw new NullReferenceException($"Config is null"); ;
            Collider = collider ?? throw new NullReferenceException($"Collider is null");
        }

        public abstract bool IsCollide(Vector3 pos, Vector3 dir, out RaycastHit hit, float dist);

        public virtual bool IsCollide(Vector3 dir, out RaycastHit hit, float dist)
        {
            return IsCollide(Collider.transform.position, dir, out hit, dist);
        }

        public bool IsCollideUp(out RaycastHit hit, float dist)
        {
            return IsCollide(Collider.transform.up, out hit, dist);
        }

        public bool IsCollideDown(out RaycastHit hit, float dist)
        {
            return IsCollide(-Collider.transform.up, out hit, dist);
        }

        public bool IsColideLeft(out RaycastHit hit, float dist)
        {
            return IsCollide(-Collider.transform.right, out hit, dist);
        }

        public bool IsCollideRight(out RaycastHit hit, float dist)
        {
            return IsCollide(Collider.transform.right, out hit, dist);
        }
    }


    public class BoxCollisionHandler : CollisionHandler<BoxCollider>
    {
        private Transform _transform;

        public BoxCollisionHandler(CollisionConfig config, BoxCollider collider) : base(config, collider)
        {
            _transform = collider.transform;
        }

        public override bool IsCollide(Vector3 pos, Vector3 dir, out RaycastHit hit, float dist)
        {
            return Physics.BoxCast(pos, Collider.size / 2, dir, out hit, _transform.localRotation, dist, Config.CollisionMask);
        }
    }

    public class SphereCollisionHandler : CollisionHandler<SphereCollider>
    {
        public SphereCollisionHandler(CollisionConfig config, SphereCollider collider) : base(config, collider)
        {
        }

        public override bool IsCollide(Vector3 pos, Vector3 dir, out RaycastHit hit, float dist)
        {
            return Physics.SphereCast(pos, Collider.radius, dir, out hit, dist, Config.CollisionMask);
        }
    }


    public class CapsuleCollisionHandler : CollisionHandler<CapsuleCollider>
    {
        public CapsuleCollisionHandler(CollisionConfig config, CapsuleCollider collider) : base(config, collider)
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

            return Physics.CapsuleCast(p1, p2, Collider.radius, dir, out hit, dist, Config.CollisionMask);
        }
    }
}

