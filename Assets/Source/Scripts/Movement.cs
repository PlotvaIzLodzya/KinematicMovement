using PlotvaIzLodzya.Player.Movement.CollideAndSlide.CollisionDetection;
using System.Collections.Generic;
using UnityEngine;

namespace PlotvaIzLodzya.Player.Movement.CollideAndSlide
{


    public interface IMovable
    {
        void Move(Vector3 velocity);
    }

    public class Movement : MonoBehaviour, IMovable
    {
        [SerializeField] private PlayerInput _input;
        [SerializeField] private LayerMask _collisionMask;
        [SerializeField] private float _speed = 2f;
        [SerializeField] private float _maxSlopeAngle = 45f;
        [SerializeField] private bool _applyGravity;
        [SerializeField] private Vector3 _exteranalForce;

        private int _collideDepth;
        private float _clipPreventingValue;
        private ICollisionHandler _collisionHandler;
        private WorldConfig _wordlConfig;
        private ShapeConfig _characterConfig;
        private Rigidbody _rb;
        private Transform _transform;

        public bool IsGrounded { get; private set; }

        private void Awake()
        {
            _transform = transform;
            _rb = GetComponent<Rigidbody>();
            _rb.isKinematic = true;
            _wordlConfig = new (Vector3.down*14f, Vector3.up);
            _characterConfig = new(Vector3.up);
            var collider = GetComponent<Collider>();
            _collisionHandler = CollisionHandlerBuilder.Create(collider, _characterConfig);
            _clipPreventingValue = 0.015f;
            _collideDepth = 5;
        }


        // Высокая скорость позволяте слетать с наклонных поврехностей
        private void Update()
        {
            var vel = _input.Direction * _speed;


            if (_applyGravity)
                vel += _wordlConfig.Gravity;

            vel += _exteranalForce;
            Move(vel);

            if (Input.GetKeyDown(KeyCode.Space))
                Jump();

            IsGrounded = _collisionHandler.IsCollide(transform.position, -_characterConfig.Up, out RaycastHit hit, 2 * _clipPreventingValue);
        }
        public void Jump()
        {
            Debug.Log($"Can jump: {IsGrounded}");
        }

        public void Move(Vector3 vel)
        {
            vel = CollideAndSlide_recursive(vel * Time.deltaTime, _transform.position, 0);
            _transform.position += vel;
        }

        private Vector3 ProjectOnSurface(Vector3 vector, Vector3 normal)
        {
            vector = Vector3.ProjectOnPlane(vector, normal).normalized * vector.magnitude;

            return vector;
        }


        private Vector3 CollideAndSlide_recursive(Vector3 vel, Vector3 currentPos, int depth)
        {
            if (depth >= _collideDepth)
                return Vector3.zero;

            float dist = vel.magnitude + _clipPreventingValue;
            var dir = vel.normalized;
            RaycastHit hit;

            if (_collisionHandler.IsCollide(currentPos, dir, out hit, dist))
            {
                var velToNextStep = dir * (hit.distance - _clipPreventingValue);
                var leftOverVel = vel - velToNextStep;

                var nextPos = currentPos + velToNextStep;

                float angle = Vector3.Angle(_wordlConfig.WorldUp, hit.normal);
                //Debug.Log(angle);
                var projectedleftOverVel = ProjectOnSurface(leftOverVel, hit.normal).normalized * leftOverVel.magnitude;

                projectedleftOverVel = HandleSlope(angle, vel, projectedleftOverVel, hit.normal);

                vel = velToNextStep + CollideAndSlide_recursive(projectedleftOverVel, nextPos, depth + 1);

                return vel;
            }

            return vel;
        }

        private bool IsTooSteepSlope(float angle)
        {
            return angle >= _maxSlopeAngle;
        }

        private Vector3 HandleSlope(float slopeAngle, Vector3 vel, Vector3 projectedleftOverVel, Vector3 surfaceNormal)
        {
            if (IsGrounded == false)
                return projectedleftOverVel;

            if (IsTooSteepSlope(slopeAngle))
            {
                projectedleftOverVel.y = vel.y;
                projectedleftOverVel = ScaleByHorizontalVelocity(vel, projectedleftOverVel, surfaceNormal);
            }
            else
            {
                vel.y = projectedleftOverVel.y;
                projectedleftOverVel = vel.normalized * projectedleftOverVel.magnitude;
            }


            return projectedleftOverVel;
        }

        public Vector3 ScaleByHorizontalVelocity(Vector3 vel, Vector3 projectedVel, Vector3 surfaceNormal)
        {
            vel.y = 0;
            surfaceNormal.y = 0;
            float scale = 1 + Vector3.Dot(vel.normalized, surfaceNormal.normalized);

            var scaledVel = projectedVel * scale;

            return scaledVel;
        }

    }
}

