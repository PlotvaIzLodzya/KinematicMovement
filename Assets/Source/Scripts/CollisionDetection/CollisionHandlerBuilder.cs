using System;
using UnityEngine;

namespace PlotvaIzLodzya.Player.Movement.CollideAndSlide.CollisionDetection
{
    public static class CollisionHandlerBuilder
    {
        public static ICollisionHandler Create<T>(T obj, CollisionConfig collisionConfig) where T : MonoBehaviour
        {
            if(obj == null)
                throw new ArgumentNullException($"The {typeof(T)} is null");

            ICollisionHandler handler = CreateHandler(obj, collisionConfig);

            return handler;
        }

        public static ICollisionHandler CreateFrom2D<R>(R collider, CollisionConfig characterConfig) where R : Collider2D
        {
            ICollisionHandler collisionHandler = collider switch
            {
                CapsuleCollider2D capsuleCollider2D => new Capsule2DCollisionHandler(characterConfig, capsuleCollider2D),
                CircleCollider2D circleCollider2D => new Circle2DCollisionHandler(characterConfig, circleCollider2D),
                BoxCollider2D boxCollider2D => new Box2DCollisionHandler(characterConfig, boxCollider2D),
                _ => throw new MissingReferenceException(GetExceptionMessage(collider.GetType().Name))
            };

            return collisionHandler;
        }



        public static ICollisionHandler CreateFrom3D<R>(R collider, CollisionConfig characterConfig) where R : Collider
        {
            ICollisionHandler collisionHandler = collider switch
            {
                CapsuleCollider capsuleCollider => new CapsuleCollisionHandler(characterConfig, capsuleCollider),
                SphereCollider circleCollider => new SphereCollisionHandler(characterConfig, circleCollider),
                BoxCollider boxCollider => new BoxCollisionHandler(characterConfig, boxCollider),
                _ => throw new MissingReferenceException(GetExceptionMessage(collider.GetType().Name))
            };

            return collisionHandler;
        }

        private static ICollisionHandler CreateHandler(MonoBehaviour obj, CollisionConfig collisionConfig)
        {
            ICollisionHandler handler = null;
            if (obj.TryGetComponent(out Collider collider))
            {
                handler = CreateFrom3D(collider, collisionConfig);
            }
            else if (obj.TryGetComponent(out Collider2D collider2D))
            {
                handler = CreateFrom2D(collider2D, collisionConfig);
            }
            else
            {
                throw new NullReferenceException($"Cant get collider from {obj.name}");
            }

            return handler;
        }

        private static string GetExceptionMessage(string name)
        {
            return $"There's no handler for {name} type";
        }
    }
}

