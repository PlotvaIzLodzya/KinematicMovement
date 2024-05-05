using UnityEngine;

namespace PlotvaIzLodzya.Player.Movement
{
    public class Jump
    {
        private Movement _movement;

        public Jump(Movement movement)
        {
            _movement = movement;
        }

        public void Perform()
        {
            Debug.Log($"Can jump: {_movement.IsGrounded}");
        }
    }
}

