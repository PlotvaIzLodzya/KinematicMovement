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
        public CollisionInfo CollisionInfo { get;  private set; }

        public Action<CollisionInfo> OnStateEnter;

        public CollisionState(ICollisionHandler collisionHandler)
        {
            _collisionHandler = collisionHandler;
        }

        public void Update(Vector3 direction)
        {
            var wasInState = IsInState;
            IsInState = _collisionHandler.IsCollide(direction, out RaycastHit hit, 2 * _collisionHandler.Config.ClipPreventingValue);
            IsEnterState = false;

            CollisionInfo = new CollisionInfo()
            {
                Hit = hit,
            };

            if(wasInState == false && IsInState)
            {
                IsEnterState = true;
                OnStateEnter?.Invoke(CollisionInfo);
            }
        }
    }
}

