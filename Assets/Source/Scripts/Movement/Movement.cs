using PlotvaIzLodzya.Player.Movement.CollideAndSlide.CollisionDetection;
using UnityEngine;
using System;

namespace PlotvaIzLodzya.Player.Movement.CollideAndSlide
{
    public interface IMovable
    {
        void Move(Vector3 direction);
    }

    public class Movement : MonoBehaviour, IMovable
    {
        [SerializeField] private float _maxSlopeAngle = 45f;
        [SerializeField] private bool _applyGravity;
        [SerializeField] private CollisionConfig _collisionConfig;

        private int _collideDepth;
        private Vector3 _exteranalForce;
        private Vector3 _direction;
        private ICollisionHandler _collisionHandler;
        private Transform _transform;
        private WorldConfig _wordlConfig;
        private MovementConfig _movementConfig;

        public MovementState State { get; private set; }
        public bool IsGrounded => State.Grounded.IsInState;

        public void Init(MovementConfig movementConfig)
        {
            _transform = transform;
            _movementConfig = movementConfig ?? throw new NullReferenceException($"{nameof(movementConfig)} is null");
            _wordlConfig = new (Vector3.down*14f, Vector3.up);
            _collisionHandler = CollisionHandlerBuilder.Create(gameObject, _collisionConfig);
            State = new(_collisionHandler, transform);
            _collideDepth = 5;
        }

        private void Update()
        {
            State.Update();
            var vel = _direction *_movementConfig.Speed;

            if (_applyGravity)
                vel += _wordlConfig.Gravity;

            vel += _exteranalForce;
            vel = SnapToSurface(vel);
            
            Translate(vel);

            if (Input.GetKeyDown(KeyCode.Space))
                Jump();

            _direction = Vector3.zero;
        }

        public void Jump()
        {
            Debug.Log($"Can jump: {IsGrounded}");
        }

        public void Move(Vector3 direction)
        {
            _direction = direction;
        }

        private void Translate(Vector3 vel)
        {
            vel = CollideAndSlide_recursive(vel * Time.deltaTime, _transform.position);
            _transform.position += vel;
        }

        private Vector3 CollideAndSlide_recursive(Vector3 vel, Vector3 currentPos, int currentDepth = 0)
        {
            if (currentDepth >= _collideDepth)
                return Vector3.zero;

            float dist = vel.magnitude + _collisionConfig.ClipPreventingValue;
            var dir = vel.normalized;

            if (_collisionHandler.IsCollide(currentPos, dir, out HitInfo hit, dist))
            {
                var velToNextStep = dir * (hit.distance - _collisionConfig.ClipPreventingValue);
                var leftOverVel = vel - velToNextStep;

                var nextPos = currentPos + velToNextStep;

                float angle = Vector3.Angle(_wordlConfig.WorldUp, hit.normal);

                
                var projectedleftOverVel = Vector3.ProjectOnPlane(leftOverVel, hit.normal);

                projectedleftOverVel = HandleSlope(angle, vel, projectedleftOverVel, hit.normal);

                vel = velToNextStep + CollideAndSlide_recursive(projectedleftOverVel, nextPos, currentDepth + 1);

                return vel;
            }

            return vel;
        }

        private bool IsSlopeTooSteep(float angle)
        {
            return angle >= _maxSlopeAngle;
        }

        private Vector3 HandleSlope(float slopeAngle, Vector3 vel, Vector3 projectedleftOverVel, Vector3 surfaceNormal)
        {
            if (IsGrounded == false)
                return projectedleftOverVel;

            if (IsSlopeTooSteep(slopeAngle))
            {
                projectedleftOverVel = ScaleHorizontalVelocity(vel, projectedleftOverVel, surfaceNormal);
            }
            else
            {
                vel.y = projectedleftOverVel.y;
                projectedleftOverVel = vel.normalized * projectedleftOverVel.magnitude;
            }

            return projectedleftOverVel;
        }

        private Vector3 ScaleHorizontalVelocity(Vector3 vel, Vector3 projectedVel, Vector3 surfaceNormal)
        {
            vel.y = 0;
            surfaceNormal.y = 0;
            projectedVel.y = vel.y;
            float scale = 1 + Vector3.Dot(vel.normalized, surfaceNormal.normalized);

            var scaledVel = projectedVel * scale;

            return scaledVel;
        }

        private Vector3 SnapToSurface(Vector3 vel)
        {
            State.HaveCollision(vel.normalized, out HitInfo hit);
            float angle = Vector3.Angle(_wordlConfig.WorldUp, hit.normal);

            if (IsGrounded && IsSlopeTooSteep(angle) == false)
                vel = ProjectVelocityOnSurface(vel, State.Grounded.CollisionInfo.Hit.normal);

            return vel;
        }

        private Vector3 ProjectVelocityOnSurface(Vector3 vel, Vector3 normal)
        {
            vel.y = 0;
            vel = Vector3.ProjectOnPlane(vel, normal);

            return vel;
        }
    }
}

