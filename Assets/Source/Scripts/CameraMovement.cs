using UnityEngine;

public class CameraMovement : MonoBehaviour
{
    [SerializeField] private Transform _followedObject;
    [SerializeField] private PlayerInput _playerInput;
    [SerializeField] private float _maxDistance;

    private void Update()
    {
        var mouseDelta = new Vector3(_playerInput.MouseDelta.x, 0, _playerInput.MouseDelta.y);
        transform.position += (Vector3)_playerInput.MouseDelta;

        transform.position = Clamp(_maxDistance);
    }

    private Vector3 Clamp( float maxDistance)
    {
        var dir = (_followedObject.position - transform.position).normalized;

        dir *= maxDistance;

        var pos = _followedObject.position - dir;

        return pos;
    }
}
