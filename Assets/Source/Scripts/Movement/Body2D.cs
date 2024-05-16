using UnityEngine;

namespace PlotvaIzLodzya.Player.Movement
{
    public class Body2D : IBody
    {
        private Rigidbody2D _rigidbody2D;
        public Vector3 Position => _rigidbody2D.position;

        public Body2D(Rigidbody2D rigidbody2D)
        {
            _rigidbody2D = rigidbody2D;
        }


        public void MovePosition(Vector3 postion)
        {
            _rigidbody2D.MovePosition(postion);
        }
    }
}

