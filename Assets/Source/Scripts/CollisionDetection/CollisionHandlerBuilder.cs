using System;
using UnityEngine;

namespace PlotvaIzLodzya.Player.Movement.CollideAndSlide.CollisionDetection
{
    public static class CollisionHandlerBuilder
    {
        public static ICollisionHandler Create<T>(T collider, CollisionConfig characterConfig) where T : Collider
        {
            ICollisionHandler collisionHandler = collider switch
            {
                CapsuleCollider capsuleCollider => new CapsuleCollisionHandler(characterConfig, capsuleCollider),
                SphereCollider sphereCollider => new SphereCollisionHandler(characterConfig, sphereCollider),
                BoxCollider boxCollider => new BoxCollisionHandler(characterConfig, boxCollider),
                _ => throw new MissingReferenceException($"There's no handler for {collider.GetType().Name} type")
            };

            return collisionHandler;
        }
    }
}

