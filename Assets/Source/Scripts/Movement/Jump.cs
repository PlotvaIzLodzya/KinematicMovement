using PlotvaIzLodzya.Extensions;
using UnityEngine;

namespace PlotvaIzLodzya.Player.Movement
{
    public class Jump: MonoBehaviour
    {
        private Movement _movement;

        private void Awake()
        {
            _movement = this.GetComponentNullAwarness<Movement>();
        }

        private void Update()
        {
            
        }

        public void Perform()
        {
            Debug.Log($"Can jump: {_movement.IsGrounded}");
        }
    }
}

