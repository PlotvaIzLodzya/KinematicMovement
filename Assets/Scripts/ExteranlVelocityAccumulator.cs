using System.Collections.Generic;
using UnityEditor.Build;
using UnityEngine;

public class ExteranlVelocityAccumulator
{
    private List<IExteranlMovemnt> _movements;

    private IPlatform _platform;
    private IExteranlMovementState _state;
    public Vector3 TotalVelocity => GetTotalVelocity();

    private bool _haveRotation => _platform != null;

    public ExteranlVelocityAccumulator(IExteranlMovementState state)
    {
        _movements = new();
        _state = state;
    }

    public bool TryAdd(IExteranlMovemnt m)
    {
        var added = false;
        if (m is IPlatform platform && _state.TrySetOnPlatform(platform))
        {
            _platform = platform;
            added = true;
        }

        if(_movements.Contains(m) == false)
        {
            _movements.Add(m);
            added = true;
        }

        return added;
    }

    public bool TryRemove(IExteranlMovemnt m)
    {
        bool removed = false;
        if(_platform != null && m == _platform)
        {
            _platform = null;
            _state.LeavePlatform(m as IPlatform);
            removed = true;
        }

        if (_movements.Contains(m))
        {            
            _movements.Remove(m);
            removed = true;
        }

        return removed;
    }

    public Vector3 GetPositionByRotation(Vector3 currentPos)
    {
        if(_haveRotation == false)
            return currentPos;

        return currentPos.RotatePointAroundPivot(_platform.Position, _platform.RotationVelocity);
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
