using System;
using UnityEngine;

namespace PlotvaIzLodzya.Player.Movement
{
    [Serializable]
    public class MovementConfig
    {
        [field: SerializeField] public float Speed { get; private set; } = 15f;
        [field: SerializeField] public float MaxSlopeAngle { get; private set; } = 45f;
        [field: SerializeField] public float AccelerationTime { get; private set; } = 0.2f;
        [field: SerializeField] public float DeccelerationTime { get; private set; } = 0.2f;        
        [field: SerializeField] public float FallMaxSpeed { get; private set; } = 45f;
        [field: SerializeField] public float JumpHeight { get; private set; } = 2f;
        [field: SerializeField] public float JumpTime { get; private set; } = 0.2f;
    }
}

