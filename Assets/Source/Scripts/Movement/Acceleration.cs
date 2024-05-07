using PlotvaIzLodzya.Extensions;
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

        public Vector3 GetMaxVelocity(Vector3 currentVelocity)
        {
            return currentVelocity.normalized * _config.Speed;
        }

        public Vector3 CalculateHorizontal(Vector3 currentVelocity, Vector3 desiredVelocity, bool increase)
        {
            var accelerationTime = _config.DeccelerationTime;
            if (increase)
            {
                accelerationTime = _config.AccelerationTime;
            }

            var acceleration = CalculateAcceleration(0f, _config.Speed, accelerationTime);
            var config = _config.CreateHorizontalConfig(currentVelocity, desiredVelocity, acceleration);
            var velocity = Calculate(config);

            return velocity;
        }

        public Vector3 CalculateVertical(Vector3 currentVelocity, Vector3 desiredVelocity)
        {
            var config = _config.CreateVerticalConfig(currentVelocity, desiredVelocity, _config.GetAcceleration());

            var velocity = Calculate(config);

            return velocity;
        }

        public Vector3 Calculate(VelocityConfig config)
        {
            var velocity = Vector3.MoveTowards(config.CurrentVelocity, config.DesiredVelocity, config.Acceleration * Time.deltaTime);
            velocity = ClampVelocity(velocity, config.MinSpeed, config.MaxSpeed);

            return velocity;
        }

        private Vector3 ClampVelocity(Vector3 currentVelocity, float min, float max) 
        {
            return currentVelocity.ClampMagnitude(min, max);
        }

        private float CalculateAcceleration(float startSpeed, float endSpeed, float time)
        {
            return (endSpeed - startSpeed) / time;
        }
    }
}

