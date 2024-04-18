using UnityEngine;

namespace PlotvaIzLodzya.Player.Movement.CollideAndSlide
{
    public class ShapeConfig
    {
        public Vector3 Up { get; private set; }

        public ShapeConfig(Vector3 characterUp)
        {
            Up = characterUp;
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

