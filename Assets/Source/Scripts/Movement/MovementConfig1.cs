using UnityEngine;
using System;

namespace PlotvaIzLodzya.Player.Movement.CollideAndSlide
{
    [Serializable]
    public class MovementConfig
    {
        [field: SerializeField] public float Speed { get; private set; } = 15f;
    }
}

