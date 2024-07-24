using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

public class ExteranlVelocityAccumulator
{
    private List<IExteranlMovemnt> _movements;

    private IExteranlMovemnt _rotation;
    public Vector3 TotalVelocity => GetTotalVelocity();

    private bool _haveRotation;

    public ExteranlVelocityAccumulator()
    {
        _movements = new();
    }

    public void Add(IExteranlMovemnt m)
    {
        if(_rotation == null)
        {
            _haveRotation = true;
            _rotation = m;
        }

        if(_movements.Contains(m) == false)
        {
            _movements.Add(m);
        }
    }

    public void Remove(IExteranlMovemnt m)
    {
        if(_rotation == m)
        {
            _haveRotation =false;
            _rotation = null;
        }

        if (_movements.Contains(m))
        {            
            _movements.Remove(m);
        }
    }

    public Vector3 GetPositionByRotation(Vector3 currentPos)
    {
        if(_haveRotation == false)
            return currentPos;

        return currentPos.RotatePointAroundPivot(_rotation.Position, _rotation.RotationVelocity);
    }

    private Vector3 GetTotalVelocity()
    {
        var velocity = Vector3.zero;

        foreach (var ext in _movements)
        {
            velocity += ext.Velocity;
        }
         
        return velocity;
    }
}
