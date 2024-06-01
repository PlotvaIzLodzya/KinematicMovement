using PlotvaIzLodzya.Player.Movement.CollideAndSlide.CollisionDetection;
using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;
using static PlotvaIzLodzya.Extensions.Extensions;
using static UnityEditor.PlayerSettings;

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

        public void Update(Vector3 direction, bool debug = false)
        {
            var wasInState = IsInState;
            var dist = CollisionConfig.ClipPreventingValue * 2;
            if (debug)
            {
                dist = CollisionConfig.ClipPreventingValue * 4;
            }
            IsInState = _collisionHandler.IsCollide(direction, out HitInfo hit, dist);
            IsEnterState = false;
            IsExitState = false;

            if(debug)
            {
                //Debug.Log(hit.Normal);
            }

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

