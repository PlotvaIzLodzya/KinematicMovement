using TMPro;
using UnityEngine;

public interface IBody
{
    Quaternion Rotation { get; set; }
    Quaternion LocalRotation { get; set; }
    Vector3 LocalScale { get; set; }
    Vector3 LossyScale { get; }
    Vector3 Position { get; set; }

    void MovePosition(Vector3 position);
}

public class Body3D : IBody
{
    private Rigidbody _rigidbody;
    private Vector3 _previousScale;

    public Quaternion Rotation
    {
        get => _rigidbody.transform.rotation;
        set => _rigidbody.transform.rotation = value;
    }

    public Quaternion LocalRotation
    {
        get => _rigidbody.transform.localRotation;
        set => _rigidbody.transform.localRotation = value;
    }

    public Vector3 Position
    {
        get { return _rigidbody.position; }
        set 
        {
            _rigidbody.transform.position = value;
            _rigidbody.position = value;
        }
    }

    public Vector3 LocalScale 
    {
        get { return _rigidbody.transform.localScale; }
        set { _rigidbody.transform.localScale = value; }
    }

    public Vector3 LossyScale => _rigidbody.transform.lossyScale;

    public Body3D(Rigidbody rigidbody)
    {
        _rigidbody = rigidbody;
        _previousScale = rigidbody.transform.localScale;
    }

    public void MovePosition(Vector3 position)
    {
        _rigidbody.MovePosition(position);
    }
}

public class Body2D: IBody
{
    private Rigidbody2D _rigidbody;

    public float Angle => _rigidbody.rotation;

    public Quaternion Rotation 
    {
        get => _rigidbody.transform.rotation; 
        set => _rigidbody.transform.rotation = value; 
    }

    public Quaternion LocalRotation
    {
        get => _rigidbody.transform.localRotation;
        set => _rigidbody.transform.localRotation = value;
    }

    public Vector3 LocalScale
    {
        get { return _rigidbody.transform.localScale; }
        set { _rigidbody.transform.localScale = value; }
    }

    public Vector3 LossyScale => _rigidbody.transform.lossyScale;

    public Vector3 Position
    {
        get { return _rigidbody.position; }
        set
        {
            _rigidbody.transform.position = value;
            _rigidbody.position = value;
        }
    }


    public Body2D(Rigidbody2D rigidbody)
    {
        _rigidbody = rigidbody;
    }    

    public void MovePosition(Vector3 position)
    {
        _rigidbody.MovePosition(position);
    }
}
