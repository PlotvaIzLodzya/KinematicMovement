using UnityEngine;
using System;

namespace PlotvaIzLodzya.Player.Movement
{
    [Serializable]
    public struct MovementConfig
    {
        public float Speed;
        public float MaxSlopeAngle;

        public MovementConfig(float speed, float maxSlopeAngle)
        {
            Speed = speed;
            MaxSlopeAngle = maxSlopeAngle;
        }
    }
}

