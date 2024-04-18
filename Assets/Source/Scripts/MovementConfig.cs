using UnityEngine;

namespace PlotvaIzLodzya.Player.Movement.CollideAndSlide
{
    public class CharacterConfig
    {
        public Vector3 CharacterUp { get; private set; }

        public CharacterConfig(Vector3 characterUp)
        {
            CharacterUp = characterUp;
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

