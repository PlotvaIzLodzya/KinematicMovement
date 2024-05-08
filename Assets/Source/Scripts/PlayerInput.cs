using UnityEngine;

namespace PlotvaIzLodzya.Player
{
    public class PlayerInput : MonoBehaviour
    {
        public Vector3 Direction { get; private set; }
        public Vector3 DirectionRelativeToCamera { get; private set; }
        public Vector2 MouseDelta { get; private set; }
        public bool IsAnyDirectionButtonHolded { get; private set; }

        public bool JumpButtonPressed { get; private set; }

        private Camera _camera;

        private void Awake()
        {
            _camera = Camera.main;
        }

        private void Update()
        {
            IsAnyDirectionButtonHolded = false;

            DirectionRelativeToCamera = Vector3.zero;
            Direction = Vector3.zero;
            MouseDelta = GetMouseDelta();
            var dir = GetDirectionRelativeToCamera();

            JumpButtonPressed = Input.GetKeyDown(KeyCode.Space);

            if (IsAnyDirectionButtonHolded)
                SetDirection(dir);
        }

        private Vector2 GetMouseDelta()
        {
            var delta = Vector2.zero;
            delta.x = Input.GetAxis("Mouse X");
            delta.y = -Input.GetAxis("Mouse Y");

            return delta;
        }

        private Vector3 GetDirectionRelativeToCamera()
        {
            var dir = Vector3.zero;
            var forward = _camera.transform.forward;
            forward.y = 0;
            var right = _camera.transform.right;
            right.y = 0;


            if (IsDirectionButtonHolded(KeyCode.W))
                dir += forward;
            if (IsDirectionButtonHolded(KeyCode.S))
                dir += -forward;
            if (IsDirectionButtonHolded(KeyCode.D))
                dir += right;
            if (IsDirectionButtonHolded(KeyCode.A))
                dir += -right;

            return dir;
        }

        private bool IsDirectionButtonHolded(KeyCode keyCode)
        {
            if (IsButtonHolded(keyCode))
            {
                IsAnyDirectionButtonHolded = true;
                return true;
            }

            return false;
        }

        public bool IsButtonHolded(KeyCode keyCode)
        {
            return Input.GetKey(keyCode);
        }

        private void SetDirection(Vector3 direction)
        {
            Direction = direction.normalized;
            DirectionRelativeToCamera = GetDirectionRelativeToCamera(direction).normalized;
        }

        private Vector3 GetDirectionRelativeToCamera(Vector3 direction)
        {
            var cameraForwardDirection = _camera.transform.forward;
            var cameraRightDirection = _camera.transform.right;

            cameraForwardDirection.y = 0;
            var directionRelativeToCamera = cameraForwardDirection + direction;

            return directionRelativeToCamera;
        }
    }
}