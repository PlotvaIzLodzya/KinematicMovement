using PlotvaIzLodzya.Extensions;
using PlotvaIzLodzya.Player.Movement;
using PlotvaIzLodzya.Player.Movement.CollideAndSlide;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace PlotvaIzLodzya.Movement.Platforms
{
    public class MovingPlatform : MonoBehaviour
    {
        [SerializeField] private List<Transform> _points;
        [SerializeField] private float _time;

        private Rigidbody2D _rb;
        private BoxCollider2D _boxCollider;
        private bool _stop;
        private List<IMovable> _movables;
        private float _speed;
        private Transform _currentPoint;
        public Vector3 Velocity { get; private set; }

        private void Awake()
        {
            _rb = GetComponent<Rigidbody2D>();
            var dist = 0f;
            _movables = new();
            _boxCollider = this.GetComponentNullAwarness<BoxCollider2D>();

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
            var dir = nextPoint.position - _currentPoint.position;
            var coveredDist = 0f;
            var dist = Vector3.Distance(_currentPoint.position, nextPoint.position);
            Velocity = dir.normalized * _speed;

            while(coveredDist < dist)
            {                 
                yield return null;
                Velocity = dir.normalized * _speed;
                var speed = _speed * Time.deltaTime;
                _rb.MovePosition(_rb.position + (Vector2)Velocity * Time.deltaTime);
                coveredDist += speed;
            }
            Velocity = Vector3.zero;

            _currentPoint = nextPoint;
        }

        public void Update()
        {
            SetMovables();
            //Velocity = Vector3.right * _speed;
            //_rb.MovePosition(_rb.position + (Vector2)Velocity * Time.deltaTime);
        }

        private void SetMovables()
        {
            var scaledSize = _boxCollider.size;
            scaledSize.x *= transform.localScale.x;
            scaledSize.y *= transform.localScale.y;
            var dist = CollisionConfig.ClipPreventingValue;
            var collidersToMove = Physics2D.BoxCastAll(transform.position, scaledSize, 0, Velocity.normalized, dist).ToList();
            var collidersOnTop = Physics2D.BoxCastAll(transform.position, scaledSize, 0, Vector2.up, dist);
            var colliderToAdd = collidersOnTop.Except(collidersToMove);
            collidersToMove.AddRange(colliderToAdd);
            var movables = collidersToMove.Where(h => RaycastHitFilter(h)).Select(c => c.collider.GetComponent<IMovable>()).ToList();
            var notOnPlatform = _movables.Except(movables);

            foreach (var m in notOnPlatform)
            {
                m.ExternalVelocity = Vector3.zero;
            }

            _movables = movables;
            
            foreach (var movable in _movables)
            {
                var velocity = Velocity;
                movable.ExternalVelocity = velocity;
            }
        }

        private bool RaycastHitFilter(RaycastHit2D raycastHit)
        {
            return raycastHit.collider.TryGetComponent(out IMovable m) && raycastHit.collider != _boxCollider;
        }
    }
}
