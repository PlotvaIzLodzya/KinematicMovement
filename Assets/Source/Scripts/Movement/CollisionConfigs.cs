using System;
using UnityEngine;

namespace PlotvaIzLodzya.Player.Movement.CollideAndSlide
{
    [Serializable]
    public class CollisionConfig
    {
        public float ClipPreventingValue => 0.02f;

        [field: SerializeField] public Vector3 ObjectUp { get; private set; }
        [field: SerializeField] public LayerMask CollisionMask { get; private set; }


        public CollisionConfig(Vector3 characterUp, LayerMask collisionMask)
        {
            ObjectUp = characterUp;
            CollisionMask = collisionMask;
        }
    }

    public class WorldConfig
    {
        public Vector3 Gravity { get; private set; }
        public Vector3 WorldUp {get; private set;}

        public WorldConfig(Vector3 gravity, Vector3 worldUp)
        {
            Gravity = gravity;
            WorldUp = worldUp;
        }

    }
}

