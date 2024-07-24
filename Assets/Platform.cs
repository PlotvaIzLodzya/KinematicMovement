using System.Drawing;
using UnityEngine;

public interface IExteranlMovemnt
{
    Vector3 Position { get; }
    Vector3 Velocity { get; }
    Quaternion RotationVelocity { get; }
}

public class Platform : MonoBehaviour, IExteranlMovemnt
{
    private Vector3 _prevPosition;
    private Quaternion _prevRotation;
    private IBody _rb;

    public Vector3 Position => _rb.Position;
    public Quaternion RotationVelocity { get; private set; }
    public Vector3 Velocity { get; private set; }

    private void Awake()
    {
        _rb = BodyBuilder.Create(gameObject);
    }

    private void FixedUpdate()
    {
        UpdateBody(Time.fixedDeltaTime);
    }

    private void OnCollisionStay(Collision collision)
    {
        if (collision.collider.TryGetComponent(out Movement movement))
        {
            movement.ExteranalMovementAccumulator.Add(this);
        }
    }

    private void OnCollisionExit(Collision collision)
    {
        if (collision.collider.TryGetComponent(out Movement movement))
        {
            movement.ExteranalMovementAccumulator.Remove(this);
        }
    }

    private void OnCollisionStay2D(Collision2D collision)
    {
        if (collision.collider.TryGetComponent(out Movement collider))
        {
            collider.ExteranalMovementAccumulator.Add(this);
        }
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.collider.TryGetComponent(out Movement collider))
        {
            collider.ExteranalMovementAccumulator.Remove(this);
        }
    }

    public virtual void UpdateBody(IBody body, float deltaTime)
    {
        body.Position = transform.position;
        body.Rotation = transform.rotation;
    }

    private void UpdateBody(float deltaTime)
    {
        UpdateBody(_rb, deltaTime);
        Velocity = _rb.Position - _prevPosition;
        RotationVelocity = _rb.Rotation * Quaternion.Inverse(_prevRotation);
        _prevRotation = _rb.Rotation;
        _prevPosition = _rb.Position;
    }
}
