using UnityEngine;

namespace PlotvaIzLodzya.Player.Movement
{
    public class Velocity
    {
        private MovementConfig _config;

        public Velocity(MovementConfig config)
        {
            _config = config;
        }

        public Vector3 Calculate(Vector3 currentVelocity, Vector3 desiredVelocity, bool increase)
        {
            var accelerationTime = _config.DecceleartionTime;

            if (increase)
                accelerationTime = _config.AcceleartionTime;

            currentVelocity = GetVelocity(currentVelocity, desiredVelocity, accelerationTime);
            return currentVelocity;
        }

        private Vector3 GetVelocity(Vector3 currentVelocity, Vector3 desiredVelocity, float accelerationTime)
        {
            var acceleration = CalculateAcceleration(_config.Speed, accelerationTime);

            currentVelocity = Vector3.MoveTowards(currentVelocity, desiredVelocity, acceleration * Time.deltaTime);
            currentVelocity = Vector3.ClampMagnitude(currentVelocity, _config.Speed);

            return currentVelocity;
        }

        private float CalculateAcceleration(float desiredSpeed, float time)
        {
            return desiredSpeed / time;
        }
    }
}

