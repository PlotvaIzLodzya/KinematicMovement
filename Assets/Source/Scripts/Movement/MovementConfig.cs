using UnityEngine;
using System;
using Unity.VisualScripting;

namespace PlotvaIzLodzya.Player.Movement
{
    [Serializable]
    public class MovementConfig
    {
        [field: SerializeField] public float Speed { get; private set; } = 15f;
        [field: SerializeField] public float MaxSlopeAngle { get; private set; } = 45f;
        [field: SerializeField] public float AccelerationTime { get; private set; } = 0.2f;
        [field: SerializeField] public float DeccelerationTime { get; private set; } = 0.2f;
        [field: SerializeField] public float FallStartSpeed { get; private set; } = 30f;
        [field: SerializeField] public float FallMaxSpeed { get; private set; } = 45f;
        [field: SerializeField] public float JumpHeight { get; private set; } = 2f;
        [field: SerializeField] public float JumpTime { get; private set; } = 0.2f;

        public float GetAcceleration()
        {
            return JumpHeight / (JumpTime * JumpTime);
        }

        public float GetJumpSpeed()
        {
            var acceleration = GetAcceleration();
            var speed = Mathf.Sqrt(2 * acceleration * JumpHeight);

            return speed;
        }

        public VelocityConfig CreateHorizontalConfig(Vector3 currentVelocity, Vector3 desiredVelocity, float acceleration)
        {
            return CreateConfig(currentVelocity, desiredVelocity, minSpeed: 0f, Speed, acceleration);
        }

        public VelocityConfig CreateVerticalConfig(Vector3 currentVelocity, Vector3 desiredVelocity, float acceleration)
        {
            return CreateConfig(currentVelocity, desiredVelocity, FallStartSpeed, FallMaxSpeed, acceleration);
        }

        private VelocityConfig CreateConfig(Vector3 currentVelocity, Vector3 desiredVelocity,float minSpeed, float maxSpeed, float acceleration)
        {
            return new VelocityConfig
            {
                CurrentVelocity = currentVelocity,
                DesiredVelocity = desiredVelocity,
                MinSpeed = minSpeed,
                MaxSpeed = maxSpeed,
                Acceleration = acceleration
            };
        }
    }
}

