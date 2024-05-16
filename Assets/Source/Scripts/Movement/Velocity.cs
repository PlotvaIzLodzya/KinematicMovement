using PlotvaIzLodzya.Extensions;
using Unity.VisualScripting.FullSerializer;
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

        public float CalculateVerticalAcceleration()
        {
            return _config.JumpHeight / (_config.JumpTime * _config.JumpTime);
        }

        public float CalculateHorizontalAcceleration(float time)
        {
            return _config.Speed / time;
        }

        public float CalculateJumpSpeed()
        {
            var acceleration = CalculateVerticalAcceleration();
            var speed = Mathf.Sqrt(2 * acceleration * _config.JumpHeight);

            return speed;
        }

        public Vector3 CalculateHorizontal(Vector3 currentVelocity, Vector3 desiredVelocity, bool increase)
        {
            var accelerationTime = _config.DeccelerationTime;
            if (increase)
            {
                accelerationTime = _config.AccelerationTime;
            }

            var acceleration = CalculateHorizontalAcceleration(accelerationTime);
            var velocity = Vector3.MoveTowards(currentVelocity, desiredVelocity, acceleration * Time.deltaTime);
            velocity = velocity.ClampMagnitude(0, _config.Speed);
            return velocity;
        }

        public Vector3 CalculateVertical(Vector3 currentVelocity)
        {
            currentVelocity.y -= CalculateVerticalAcceleration() * Time.deltaTime;
            currentVelocity.y = Mathf.Clamp(currentVelocity.y, -_config.FallMaxSpeed, 100f);
            return currentVelocity;
        }
    }
}

