using System;
using UnityEngine;

namespace PlotvaIzLodzya.Player.Movement.CollideAndSlide.CollisionDetection
{
    public abstract class CollisionHandler2D<T> : ICollisionHandler where T : Collider2D
    {
        public CollisionConfig Config { get; private set; }
        protected T Collider;

        protected CollisionHandler2D(CollisionConfig config, T collider)
        {
            Config = config ?? throw new NullReferenceException($"Config is null"); ;
            Collider = collider ?? throw new NullReferenceException($"Collider is null");
        }

        public abstract bool IsCollide(Vector3 pos, Vector3 dir, out HitInfo hit, float dist);

        public virtual bool IsCollide(Vector3 dir, out HitInfo hit, float dist)
        {
            return IsCollide(Collider.transform.position, dir, out hit, dist);
        }

        public bool IsCollideUp(out HitInfo hit, float dist)
        {
            return IsCollide(Collider.transform.up, out hit, dist);
        }

        public bool IsCollideDown(out HitInfo hit, float dist)
        {
            return IsCollide(-Collider.transform.up, out hit, dist);
        }

        public bool IsColideLeft(out HitInfo hit, float dist)
        {
            return IsCollide(-Collider.transform.right, out hit, dist);
        }

        public bool IsCollideRight(out HitInfo hit, float dist)
        {
            return IsCollide(Collider.transform.right, out hit, dist);
        }
    }

    public class Box2DCollisionHandler : CollisionHandler2D<BoxCollider2D>
    {
        public Box2DCollisionHandler(CollisionConfig config, BoxCollider2D collider) : base(config, collider)
        {
        }

        public override bool IsCollide(Vector3 pos, Vector3 dir, out HitInfo hit, float dist)
        {
            var raycastHit = Physics2D.BoxCast(pos, Collider.size, 0, dir, dist, Config.CollisionMask);
            hit = raycastHit.ToHitInfo();
            return hit.HaveHit;
        }
    }

    public class Circle2DCollisionHandler : CollisionHandler2D<CircleCollider2D>
    {
        public Circle2DCollisionHandler(CollisionConfig config, CircleCollider2D collider) : base(config, collider)
        {
        }

        public override bool IsCollide(Vector3 pos, Vector3 dir, out HitInfo hit, float dist)
        {
            var raycastHit = Physics2D.CircleCast(pos, Collider.radius, dir, dist, Config.CollisionMask);
            hit = raycastHit.ToHitInfo();
            return hit.HaveHit;
        }
    }

    public class Capsule2DCollisionHandler : CollisionHandler2D<CapsuleCollider2D>
    {
        public Capsule2DCollisionHandler(CollisionConfig config, CapsuleCollider2D collider) : base(config, collider)
        {
        }

        public override bool IsCollide(Vector3 pos, Vector3 dir, out HitInfo hit, float dist)
        {
            var raycastHit = Physics2D.CapsuleCast(pos, Collider.size, Collider.direction, 0, dir, dist, Config.CollisionMask);
            hit = raycastHit.ToHitInfo();
            return hit.HaveHit;
        }
    }


    public abstract class CollisionHandler<T> : ICollisionHandler where T : Collider
    {
        public CollisionConfig Config { get; private set; }
        protected T Collider;

        protected CollisionHandler(CollisionConfig config, T collider)
        {
            Config = config ?? throw new NullReferenceException($"Config is null"); ;
            Collider = collider ?? throw new NullReferenceException($"Collider is null");
        }

        public abstract bool IsCollide(Vector3 pos, Vector3 dir, out HitInfo hit, float dist);

        public virtual bool IsCollide(Vector3 dir, out HitInfo hit, float dist)
        {
            return IsCollide(Collider.transform.position, dir, out hit, dist);
        }

        public bool IsCollideUp(out HitInfo hit, float dist)
        {
            return IsCollide(Collider.transform.up, out hit, dist);
        }

        public bool IsCollideDown(out HitInfo hit, float dist)
        {
            return IsCollide(-Collider.transform.up, out hit, dist);
        }

        public bool IsColideLeft(out HitInfo hit, float dist)
        {
            return IsCollide(-Collider.transform.right, out hit, dist);
        }

        public bool IsCollideRight(out HitInfo hit, float dist)
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

        public override bool IsCollide(Vector3 pos, Vector3 dir, out HitInfo hit, float dist)
        {
            var isCollide = Physics.BoxCast(pos, Collider.size / 2, dir, out RaycastHit raycastHit, _transform.localRotation, dist, Config.CollisionMask);
            hit = raycastHit.ToHitInfo();
            return isCollide;
        }
    }

    public class SphereCollisionHandler : CollisionHandler<SphereCollider>
    {
        public SphereCollisionHandler(CollisionConfig config, SphereCollider collider) : base(config, collider)
        {
        }

        public override bool IsCollide(Vector3 pos, Vector3 dir, out HitInfo hit, float dist)
        {
            var isCollide = Physics.SphereCast(pos, Collider.radius, dir, out RaycastHit raycastHit, dist, Config.CollisionMask);
            hit = raycastHit.ToHitInfo();
            return isCollide;
        }
    }


    public class CapsuleCollisionHandler : CollisionHandler<CapsuleCollider>
    {
        public CapsuleCollisionHandler(CollisionConfig config, CapsuleCollider collider) : base(config, collider)
        {
        }

        public override bool IsCollide(Vector3 pos, Vector3 dir, out HitInfo hit, float dist)
        {
            return CapsuleCast(pos, dir, out hit, dist);
        }
        
        private bool CapsuleCast(Vector3 pos, Vector3 dir, out HitInfo hit, float dist)
        {
            var p1 = pos + Collider.center + Config.ObjectUp * -Collider.height * 0.5f;
            var p2 = p1 + Config.ObjectUp * Collider.height;
            var isCollide = Physics.CapsuleCast(p1, p2, Collider.radius, dir, out RaycastHit raycastHit, dist, Config.CollisionMask);
            hit = raycastHit.ToHitInfo();
            return isCollide;
        }
    }
}

