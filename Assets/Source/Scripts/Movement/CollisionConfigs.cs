using System;
using UnityEngine;

namespace PlotvaIzLodzya.Player.Movement.CollideAndSlide
{
    [Serializable]
    public class CollisionConfig
    {
        public const float ClipPreventingValue = 0.015f;
        public const float CollsisionDist = 0.02f;

        [field: SerializeField] public LayerMask CollisionMask { get; private set; }


        public CollisionConfig(Vector3 characterUp, LayerMask collisionMask)
        {
            CollisionMask = collisionMask;
        }
    }
}

