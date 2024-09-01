using PlotvaIzLodzya.Extensions;
using UnityEngine;

namespace PlotvaIzLodzya.KinematicMovement.Examples
{
    public class Controll : MonoBehaviour
    {
        [SerializeField] private Movement _movement;
        [SerializeField] private float _sensitivity = 5f;
        [SerializeField] CharacterController _charasd;

        private Camera _camera;
        private Vector3 _direction;

        private void Awake()
        {
            _camera = Camera.main;
            _camera.transform.RotateAround(transform.position, Vector3.right, 20f);
        }

        private void Update()
        {
            _direction = Vector2.zero;
            //HandleRotation();

            if (Input.GetKey(KeyCode.D))
                _direction += _camera.transform.right;

            if (Input.GetKey(KeyCode.A))
                _direction -= _camera.transform.right;

            if (Input.GetKey(KeyCode.S))
                _direction -= _camera.transform.forward;

            if (Input.GetKey(KeyCode.W))
                _direction += _camera.transform.forward;

            if (Input.GetKeyUp(KeyCode.Space))
                _movement.CancelJump();

            if (Input.GetKeyDown(KeyCode.Space))
                _movement.Jump();


            _direction = _direction.GetHorizontal().normalized;
            if(_charasd != null)
            _charasd.Move(_direction * 15f * Time.deltaTime);
            _movement.Move(_direction);
        }

        private void HandleRotation()
        {
            var horVector = Vector3.up * Input.GetAxis("Mouse X") * _sensitivity;
            var vert = Input.GetAxis("Mouse Y") * _sensitivity;
            var vertPos = _camera.transform.localPosition.RotateAroundPivot(Vector3.zero, Quaternion.Euler(vert * Vector3.right));
            _camera.transform.localPosition = vertPos;
            _camera.transform.LookAt(transform.position);
            var horRot = Quaternion.Euler(horVector);
            transform.rotation *= horRot;
        }
    }
}