using System.Collections;
using System.Collections.Generic;
using Unity.Android.Types;
using Unity.VisualScripting;
using UnityEngine;

public interface IExteranlVelocity
{
    Vector3 Velocity { get; }
}

public class Platform : MonoBehaviour, IExteranlVelocity
{
    [SerializeField] private float _ySpeed;
    [SerializeField] private float _xSpeed;
    [SerializeField] private float _rotationSpeed;

    private IBody _rb;
    public Vector3 Velocity { get; private set; }

    private void Awake()
    {
        _rb = BodyBuilder.Create(gameObject);
    }

    private void FixedUpdate()
    {
        Move(Time.fixedDeltaTime);
    }

    private void OnCollisionStay(Collision collision)
    {
        Debug.Log("hi");
        if (collision.collider.TryGetComponent(out Movement collider))
        {
            collider.VelocityAccumalator.Add(this);
        }
    }

    private void OnCollisionExit(Collision collision)
    {
        if (collision.collider.TryGetComponent(out Movement collider))
        {
            collider.VelocityAccumalator.Remove(this);
        }
    }

    private void OnCollisionStay2D(Collision2D collision)
    {
        if (collision.collider.TryGetComponent(out Movement collider))
        {
            collider.VelocityAccumalator.Add(this);
        }
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.collider.TryGetComponent(out Movement collider))
        {
            collider.VelocityAccumalator.Remove(this);
            //collider.ReleaseFromPlatform();
        }
    }

    private void OnStay()
    {

    }

    public void Move(float deltaTime)
    {
        var vert = Vector3.down * _ySpeed;
        var hor = Vector3.right * _xSpeed;
        Velocity = vert + hor;
        _rb.Position += Velocity * deltaTime;
        transform.position += Velocity * deltaTime;
    }
}
