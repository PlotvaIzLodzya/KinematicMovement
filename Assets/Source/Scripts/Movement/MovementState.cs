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

        private CollisionState _collisionCheck;
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
            _collisionCheck = new(collisionHandler);
        }


        public void Update()
        {
            Grounded.Update(-_transform.up);
            Ceiled.Update(_transform.up);
            LeftSide.Update(-_transform.right);
            RightSide.Update(_transform.right);
            ForwardSide.Update(_transform.forward);
            BackSide.Update(-_transform.forward);
        }

        public bool HaveCollision(Vector3 dir, out HitInfo hit)
        {
            _collisionCheck.Update(dir);
            hit = _collisionCheck.CollisionInfo.Hit;
            return _collisionCheck.CollisionInfo.Hit.HaveHit;
        }
        
        public bool HaveCollision(Vector3 dir)
        {
            _collisionCheck.Update(dir);
            return _collisionCheck.CollisionInfo.Hit.HaveHit;
        }
    }
}

