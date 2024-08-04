using UnityEngine;

namespace PlotvaIzLodzya.KinematicMovement.Examples
{
    public class Mover : MonoBehaviour
    {
        public Vector3 Vel;
        public Vector3 Axis;
        public float rotationSpeed;

        private void FixedUpdate()
        {
            transform.position += Vel * Time.fixedDeltaTime;
            transform.rotation *= Quaternion.Euler(Axis * rotationSpeed * Time.fixedDeltaTime);
        }
    }
}