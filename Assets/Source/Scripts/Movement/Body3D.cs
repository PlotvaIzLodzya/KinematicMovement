using UnityEngine;

namespace PlotvaIzLodzya.Player.Movement
{
    public class Body3D : IBody
    {
        private Rigidbody _rigidbody;
        public Vector3 Position => _rigidbody.position;
        public Body3D(Rigidbody rigdibody)
        {
            _rigidbody = rigdibody;
        }

        public void MovePosition(Vector3 postion)
        {
            _rigidbody.MovePosition(postion);
        }
    }
}

