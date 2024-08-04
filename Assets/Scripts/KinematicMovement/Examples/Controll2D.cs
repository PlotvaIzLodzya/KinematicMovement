using UnityEngine;

namespace PlotvaIzLodzya.KinematicMovement.Examples
{
    public class Controll2D : MonoBehaviour
    {
        [SerializeField] private Movement _movement;

        private Vector2 _direction;

        private void Update()
        {
            _direction = Vector2.zero;

            if (Input.GetKey(KeyCode.D))
                _direction = Vector2.right;

            if (Input.GetKey(KeyCode.A))
                _direction = Vector2.left;

            if (Input.GetKeyDown(KeyCode.Space))
                _movement.Jump();

            _movement.Move(_direction);
        }
    }
}