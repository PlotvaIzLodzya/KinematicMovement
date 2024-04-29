using PlotvaIzLodzya.Player.Movement.CollideAndSlide.CollisionDetection;
using System;

namespace PlotvaIzLodzya.Player.Movement.CollideAndSlide
{
    public class MovementState
    {
        public CollisionState Grounded { get; private set; }
        public CollisionState Ceiled { get; private set; }
        private CollisionConfig _collisionConfig;

        public MovementState(ICollisionHandler collisionHandler)
        {
            if(collisionHandler == null)
                throw new ArgumentNullException($"Collision handler is null");

            _collisionConfig = collisionHandler.Config;
            Grounded = new(collisionHandler);
            Ceiled = new(collisionHandler);
        }


        public void Update()
        {
            Grounded.Update(-_collisionConfig.Up);
            Ceiled.Update(_collisionConfig.Up);
        }
    }
}

