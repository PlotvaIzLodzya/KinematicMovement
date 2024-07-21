﻿using UnityEngine;

public interface IBody
{
    Vector3 Position { get; set; }
    void MovePosition(Vector3 position);
}

public class Body3D : IBody
{
    private Rigidbody _rigidbody;
    public Quaternion rotation =>_rigidbody.rotation;
    public Vector3 Position
    {
        get { return _rigidbody.position; }
        set {
            _rigidbody.position = value;
            _rigidbody.transform.position = value;
            }
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

    public float Rotation => _rigidbody.rotation;

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
