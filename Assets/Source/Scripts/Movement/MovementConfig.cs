using UnityEngine;
using System;

namespace PlotvaIzLodzya.Player.Movement
{
    [Serializable]
    public class MovementConfig
    {
        [field: SerializeField] public float Speed { get; private set; } = 15f;
        [field: SerializeField] public float MaxSlopeAngle { get; private set; } = 45f;
        [field: SerializeField] public float AcceleartionTime { get; private set; } = 0.2f;
        [field: SerializeField] public float DecceleartionTime { get; private set; } = 0.2f;
    }
}

