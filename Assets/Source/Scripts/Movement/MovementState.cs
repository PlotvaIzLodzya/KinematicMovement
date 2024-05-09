using PlotvaIzLodzya.Player.Movement.CollideAndSlide.CollisionDetection;
using System;
using UnityEngine;

namespace PlotvaIzLodzya.Player.Movement.CollideAndSlide
{
    public class MovementState
    {
        public CollisionState Grounded { get; private set; }
        public CollisionState Ceiled { get; private set; }
        public CollisionState LeftSide { get; private set; }
        public CollisionState RightSide { get; private set; }
        public CollisionState ForwardSide { get; private set; }
        public CollisionState BackSide { get; private set; }
        public CollisionState CurrentCollision { get; private set; }

        public CollisionState _tempCheck;

        private Transform _transform;


        //Whoever will control transform from this script, will be lefted without right ball
        public MovementState(ICollisionHandler collisionHandler, Transform transform)
        {
            if(collisionHandler == null)
                throw new ArgumentNullException($"{nameof(collisionHandler)} is null");

            _transform = transform ?? throw new ArgumentNullException($"{nameof(transform)} is null");

            Grounded = new(collisionHandler);
            Ceiled = new(collisionHandler);
            LeftSide = new(collisionHandler);
            RightSide = new(collisionHandler);
            ForwardSide = new(collisionHandler);
            BackSide = new(collisionHandler);
            CurrentCollision = new(collisionHandler);
            _tempCheck = new(collisionHandler);
        }


        public void Update(Vector3 dir)
        {
            Grounded.Update(-_transform.up);
            Ceiled.Update(_transform.up);
            LeftSide.Update(-_transform.right);
            RightSide.Update(_transform.right);
            ForwardSide.Update(_transform.forward);
            BackSide.Update(-_transform.forward);
            CurrentCollision.Update(dir);
        }

        public bool HaveCollision(Vector3 dir, out HitInfo hit)
        {
            _tempCheck.Update(dir);
            hit = _tempCheck.CollisionInfo.Hit;
            return _tempCheck.CollisionInfo.Hit.HaveHit;
        }
        
        public bool HaveCollision(Vector3 dir)
        {
            _tempCheck.Update(dir);
            return _tempCheck.CollisionInfo.Hit.HaveHit;
        }
    }
}

