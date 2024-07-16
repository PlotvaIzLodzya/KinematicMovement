﻿using System.Collections.Generic;
using UnityEngine;

public class ExteranlVelocityAccumalator
{
    private List<IExteranlVelocity> _exteranls;
    public Vector3 TotalVelocity => GetTotalVelocity();

    public ExteranlVelocityAccumalator()
    {
        _exteranls = new();
    }

    public void Add(IExteranlVelocity v)
    {
        if(_exteranls.Contains(v) == false)
        {
            Debug.Log("add");
            _exteranls.Add(v);
        }
    }

    public void Remove(IExteranlVelocity v)
    {
        if (_exteranls.Contains(v))
        {
            Debug.Log("remove");
            _exteranls.Remove(v);
        }
    }

    private Vector2 GetTotalVelocity()
    {
        var velocity = Vector3.zero;

        foreach (var ext in _exteranls)
        {
            velocity += ext.Velocity;
        }

        return velocity;
    }
}
