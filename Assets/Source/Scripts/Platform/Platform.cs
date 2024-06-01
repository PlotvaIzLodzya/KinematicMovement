using PlotvaIzLodzya.Extensions;
using PlotvaIzLodzya.Player.Movement;
using PlotvaIzLodzya.Player.Movement.CollideAndSlide;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;

namespace PlotvaIzLodzya.Movement.Platforms
{
    public class Platform : MonoBehaviour
    {
        [SerializeField] private List<Transform> _points;
        [SerializeField] private float _time;

        private Rigidbody2D _rb;
        private BoxCollider2D _boxCollider;
        private bool _stop;
        private float _speed;
        private Transform _currentPoint;
        private RaycastHit2D[] _hits;
        public Vector3 Velocity { get; private set; }

        private void Awake()
        {
            _rb = GetComponent<Rigidbody2D>();
            var dist = 0f;
            _boxCollider = this.GetComponentNullAwarness<BoxCollider2D>();
            _hits = new RaycastHit2D[10];
            for (int Index = 0; Index < _points.Count; Index++)
            {
                var nextIndex= Index+1;
                if(nextIndex < _points.Count)
                    dist += Vector3.Distance(_points[Index].position, _points[nextIndex].position);
            }

            _speed = dist / _time;
            _currentPoint = _points[0];
            StartCoroutine(LoopingMovement());
        }

        private IEnumerator LoopingMovement()
        {
            var index = 0;
            var indexDirection = 1;
            while (_stop == false)
            {
                yield return new WaitForSeconds(0.2f);
                if (index >= _points.Count - 1)
                    indexDirection = -1;

                if (index <= 0)
                    indexDirection = 1;


                var nextIndex = index + indexDirection;
                index = nextIndex;
                var nextPoint = _points[nextIndex];
                yield return MovingToPoint(nextPoint);                
            }
        }

        private IEnumerator MovingToPoint(Transform nextPoint)
        {
            var dir = (nextPoint.position - _currentPoint.position).normalized;
            var coveredDist = 0f;
            var dist = Vector3.Distance(_currentPoint.position, nextPoint.position);
            Velocity = dir * _speed;

            while(coveredDist < dist)
            {
                yield return null;
                Velocity = dir.normalized * _speed;
                
                var speed = _speed * Time.deltaTime;
                transform.position += Velocity * Time.deltaTime;
                coveredDist += speed;
            }
            Velocity = Vector3.zero;

            _currentPoint = nextPoint;
        }

        public void Update()
        {
            SetMovables();
        }

        private void SetMovables()
        {
            var scaledSize = _boxCollider.size;
            scaledSize.x *= transform.localScale.x;
            scaledSize.y *= transform.localScale.y;
            var dist = CollisionConfig.CollsisionDist;
            foreach (var hit in _hits)
            {
                if (hit.transform != null && hit.transform.TryGetComponent(out IMovable movable))
                {
                    movable.ExternalVelocity = Vector3.zero;
                }
            }
            Array.Clear(_hits,0, 10);

            Physics2D.BoxCastNonAlloc(transform.position, scaledSize, 0, Vector3.up, _hits, dist);

            foreach (var hit in _hits)
            {
                var velocity = Velocity;
                if (hit.transform != null && hit.transform.TryGetComponent(out IMovable movable))
                {
                    movable.ExternalVelocity = velocity;
                }
            }
        }

        private bool RaycastHitFilter(RaycastHit2D raycastHit)
        {
            return raycastHit.collider.TryGetComponent(out IMovable m) && raycastHit.collider != _boxCollider;
        }
    }
}
