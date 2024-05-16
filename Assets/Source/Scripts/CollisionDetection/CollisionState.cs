using PlotvaIzLodzya.Player.Movement.CollideAndSlide.CollisionDetection;
using System;
using UnityEngine;

namespace PlotvaIzLodzya.Player.Movement.CollideAndSlide
{
    public class CollisionState
    {
        private ICollisionHandler _collisionHandler;

        public bool IsInState { get; private set; }
        public bool IsEnterState { get; private set; }
        public bool IsExitState { get; private set; }
        public CollisionInfo CollisionInfo { get;  private set; }

        public Action<CollisionInfo> OnStateEnter;
        public Action<CollisionInfo> OnStateExit;

        public CollisionState(ICollisionHandler collisionHandler)
        {
            _collisionHandler = collisionHandler;
        }

        public void Update(Vector3 direction)
        {
            var wasInState = IsInState;
            IsInState = _collisionHandler.IsCollide(direction, out HitInfo hit, CollisionConfig.CollsisionDist);
            IsEnterState = false;
            IsExitState = false;

            CollisionInfo = new CollisionInfo()
            {
                Hit = hit,
            };

            if(wasInState == false && IsInState)
            {
                IsEnterState = true;
                OnStateEnter?.Invoke(CollisionInfo);
            }

            if(wasInState && IsInState == false)
            {
                IsExitState = true;
                OnStateExit?.Invoke(CollisionInfo);
            }


        }
    }
}

