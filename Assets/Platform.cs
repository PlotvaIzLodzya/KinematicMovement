using System.Drawing;
using UnityEngine;

public interface IExteranlMovemnt
{
    Vector3 Velocity { get; }
    Vector3 UnscaledVelocity { get; }
}

public interface IPlatform: IExteranlMovemnt
{
    Vector3 Position { get; }
    Vector3 CollisionPoint { get; }
    Quaternion RotationVelocity { get; }
}

public class Platform : MonoBehaviour, IPlatform
{
    private Vector3 _prevPosition;
    private Quaternion _prevRotation;
    private IBody _rb;

    public Vector3 Position => _rb.Position;
    public Vector3 Velocity { get; private set; }
    public Vector3 UnscaledVelocity {get; private set; }
    public Vector3 CollisionPoint { get; private set; }
    public Quaternion RotationVelocity { get; private set; }


    private void Awake()
    {
        _rb = BodyBuilder.Create(gameObject);

    }

    private void OnEnable()
    {
        _prevPosition = _rb.Position;
        _prevRotation = _rb.Rotation;
    }

    private void FixedUpdate()
    {
        UpdateBody(Time.fixedDeltaTime);
    }

    private void OnCollisionStay(Collision collision)
    {
        if (collision.collider.TryGetComponent(out Movement movement))
        {
            CollisionPoint = collision.GetContact(0).point;
            if (movement.State.CanSetOnPlatform(this))
                movement.ExteranalMovementAccumulator.TryAdd(this);
            else
                movement.ExteranalMovementAccumulator.TryRemove(this);
        }
    }

    private void OnCollisionExit(Collision collision)
    {
        if (collision.collider.TryGetComponent(out Movement movement))
        {
            CollisionPoint = Vector3.zero;
            movement.ExteranalMovementAccumulator.TryRemove(this);
        }
    }

    private void OnCollisionStay2D(Collision2D collision)
    {
        if (collision.collider.TryGetComponent(out Movement movement))
        {
            CollisionPoint = collision.GetContact(0).point;
            if (movement.State.CanSetOnPlatform(this))
                movement.ExteranalMovementAccumulator.TryAdd(this);
            else
                movement.ExteranalMovementAccumulator.TryRemove(this);
        }
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.collider.TryGetComponent(out Movement movement))
        {
            CollisionPoint = Vector3.zero;
            movement.ExteranalMovementAccumulator.TryRemove(this);
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
        UnscaledVelocity = Velocity / deltaTime;
        RotationVelocity = _rb.Rotation * Quaternion.Inverse(_prevRotation);
        _prevRotation = _rb.Rotation;
        _prevPosition = _rb.Position;
    }
}
