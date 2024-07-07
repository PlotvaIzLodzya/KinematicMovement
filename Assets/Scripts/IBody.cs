using UnityEngine;

public interface IBody
{
    Vector3 Position { get; set; }
}

public class Body3D : IBody
{
    private Rigidbody _rigidbody;

    public Vector3 Position
    {
        get { return _rigidbody.position; }
        set { _rigidbody.position = value; }
    }

    public Body3D(Rigidbody rigidbody)
    {
        _rigidbody = rigidbody;
    }
}

public class Body2D: IBody
{
    private Rigidbody2D _rigidbody;

    public Vector3 Position
    {
        get { return _rigidbody.position; }
        set { _rigidbody.position = value; }
    }

    public Body2D(Rigidbody2D rigidbody)
    {
        _rigidbody = rigidbody;
    }    
}
