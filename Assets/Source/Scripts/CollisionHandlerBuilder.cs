using System;
using UnityEngine;

namespace PlotvaIzLodzya.Player.Movement.CollideAndSlide.CollisionDetection
{
    public static class CollisionHandlerBuilder
    {
        public static ICollisionHandler Create<T>(T collider, CharacterConfig characterConfig) where T : Collider
        {
            ICollisionHandler collisionHandler;

            collisionHandler = collider switch
            {
                CapsuleCollider capsuleCollider => new CapsuleCollisionHandler(characterConfig, capsuleCollider),
                SphereCollider sphereCollider => new SphereColliderHandler(characterConfig, sphereCollider),
                BoxCollider boxCollider => new BoxColliderHandler(characterConfig, boxCollider),
                _ => throw new NullReferenceException($"There's no handler for {collider.GetType().Name} type")
            };

            return collisionHandler;
        }
    }
}

