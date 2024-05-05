using UnityEngine;

namespace PlotvaIzLodzya.Player.Movement
{
    public struct VelocityConfig
    {
        public Vector3 CurrentVelocity;
        public Vector3 DesiredVelocity;
        public float MinSpeed;
        public float MaxSpeed;
        public float AccelerationTime;
    }
}

