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
            var horizontalConfig = _config.CreateHorizontalConfig(currentVelocity, desiredVelocity, increase);
            return Calculate(horizontalConfig);
        }

        public Vector3 CalculateVertical(Vector3 currentVelocity, Vector3 desiredVelocity)
        {
            var horizontalConfig = _config.CreateVerticalConfig(currentVelocity, desiredVelocity);
            return Calculate(horizontalConfig);
        }

        public Vector3 Calculate(VelocityConfig config)
        {
            var acceleration = CalculateAcceleration(config.MinSpeed, config.MaxSpeed, config.AccelerationTime);
            var velocity = Vector3.MoveTowards(config.CurrentVelocity, config.DesiredVelocity, acceleration * Time.deltaTime);
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

