using UnityEngine;

public interface IBody
{
    Quaternion Rotation { get; set; }
    Quaternion LocalRotation { get; set; }
    Vector3 Scale { get; set; }
    Vector3 Position { get; set; }

    void MovePosition(Vector3 position);
}

public class Body3D : IBody
{
    private Rigidbody _rigidbody;

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
            _rigidbody.position = value;
            _rigidbody.transform.position = value;
        }
    }

    public Vector3 Scale 
    {
        get { return _rigidbody.transform.localScale; }
        set { _rigidbody.transform.localScale = value; }
    }

    public Body3D(Rigidbody rigidbody)
    {
        _rigidbody = rigidbody;
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

    public Vector3 Scale
    {
        get { return _rigidbody.transform.localScale; }
        set { _rigidbody.transform.localScale = value; }
    }

    public Vector3 Position
    {
        get { return _rigidbody.position; }
        set
        {
            _rigidbody.position = value;
            _rigidbody.transform.position = value;
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
